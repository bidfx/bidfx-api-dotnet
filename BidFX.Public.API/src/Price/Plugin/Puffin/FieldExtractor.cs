﻿using System;
using System.Reflection;
using Serilog;

namespace BidFX.Public.API.Price.Plugin.Puffin
{
    internal class FieldExtractor
    {
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
                    Log.Debug("Value of {fieldName} is: {value}", fieldName, value);

                    return value;
                }
            }

            return null;
        }
    }
}