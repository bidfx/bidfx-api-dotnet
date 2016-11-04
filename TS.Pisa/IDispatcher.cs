
namespace TS.Pisa
{
    public interface IDispatcher
    {
        void AddPriceListener(IPriceListener priceListener);
        void RemovePriceListener(IPriceListener priceListener);
        void RemoveAllListeners();
    }
}