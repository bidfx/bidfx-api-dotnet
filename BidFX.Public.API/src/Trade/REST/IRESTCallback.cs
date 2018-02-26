namespace BidFX.Public.API.Trade.REST
{
    internal interface IRESTCallback<out T> where T : AbstractRESTResponse
    {
        void Notfiy();

        bool HasResponse();

        T GetResponse();

        T GetResponse(long timeout);
    }
}