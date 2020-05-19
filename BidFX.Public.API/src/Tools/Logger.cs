using System;
using Serilog;

namespace BidFX.Public.API.Price.Tools
{
    public static class Logger
    {
        public static ILogger ForContext<TSource>()
        {
            return ForContext(typeof(TSource));
        }

        public static ILogger ForContext(Type type)
        {
            return Serilog.Log.ForContext("SourceContext", type.Name);
        }
    }
}