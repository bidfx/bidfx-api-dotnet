using System;
using System.Collections.Generic;

namespace BidFX.Public.API.Trade
{
    public abstract class OrderResponse : EventArgs
    {
        public abstract long GetMessageId();
        
        /// <summary>
        /// Get the unique TSId assigned to the order. This can be used to query the state of an order.
        /// </summary>
        /// <returns>The TSId assigned to the order, or null if no TSId was assigned.</returns>
        public abstract string GetOrderId();
        
        /// <summary>
        /// Get the last known state of the order.
        /// </summary>
        /// <returns>The state assigned to the order, or null if no state was assigned.</returns>
        public abstract string GetState();
        
        /// <summary>
        /// Get the errors attached to the order, if there is one.
        /// </summary>
        /// <returns>A list of errors given to the order. The list is empty if there were no errors.</returns>
        public abstract List<string> GetErrors();

        public abstract string GetField(string fieldName);
    }
}