using System.Collections.Generic;

namespace TS.Pisa
{    
    public interface IPriceListener
    {
        void OnPriceUpdate(IPrice price, bool replaceAllFields);
        void OnPriceStatus(PriceStatus priceStatus);
    }
}
