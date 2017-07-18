using BidFX.Public.API.Price.Plugin.Pixie.Fields;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    /// <summary>
    /// Register of column headers (field definitions) accessed using a sid of a specific subject set edition.
    /// It is required by the Pixie decoder to be able to correlate the CID (column id) with a specific data
    /// dictionary entry, in order to know how to parse received values.
    /// </summary>
    public interface IGridHeaderRegistry
    {
        void SetGridHeader(int sid, FieldDef[] headerDefs);

        FieldDef[] GetGridHeader(int sid);
    }
}