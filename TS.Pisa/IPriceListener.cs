using System.Collections.Generic;

namespace TS.Pisa
{    
    public interface IPriceListener
    {
        void OnPriceUpdate(IPriceField priceField, bool replaceAllFields);
        void OnPriceStatus(PriceStatus priceStatus);
    }
}
