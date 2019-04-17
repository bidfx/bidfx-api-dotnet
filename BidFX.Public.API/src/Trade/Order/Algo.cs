using System.Collections.Generic;
using BidFX.Public.API.Trade.Rest.Json;

namespace BidFX.Public.API.Trade.Order
{
    public class Algo : IJsonMarshallable
    {
        internal const string Name = "name";
        internal const string Parameters = "parameters";
        
        private readonly IDictionary<string, object> _parameters = new SortedDictionary<string, object>();
        private readonly string _name;


        internal Algo(string name, IDictionary<string, object> parameters)
        {
            _name = name;
            _parameters = parameters;
        }

        public string GetName()
        {
            return _name;
        }

        public object GetParameter(string parameter)
        {
            object value;
            return _parameters.TryGetValue(parameter, out value) ? value : null;
        }
        
        public IDictionary<string, object> GetJsonMap()
        {
            IDictionary<string, object> dictionary = new SortedDictionary<string, object>();
            dictionary.Add("name", _name);
            dictionary.Add("parameters", new SortedDictionary<string, object>(_parameters));
            return dictionary;
        }
    }
}