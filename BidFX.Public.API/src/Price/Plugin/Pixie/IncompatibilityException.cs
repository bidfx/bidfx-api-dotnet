using System;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    internal class IncompatibilityException : Exception
    {
        public IncompatibilityException(int version, string message) : base(
            "version " + version + "of the Pixie protocol is incompatible with " + message)
        {
        }
    }
}