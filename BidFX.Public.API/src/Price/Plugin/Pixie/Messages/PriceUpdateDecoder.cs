/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.Collections.Generic;
using System.IO;
using BidFX.Public.API.Price.Plugin.Pixie.Fields;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    internal class PriceUpdateDecoder
    {
        public static void Visit(Stream stream, int size, IDataDictionary dataDictionary,
            IGridHeaderRegistry gridHeaderRegistry, ISyncable syncable)
        {
            for (int i = 0; i < size; i++)
            {
                char type = (char) stream.ReadByte();
                int sid = (int) Varint.ReadU32(stream);
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
            char status = (char) stream.ReadByte();
            string reason = DecodeAsString(stream);
            syncable.PriceStatus(sid, PriceStatusCodec.DecodeStatus(status), reason);
        }

        private static Dictionary<string, object> DecodePriceUpdate(Stream stream, IDataDictionary dataDictionary)
        {
            int size = (int) Varint.ReadU32(stream);
            Dictionary<string, object> priceMap = new Dictionary<string, object>(size);
            for (int i = 0; i < size; i++)
            {
                int fid = (int) Varint.ReadU32(stream);
                if (fid == FieldDef.ReferecingFid)
                {
                    uint referenceFid = Varint.ReadU32(stream);
                    FieldDef fieldDef = GetFieldDef(stream, dataDictionary, priceMap, (int) referenceFid);
                    priceMap[fieldDef.Name] = null;
                }
                else
                {
                    FieldDef fieldDef = GetFieldDef(stream, dataDictionary, priceMap, fid);
                    priceMap[fieldDef.Name] = DecodeField(stream, fieldDef);
                }
            }

            return priceMap;
        }

        private static void DecodeFullGridImage(int sid, Stream stream, IDataDictionary dataDictionary,
            IGridHeaderRegistry gridHeaderRegistry, ISyncable synable)
        {
            int columnCount = (int) Varint.ReadU32(stream);
            synable.StartGridImage(sid, columnCount);
            FieldDef[] headers = new FieldDef[columnCount];
            for (int i = 0; i < columnCount; i++)
            {
                int fid = (int) Varint.ReadU32(stream);
                if (fid == FieldDef.ReferecingFid)
                {
                    throw new IllegalStateException("unexpected field " + fid);
                }

                int rowCount = (int) Varint.ReadU32(stream);
                object[] column = new object[rowCount];
                FieldDef fieldDef = GetFieldDef(stream, dataDictionary, new Dictionary<string, object>(), fid);
                for (int j = 0; j < rowCount; j++)
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
            FieldDef[] header = gridHeaderRegistry.GetGridHeader(sid);

            int columnCount = (int) Varint.ReadU32(stream);
            syncable.StartGridUpdate(sid, columnCount);

            for (int i = 0; i < columnCount; i++)
            {
                char updateType = (char) stream.ReadByte();
                int cid = (int) Varint.ReadU32(stream);
                int rowCount = (int) Varint.ReadU32(stream);
                object[] column = new object[rowCount];

                FieldDef fieldDef = header[cid];
                for (int j = 0; j < rowCount; j++)
                {
                    column[j] = DecodeField(stream, fieldDef);
                }

                if (updateType == 'f')
                {
                    syncable.FullColumnUpdate(fieldDef.Name, cid, rowCount, column);
                }
                else
                {
                    int[] rids = new int[rowCount];

                    for (int j = 0; j < rowCount; j++)
                    {
                        rids[j] = (int) Varint.ReadU32(stream);
                    }

                    if (updateType == 't')
                    {
                        int deletedFrom = (int) Varint.ReadU32(stream);
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
            FieldDef fieldDef = dataDictionary.FieldDefByFid(fid);
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
                    return ((long) Varint.ReadU64(stream) * 1) ^ fieldDef.Scale;
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
                    return ((int) Varint.ReadU32(stream) * 1) ^ fieldDef.Scale;
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
            double pow = Math.Pow(10, scale);
            return scale == 0 ? value : value / pow;
        }
    }
}