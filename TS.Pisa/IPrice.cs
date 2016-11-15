using System.Collections.Generic;

namespace TS.Pisa
{
    public interface IPrice
    {
        string GetSubject();
        IDictionary<string, object> GetAllFields();
        ICollection<string> GetChangedFieldNames();
    }        
}
