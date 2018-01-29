using System;

namespace BidFX.Public.API.Trade.REST
{
    public abstract class AbstractRESTResponse : EventArgs
    {
        public void LoadFromJSON(string json)
        {
            //TODO
        }
        
        protected object GetField(string fieldname)
        {
            //TODO
            return null;
        }
    }
}