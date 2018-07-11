namespace BidFX.Public.API.Trade.Order
{
    public class Error
    {
        public string Field { get; private set; }
        public string Message { get; private set; }

        internal Error(string field, string message)
        {
            Field = field;
            Message = message;
        }

        public override string ToString()
        {
            return "Error: [" +
                   (Field == null ? "" : "Field=\"" + Field + "\",") +
                   (Message == null ? "" : "Message=\"" + Message + "\"") +
                   "]";
        }
    }
}