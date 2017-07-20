using System.Collections.Generic;

namespace BidFX.Public.API.Price.Plugin.Pixie.Fields
{
    /// <summary>
    /// This interface provides a data dictionary that defines the fields that can be used within price updates and provides a mapping from FIDs to field names and their type.
    /// </summary>
    internal interface IDataDictionary
    {
        /// <summary>
        /// Looks up a field definition by its field ID.
        /// </summary>
        /// <param name="fid">the field ID to look up</param>
        /// <returns>the found field definition or null if it was not present</returns>
        FieldDef FieldDefByFid(int fid);

        /// <summary>
        /// Looks up a field definition by its field name.
        /// </summary>
        /// <param name="fieldName">the field name to look up</param>
        /// <returns>the found field definition or null if it was not present</returns>
        FieldDef FieldDefByName(string fieldName);

        /// <summary>
        /// Adds a field definition to the dictionary.
        /// </summary>
        /// <param name="fieldDef">the field definition to add</param>
        void AddFieldDef(FieldDef fieldDef);

        /// <summary>
        /// Gets a list of all field definitions.
        /// </summary>
        /// <returns>the list of all field definitions</returns>
        List<FieldDef> AllFieldDefs();

        /// <summary>
        /// Gets the size of the data dictionary in terms of its number of field definition entries.
        /// </summary>
        /// <returns>the size of the data dictionary</returns>
        int Size();

        /// <summary>
        /// Gets the next free FID that could be inserted into the data dictionary without clashing with an existing entry.
        /// </summary>
        /// <returns>the next free field ID</returns>
        int NextFreeFid();
    }
}