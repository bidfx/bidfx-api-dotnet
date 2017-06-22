using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using BidFX.Public.API.Price.Plugin.Pixie.Fields;
using BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public class DataDictionaryMessage
    {
        private readonly uint _options;
        private readonly List<FieldDef> _fieldDefs;
        
        public DataDictionaryMessage(Stream stream)
        {
            _options = Varint.ReadU32(stream);
            var size = (int) Varint.ReadU32(stream);
            _fieldDefs = new List<FieldDef>(size);
            for (var i = 0; i < size; i++)
            {
                _fieldDefs.Add(ReadFieldDef(stream));
            }
        }

        private static FieldDef ReadFieldDef(Stream stream)
        {
            return new FieldDef()
            {
                Fid = (int) Varint.ReadU32(stream),
                Type = stream.ReadByte(),
                Encoding = FieldEncoding.ValueOf(stream.ReadByte()),
                Scale = (int) Varint.ReadU32(stream),
                Name = Varint.ReadString(stream)
            };
        }

        public bool IsUpdate()
        {
            return BitSetter.IsSet(_options, 1);
        }

        public override string ToString()
        {
            var join = string.Join("\n ", _fieldDefs.Select(x => x.ToString()).ToArray());
            return new StringBuilder()
                .Append("DataDictionaryMessage(" + "update=" + IsUpdate() + ", fields=[\n  ")
                .Append(join)
                .Append("\n])").ToString();
        }

        public List<FieldDef> FieldDefs
        {
            get { return _fieldDefs; }
        }
    }
}