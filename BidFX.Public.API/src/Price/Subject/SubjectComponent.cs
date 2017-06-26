using System.Text;

namespace BidFX.Public.API.Price.Subject
{
    public class SubjectComponent
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public SubjectComponent()
        {
        }

        public SubjectComponent(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public override string ToString()
        {
            return new StringBuilder().Append(Key).Append('=').Append(Value).ToString();
        }

        protected bool Equals(SubjectComponent other)
        {
            return string.Equals(Key, other.Key) && string.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SubjectComponent) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? Key.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
            }
        }
    }
}