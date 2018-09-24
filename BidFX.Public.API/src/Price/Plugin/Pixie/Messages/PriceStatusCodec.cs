/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    internal class PriceStatusCodec
    {
        public static SubscriptionStatus DecodeStatus(int encoding)
        {
            switch (encoding)
            {
                case 'C':
                    return SubscriptionStatus.CANCELLED;
                case 'D':
                    return SubscriptionStatus.DISCONTINUED;
                case 'E':
                    return SubscriptionStatus.EXHAUSTED;
                case 'H':
                    return SubscriptionStatus.PROHIBITED;
                case 'I':
                    return SubscriptionStatus.INACTIVE;
                case 'L':
                    return SubscriptionStatus.CLOSED;
                case 'O':
                    return SubscriptionStatus.OK;
                case 'P':
                    return SubscriptionStatus.PENDING;
                case 'R':
                    return SubscriptionStatus.REJECTED;
                case 'S':
                    return SubscriptionStatus.STALE;
                case 'T':
                    return SubscriptionStatus.TIMEOUT;
                case 'U':
                    return SubscriptionStatus.UNAVAILABLE;
                default:
                    return SubscriptionStatus.PENDING;
            }
        }
    }
}