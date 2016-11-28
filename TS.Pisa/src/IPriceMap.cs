using System.Collections.Generic;

namespace TS.Pisa
{
    public interface IPriceMap
    {
        IEnumerable<string> FieldNames { get; }
        IEnumerable<KeyValuePair<string, IPriceField>> PriceFields { get; }
        IPriceField Field(string name);

        decimal? DecimalField(string name);
        long? LongField(string name);
        int? IntField(string name);
        string StringField(string name);
    }
}