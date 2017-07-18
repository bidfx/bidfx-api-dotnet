using System.Collections.Generic;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    /// <summary>
    /// This class provides utilities for dealing with data dictionaries.
    /// </summary>
    public class DataDictionaryUtils
    {
        /// <summary>
        /// Checks if a field definition is valid with all of its parts present.
        /// </summary>
        /// <param name="fieldDef">the field definition to validate</param>
        /// <returns>true if valid and false otherwise</returns>
        public static bool IsValid(FieldDef fieldDef)
        {
            return fieldDef != null &&
                   fieldDef.Fid >= 0 &&
                   fieldDef.Type != 0 &&
                   fieldDef.Name != null &&
                   fieldDef.Encoding != null;
        }

        /// <summary>
        /// Add a set of field definitions to a data dictionary.
        /// </summary>
        /// <param name="dataDictionary">the dictionary to add to</param>
        /// <param name="fieldDefs">the list of new field definitions to add</param>
        public static void AddAllFields(IDataDictionary dataDictionary, IEnumerable<FieldDef> fieldDefs)
        {
            foreach (var fieldDef in fieldDefs)
            {
                dataDictionary.AddFieldDef(fieldDef);
            }
        }
    }
}