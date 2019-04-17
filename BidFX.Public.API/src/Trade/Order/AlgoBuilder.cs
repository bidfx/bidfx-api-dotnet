using System.Collections.Generic;

namespace BidFX.Public.API.Trade.Order
{
    public class AlgoBuilder
    {
        private string _name;
        private readonly IDictionary<string, object> _parameters = new SortedDictionary<string, object>(); 
        
        public AlgoBuilder SetName(string name)
        {
            _name = name;
            return this;
        }

        public AlgoBuilder SetParameter(string parameter, object value)
        {
            if (value == null)
            {
                _parameters.Remove(parameter);
            }
            else
            {
                _parameters[parameter] = value;
            }

            return this;
        }
        
        public Algo Build()
        {
            return new Algo(_name, _parameters);
        }
    }
}