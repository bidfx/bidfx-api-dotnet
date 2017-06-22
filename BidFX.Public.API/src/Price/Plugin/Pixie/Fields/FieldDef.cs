using System;
using BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    public class FieldDef
    {
        public const int ReferecingFid = int.MaxValue;

        public int Fid { get; set; }
        public string Name { get; set; }
        public int Type { get; set; }
        public FieldEncoding Encoding { get; set; }
        public int Scale { get; set; }

        protected bool Equals(FieldDef other)
        {
            return Fid == other.Fid && string.Equals(Name, other.Name) && Equals(Encoding, other.Encoding) &&
                   Scale == other.Scale;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((FieldDef) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Fid;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Encoding != null ? Encoding.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Scale;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return "fieldDef(FID=" + Fid +
                   ", name=\"" + Name + "\", encoding=" + Encoding + ", scale=" + Scale + ')';
        }
    }
}