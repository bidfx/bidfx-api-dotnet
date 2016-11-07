using System;

namespace TS.Pisa.Plugin.Puffin
{
    /// <summary>
    /// This class is a bean of client connections options.
    /// </summary>
    public class ConnectionOptions : ICloneable
    {
        /// <summary>
        /// Determines if subscription requests are compressed.
        /// True if configured to compress subscriptions.
        /// </summary>
        public bool ZipRequests { get; set; } = true;
        /// <summary>
        /// Determines if price updates are compressed by the server.
        /// True if the server is configured to compress price updates.
        /// </summary>
        public bool ZipPrices { get; set; } = true;
        /// <summary>
        /// Get the encrypt I/O flag.
        /// True if configured to encrypt all I/O communications.
        /// </summary>
        public bool Encrypt { get; set; } = true;
        /// <summary>
        /// Determine if an SSL session should be established with the server.
        /// True if configured to establish an SSL connection.
        /// </summary>
        public bool SSL { get; set; } = false;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}