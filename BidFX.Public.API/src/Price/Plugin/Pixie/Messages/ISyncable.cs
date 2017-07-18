using System.Collections.Generic;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    /// <summary>
    /// Interface to sync the state of prices between peer components by sending updates.
    /// </summary>
    public interface ISyncable
    {
        /// <summary>
        /// Encodes a full price image for a single subscription.
        /// </summary>
        /// <param name="sid">the subject ID (index within a subject set edition)</param>
        /// <param name="price">the price fields</param>
        void PriceImage(int sid, Dictionary<string, object> price);

        /// <summary>
        /// Encodes a delta price update for a single subscription.
        /// </summary>
        /// <param name="sid">the subject ID (index within a subject set edition)</param>
        /// <param name="price">the price fields</param>
        void PriceUpdate(int sid, Dictionary<string, object> price);

        /// <summary>
        /// Encodes a price status update for a single subscription.
        /// </summary>
        /// <param name="sid">the subject ID (index within a subject set edition)</param>
        /// <param name="status">the Pisa status code</param>
        /// <param name="explanation">a more detailed explanation of the problem</param>
        void PriceStatus(int sid, SubscriptionStatus status, string explanation);

        /// <summary>
        /// Invoked at the beginning of a Grid Image. It will be followed by an invocation of <see cref="ColumnImage"/>
        /// for each of the columns in the grid.
        /// </summary>
        /// <param name="sid">the subject ID (index within a subject set edition)</param>
        /// <param name="columnCount">the number of columns in the grid</param>
        void StartGridImage(int sid, int columnCount);

        /// <summary>
        /// The complete set of values for a specific column in a grid image.
        /// Invoked in order: each column is explicitely assigned an increasing column id (CID), stating from 0.
        /// </summary>
        /// <param name="name">the name of the column</param>
        /// <param name="rowCount">the number of rows in the column</param>
        /// <param name="columnValues">the array containing the values. It has at least <see cref="rowCount"/> elements</param>
        void ColumnImage(string name, int rowCount, object[] columnValues);

        /// <summary>
        /// Invoked when all the column images have been provided.
        /// </summary>
        void EndGridImage();

        void StartGridUpdate(int sid, int columnCount);

        void FullColumnUpdate(string name, int cid, int rowCount, object[] columnValues);

        void PartialColumnUpdate(string name, int cid, int rowCount, object[] columnValues, int[] rids);

        void PartialTruncatedColumnUpdate(string name, int cid, int rowCount, object[] columnValues, int[] rids,
            int truncatedRid);

        void EndGridUpdate();
    }
}