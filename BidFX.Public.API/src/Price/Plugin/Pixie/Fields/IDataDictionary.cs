using System.Collections.Generic;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    public interface IDataDictionary
    {
        FieldDef FieldDefByFid(int fid);

        FieldDef FieldDefByName(string fieldName);

        void AddFieldDef(FieldDef fieldDef);

        List<FieldDef> AllFieldDefs();

        int Size();

        int NextFreeFid();
    }
}