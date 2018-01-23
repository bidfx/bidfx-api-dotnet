using System;
using System.Reflection;
using log4net;

namespace BidFX.Public.API.Price.Plugin.Puffin
{
    internal class FieldExtractor
    {
        #if DEBUG
private static readonly ILog Log = DevLog.CreateLogger(MethodBase.GetCurrentMethod().DeclaringType);
#else
private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
#endif


        public static string Extract(string message, string fieldName)
        {
            var prefix = ' ' + fieldName + "=\"";
            var pos = message.IndexOf(prefix, StringComparison.Ordinal);
            if (pos != -1)
            {
                var start = pos + prefix.Length;
                var end = message.IndexOf('"', start);
                if (end != -1)
                {
                    var value = message.Substring(start, end - start);
                    if (Log.IsDebugEnabled) Log.Debug("Value of " + fieldName + " is: \"" + value + '"');
                    return value;
                }
            }
            return null;
        }
    }
}