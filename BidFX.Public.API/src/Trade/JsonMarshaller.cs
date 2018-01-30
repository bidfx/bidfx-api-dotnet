using System.Text;
using BidFX.Public.API.Trade.Order;

namespace BidFX.Public.API.Trade
{
    public class JsonMarshaller
    {
        public static string ToJSON(FxOrder order)
        {
            var stringBuilder = new StringBuilder();
            var components = order.getInternalComponents();
            stringBuilder.Append("{");
            for (var i = 0; i < components.Length;)
            {
                var key = components[i++];
                var value = components[i++];
                stringBuilder.Append("\"");
                stringBuilder.Append(key);
                stringBuilder.Append("\":");
                switch (key)
                {
                    case FxOrder.Quantity:
                    case FxOrder.Price:
                    case FxOrder.FarQuantity:
                        //Decimals -> no quoting the value
                        stringBuilder.Append(value);
                        break;
                    default:
                        //String literals for values
                        stringBuilder.Append("\"");
                        stringBuilder.Append(value);
                        stringBuilder.Append("\"");
                        break;
                }

                if (i < components.Length)
                {
                    stringBuilder.Append(",");
                }
            }
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }
    }
}