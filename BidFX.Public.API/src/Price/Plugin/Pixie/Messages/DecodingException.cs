using System;
using System.Collections.Generic;
using System.IO;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    /// <summary>
    /// This class of exception is thrown when we have a protocol decoding error. It provides information that helps
    /// us diagnose the problem.
    /// </summary>
    public class DecodingException : Exception
    {
        public Stream MemoryStream { get; internal set; }
        public Dictionary<string, object> ResultSoFar { get; internal set; }

        public DecodingException(string msg) : base(msg)
        {
        }

        public DecodingException(string msg, Stream stream, Dictionary<string, object> resultSoFar) : base(msg)
        {
            MemoryStream = stream;
            ResultSoFar = resultSoFar;
        }

        public override string ToString()
        {
            return "DecodingException{" +
                   "\n  message = " + Message +
                   "\n  buffer = " + MemoryStream +
                   "\n  resultSoFar = " + ResultSoFar +
                   "\n}";
        }
    }
}