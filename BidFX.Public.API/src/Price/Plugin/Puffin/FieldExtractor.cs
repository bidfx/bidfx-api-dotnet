using System;
using System.Reflection;
using log4net;

namespace BidFX.Public.API.Price.Plugin.Puffin
{
    internal class FieldExtractor
    {
#if DEBUG
private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#endif


        public static string Extract(string message, string fieldName)
        {
            string prefix = ' ' + fieldName + "=\"";
            int pos = message.IndexOf(prefix, StringComparison.Ordinal);
            if (pos != -1)
            {
                int start = pos + prefix.Length;
                int end = message.IndexOf('"', start);
                if (end != -1)
                {
                    string value = message.Substring(start, end - start);
                    if (Log.IsDebugEnabled)
                    {
                        Log.Debug("Value of " + fieldName + " is: \"" + value + '"');
                    }

                    return value;
                }
            }

            return null;
        }
    }
}