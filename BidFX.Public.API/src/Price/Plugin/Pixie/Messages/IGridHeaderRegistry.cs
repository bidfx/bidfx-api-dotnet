using BidFX.Public.API.Price.Plugin.Pixie.Fields;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public interface IGridHeaderRegistry
    {
        void SetGridHeader(int sid, FieldDef[] headerDefs);

        FieldDef[] GetGridHeader(int sid);
    }
}