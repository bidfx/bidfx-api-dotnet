/// Copyright (c) 2018 BidFX Systems LTD. All Rights Reserved.

namespace BidFX.Public.API.Trade.REST
{
    internal interface IRESTCallback
    {
        void Notfiy();

        bool HasResponse();

        Order.Order GetResponse();

        Order.Order GetResponse(long timeout);
    }
}