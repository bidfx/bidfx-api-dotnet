using System;
using System.Text;
using BidFX.Public.API.Trade.Order;

namespace BidFX.Public.API.Trade
{
    public static class JsonMarshaller
    {
        public static string ToJSON(FxOrder order, long messageId)
        {
            StringBuilder stringBuilder = new StringBuilder(255);
            string[] components = order.GetInternalComponents();
            stringBuilder.Append("[{");
            for (int i = 0; i < components.Length;)
            {
                string key = components[i++];
                string value = components[i++];
                stringBuilder.Append("\"");
                stringBuilder.Append(key);
                stringBuilder.Append("\":");

                AppendValue(stringBuilder, key, value);
                stringBuilder.Append(",");
            }
            stringBuilder.Append("\"correlation_id\":\"");
            stringBuilder.Append(messageId);
            stringBuilder.Append("\"}]");
            return stringBuilder.ToString();
        }

        private static void AppendValue(StringBuilder stringBuilder, string key, string value)
        {
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
        }
    }
}