using System.Collections.Generic;

namespace TS.Pisa
{
    public interface IPrice
    {
        string GetSubject();
        IReadOnlyDictionary<string, object> GetAllFields();
        IReadOnlyCollection<string> GetChangedFieldNames();
    }        
}
