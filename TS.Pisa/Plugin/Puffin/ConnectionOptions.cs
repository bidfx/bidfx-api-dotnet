using System;

namespace TS.Pisa.Plugin.Puffin
{
    /// <summary>
    /// This class is a bean of client connections options.
    /// </summary>
    public class ConnectionOptions : ICloneable
    {
        private bool _ZipRequests = true;
        private bool _ZipPrices = true;
        private bool _Encrypt = true;
        private bool _SSL = false;

        /// <summary>
        /// Determines if subscription requests are compressed.
        /// </summary>
        /// <returns>True if configured to compress subscriptions.</returns>
        public bool IsZipRequests()
        {
            return _ZipRequests;
        }

        public void SetZipRequests(bool ZipRequests)
        {
            _ZipRequests = ZipRequests;
        }

        /// <summary>
        /// Determines if price updates should be compressed by the server.
        /// </summary>
        /// <returns>True if price updates are required to be compressed by the server.</returns>
        public bool IsZipPrices()
        {
            return _ZipPrices;
        }

        public void SetZipPrices(bool ZipPrices)
        {
            _ZipPrices = ZipPrices;
        }

        /// <summary>
        /// Get the encrypt I/O flag.
        /// </summary>
        /// <returns>True if configured to encrypt all I/O communications.</returns>
        public bool IsEncrypt()
        {
            return _Encrypt;
        }

        public void SetEncrypt(bool Encrypt)
        {
            _Encrypt = Encrypt;
        }

        /// <summary>
        /// Determine if an SSL session should be established with the server.
        /// </summary>
        /// <returns>True if configured to establish SSL session.</returns>
        public bool IsSSL()
        {
            return _SSL;
        }

        public void SetSSL(bool SSL)
        {
            _SSL = SSL;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}