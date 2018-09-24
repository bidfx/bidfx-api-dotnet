/// Copyright (c) 2018 BidFX Systems LTD. All Rights Reserved.

namespace BidFX.Public.API
{
    /// <summary>
    /// This class provides a holder for the default Client.
    /// Most applications will share a single Client session and get access to it from here.
    /// </summary>
    public class DefaultClient
    {
        private static readonly Client _client = new Client();

        /// <summary>
        /// The default Client instance.
        /// </summary>
        public static Client Client
        {
            get { return _client; }
        }
    }
}