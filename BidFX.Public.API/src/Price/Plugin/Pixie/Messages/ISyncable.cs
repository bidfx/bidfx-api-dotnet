using System.Collections.Generic;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public interface ISyncable
    {
        void PriceImage(int sid, Dictionary<string, object> price);

        void PriceUpdate(int sid, Dictionary<string, object> price);

        void PriceStatus(int sid, SubscriptionStatus status, string explanation);

        void StartGridImage(int sid, int columnCount);

        void ColumnImage(string name, int rowCount, object[] columnValues);

        void EndGridImage();

        void StartGridUpdate(int sid, int columnCount);

        void FullColumnUpdate(string name, int cid, int rowCount, object[] columnValues);

        void PartialColumnUpdate(string name, int cid, int rowCount, object[] columnValues, int[] rids);

        void PartialTruncatedColumnUpdate(string name, int cid, int rowCount, object[] columnValues, int[] rids,
            int truncatedRid);

        void EndGridUpdate();
    }
}