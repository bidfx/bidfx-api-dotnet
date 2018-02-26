/// Copyright (c) 2018 BidFX Systems LTD. All Rights Reserved.

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