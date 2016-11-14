using System;
namespace TS.Pisa.Tools
{
    internal class TunnelException : Exception
    {
        public TunnelException(string msg) :
            base(msg)
        {
        }
    }
}
