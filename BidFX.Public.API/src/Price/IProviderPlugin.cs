/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

namespace BidFX.Public.API.Price
{
    /// <summary>
    /// Interface for market data provider plugin components that can access and normalise sources of price data.
    /// </summary>
    internal interface IProviderPlugin : IProviderProperties, ISubscriber, IBackground
    {
        /// <summary>
        /// The event handler used for propagating events to the users of the NAPIClient API.
        /// </summary>
        IApiEventHandler InapiEventHandler { get; set; }

        /// <summary>
        /// Checks if a given subject is compatible with the provider plugin.
        /// </summary>
        /// <param name="subject">the subject to check</param>
        /// <returns>true if the subject is compatible and false otherwise</returns>
        bool IsSubjectCompatible(Subject.Subject subject);
    }
}