namespace BidFX.Public.API.Trade.REST
{
    public interface IRESTCallback<out T> where T : AbstractRESTResponse
    {
        void Notfiy();

        bool HasResponse();

        T GetResponse();

        T GetResponse(long timeout);
    }
}