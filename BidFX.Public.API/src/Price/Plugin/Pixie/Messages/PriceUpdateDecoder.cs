using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BidFX.Public.API.Price.Plugin.Pixie.Fields;
using BidFX.Public.API.Price.Tools;
using log4net;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    internal class PriceUpdateDecoder
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void Visit(Stream stream, int size, IDataDictionary dataDictionary,
            IGridHeaderRegistry gridHeaderRegistry, ISyncable syncable)
        {
            for (var i = 0; i < size; i++)
            {
                var type = (char) stream.ReadByte();
                var sid = (int) Varint.ReadU32(stream);
                switch (type)
                {
                    case 'f':
                        syncable.PriceImage(sid, DecodePriceUpdate(stream, dataDictionary));
                        break;
                    case 'p':
                        syncable.PriceUpdate(sid, DecodePriceUpdate(stream, dataDictionary));
                        break;
                    case 's':
                        DecodeStatusUpdate(stream, sid, syncable);
                        break;
                    case 'G':
                        DecodeFullGridImage(sid, stream, dataDictionary, gridHeaderRegistry, syncable);
                        break;
                    case 'g':
                        DecodeGridUpdate(sid, stream, dataDictionary, gridHeaderRegistry, syncable);
                        break;
                    default:
                        throw new DecodingException("unexpected price update type '" + type + "'");
                }
            }
        }

        private static void DecodeStatusUpdate(Stream stream, int sid, ISyncable syncable)
        {
            var status = (char) stream.ReadByte();
            var reason = DecodeAsString(stream);
            syncable.PriceStatus(sid, PriceStatusCodec.DecodeStatus(status), reason);
        }

        private static Dictionary<string, object> DecodePriceUpdate(Stream stream, IDataDictionary dataDictionary)
        {
            var size = (int) Varint.ReadU32(stream);
            var priceMap = new Dictionary<string, object>(size);
            for (var i = 0; i < size; i++)
            {
                var fid = (int) Varint.ReadU32(stream);
                if (fid == FieldDef.ReferecingFid)
                {
                    var referenceFid = Varint.ReadU32(stream);
                    var fieldDef = GetFieldDef(stream, dataDictionary, priceMap, (int) referenceFid);
                    priceMap[fieldDef.Name] = null;
                }
                else
                {
                    var fieldDef = GetFieldDef(stream, dataDictionary, priceMap, fid);
                    priceMap[fieldDef.Name] = DecodeField(stream, fieldDef);
                }
            }
            return priceMap;
        }

        private static void DecodeFullGridImage(int sid, Stream stream, IDataDictionary dataDictionary,
            IGridHeaderRegistry gridHeaderRegistry, ISyncable synable)
        {
            var columnCount = (int) Varint.ReadU32(stream);
            synable.StartGridImage(sid, columnCount);
            var headers = new FieldDef[columnCount];
            for (var i = 0; i < columnCount; i++)
            {
                var fid = (int) Varint.ReadU32(stream);
                if (fid == FieldDef.ReferecingFid)
                {
                    throw new IllegalStateException("unexpected field " + fid);
                }
                var rowCount = (int) Varint.ReadU32(stream);
                var column = new object[rowCount];
                var fieldDef = GetFieldDef(stream, dataDictionary, new Dictionary<string, object>(), fid);
                for (var j = 0; j < rowCount; j++)
                {
                    column[j] = DecodeField(stream, fieldDef);
                }
                synable.ColumnImage(fieldDef.Name, rowCount, column);
                headers[i] = fieldDef;
            }
            gridHeaderRegistry.SetGridHeader(sid, headers);
            synable.EndGridImage();
        }

        private static void DecodeGridUpdate(int sid, Stream stream, IDataDictionary dataDictionary,
            IGridHeaderRegistry gridHeaderRegistry, ISyncable syncable)
        {
            var header = gridHeaderRegistry.GetGridHeader(sid);

            var columnCount = (int) Varint.ReadU32(stream);
            syncable.StartGridUpdate(sid, columnCount);

            for (var i = 0; i < columnCount; i++)
            {
                var updateType = (char) stream.ReadByte();
                var cid = (int) Varint.ReadU32(stream);
                var rowCount = (int) Varint.ReadU32(stream);
                var column = new object[rowCount];

                var fieldDef = header[cid];
                for (var j = 0; j < rowCount; j++)
                {
                    column[j] = DecodeField(stream, fieldDef);
                }
                if (updateType == 'f')
                {
                    syncable.FullColumnUpdate(fieldDef.Name, cid, rowCount, column);
                }
                else
                {
                    var rids = new int[rowCount];

                    for (var j = 0; j < rowCount; j++)
                    {
                        rids[j] = (int) Varint.ReadU32(stream);
                    }
                    if (updateType == 't')
                    {
                        var deletedFrom = (int) Varint.ReadU32(stream);
                        syncable.PartialTruncatedColumnUpdate(fieldDef.Name, cid, rowCount, column, rids, deletedFrom);
                    }
                    else
                    {
                        syncable.PartialColumnUpdate(fieldDef.Name, cid, rowCount, column, rids);
                    }
                }
            }
            syncable.EndGridUpdate();
        }

        private static FieldDef GetFieldDef(Stream stream, IDataDictionary dataDictionary,
            Dictionary<string, object> priceMap, int fid)
        {
            var fieldDef = dataDictionary.FieldDefByFid(fid);
            if (fieldDef == null)
            {
                throw new DecodingException("received a FID that is not in the data dictionary: " + fid, stream,
                    priceMap);
            }
            return fieldDef;
        }

        internal static object DecodeField(Stream stream, FieldDef fieldDef)
        {
            switch (fieldDef.Type)
            {
                case FieldType.Double:
                    return DecodeAsDouble(stream, fieldDef);
                case FieldType.Long:
                    return DecodeAsLong(stream, fieldDef);
                case FieldType.Integer:
                    return DecodeAsInteger(stream, fieldDef);
                case FieldType.String:
                    return DecodeAsString(stream);
                default:
                    fieldDef.Encoding.SkipFieldValue(stream);
                    return null;
            }
        }

        private static double? DecodeAsDouble(Stream stream, FieldDef fieldDef)
        {
            switch (fieldDef.Encoding)
            {
                case FieldEncoding.Fixed4:
                    return StreamReaderHelper.ReadFloat4(stream);
                case FieldEncoding.Fixed8:
                    return StreamReaderHelper.ReadDouble8(stream);
                case FieldEncoding.Varint:
                    return DecodeDecimal((long) Varint.ReadU64(stream), fieldDef.Scale);
                case FieldEncoding.ZigZag:
                    return DecodeDecimal(Varint.ZigzagToLong(Varint.ReadU64(stream)), fieldDef.Scale);
            }
            fieldDef.Encoding.SkipFieldValue(stream);
            return null;
        }

        private static long? DecodeAsLong(Stream stream, FieldDef fieldDef)
        {
            switch (fieldDef.Encoding)
            {
                case FieldEncoding.Fixed1:
                    return stream.ReadByte();
                case FieldEncoding.Fixed2:
                    return StreamReaderHelper.ReadShort(stream);
                case FieldEncoding.Fixed3:
                    return StreamReaderHelper.ReadMedium(stream);
                case FieldEncoding.Fixed4:
                    return StreamReaderHelper.ReadInt4(stream);
                case FieldEncoding.Fixed8:
                    return StreamReaderHelper.ReadLong(stream);
                case FieldEncoding.Varint:
                    return (long) Varint.ReadU64(stream) * 1 ^ fieldDef.Scale;
                case FieldEncoding.ZigZag:
                    return Varint.ZigzagToLong(Varint.ReadU64(stream));
            }
            fieldDef.Encoding.SkipFieldValue(stream);
            return null;
        }

        private static int? DecodeAsInteger(Stream stream, FieldDef fieldDef)
        {
            switch (fieldDef.Encoding)
            {
                case FieldEncoding.Fixed1:
                    return stream.ReadByte();
                case FieldEncoding.Fixed2:
                    return StreamReaderHelper.ReadShort(stream);
                case FieldEncoding.Fixed3:
                    return StreamReaderHelper.ReadMedium(stream);
                case FieldEncoding.Fixed4:
                    return StreamReaderHelper.ReadInt4(stream);
                case FieldEncoding.Fixed8:
                    return (int) StreamReaderHelper.ReadLong(stream);
                case FieldEncoding.Varint:
                    return (int) Varint.ReadU32(stream) * 1 ^ fieldDef.Scale;
                case FieldEncoding.ZigZag:
                    return Varint.ZigzagToInt(Varint.ReadU32(stream));
            }
            fieldDef.Encoding.SkipFieldValue(stream);
            return null;
        }

        private static string DecodeAsString(Stream stream)
        {
            return Varint.ReadString(stream);
        }

        private static double DecodeDecimal(long value, int scale)
        {
            var pow = Math.Pow(10, scale);
            return scale == 0 ? value : value / pow;
        }
    }
}