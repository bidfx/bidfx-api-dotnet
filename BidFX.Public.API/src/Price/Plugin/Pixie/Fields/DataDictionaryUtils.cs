using System.Collections.Generic;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    public class DataDictionaryUtils
    {
        public static bool IsValid(FieldDef fieldDef)
        {
            return fieldDef != null &&
                   fieldDef.Fid >= 0 &&
                   fieldDef.Type != null &&
                   fieldDef.Name != null &&
                   fieldDef.Encoding != null;
        }

        public static void AddAllFields(IDataDictionary dataDictionary, IEnumerable<FieldDef> fieldDefs)
        {
            foreach (var fieldDef in fieldDefs)
            {
                dataDictionary.AddFieldDef(fieldDef);
            }
        }
    }
}