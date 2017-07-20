using BidFX.Public.API.Price.Tools;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    /// <summary>
    /// This interface provides a field definition that binds a field ID (FID) to a field name and type.
    /// It is used as an entry within a data dictionary.
    /// </summary>
    internal class FieldDef
    {
        public const int ReferecingFid = int.MaxValue;

        private int _fid = -1;
        private string _name;
        private FieldType _type;
        
        /// <summary>
        /// The encoding used to skip over the value part of an unrecognised field.
        /// </summary>
        public FieldEncoding Encoding { get; set; }
        
        /// <summary>
        /// The decimal scale factor to apply to the field when using double values with varint or zigzag encoding.
        /// </summary>
        public int Scale { get; set; }

        /// <summary>
        /// The field ID.
        /// </summary>
        public int Fid
        {
            get { return _fid; }
            set { _fid = Params.NotNegative(value); }
        }

        /// <summary>
        /// The type to decode the field value.
        /// </summary>
        public FieldType Type
        {
            get { return _type; }
            set { _type = Params.NotNull(value); }
        }

        /// <summary>
        /// The field name.
        /// </summary>
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
                   ", name=\"" + Name + "\", type=" + Type + ", encoding=" + Encoding + ", scale=" + Scale + ')';
        }
    }
}