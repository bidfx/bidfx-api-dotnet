using BidFX.Public.API.Price.Plugin.Pixie.Fields.FieldEncodings;
using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    public class FieldDef
    {
        public const int ReferecingFid = int.MaxValue;

        private int _fid = -1;
        private string _name;
        private FieldType _type;
        public FieldEncoding Encoding { get; set; }
        public int Scale { get; set; }

        public int Fid
        {
            get { return _fid; }
            set { _fid = Params.NotNegative(value); }
        }

        public FieldType Type
        {
            get { return _type; }
            set { _type = Params.NotNull(value); }
        }

        public string Name
        {
            get { return _name; }
            set { _name = Params.NotBlank(value); }
        }

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
                hashCode = (hashCode * 397) ^ Encoding.GetHashCode();
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