using System;
using System.Collections.Generic;
using BidFX.Public.API.Trade.Order;

namespace BidFX.Public.API.Trade
{
    public class SettlementDateResponse : EventArgs
    {
        private readonly long _messageId;
        private readonly List<Error> _errors;
        private readonly string _settlementDate;
        private readonly string _fixingDate;
        private readonly string _farSettlementDate;
        private readonly string _farFixingDate;
        
        private SettlementDateResponse(long messageId, List<Error> errors)
        {
            _messageId = messageId;
            _errors = errors;
        }

        private SettlementDateResponse(long messageId, IDictionary<string, object> map)
        {
            _messageId = messageId;
            object temp;
            _settlementDate = map.TryGetValue("settlement_date", out temp) ? (string) temp : null;
            _fixingDate = map.TryGetValue("fixing_date", out temp) ? (string) temp : null;
            _farSettlementDate = map.TryGetValue("far_settlement_date", out temp) ? (string) temp : null;
            _farFixingDate = map.TryGetValue("far_fixing_date", out temp) ? (string) temp : null;
        }

        public long GetMessageId()
        {
            return _messageId;
        }
        
        public List<Error> GetErrors()
        {
            return _errors;
        }

        public string GetSettlementDate()
        {
            return _settlementDate;
        }

        public string GetFixingDate()
        {
            return _fixingDate;
        }

        public string GetFarSettlementDate()
        {
            return _farSettlementDate;
        }

        public string GetFarFixingDate()
        {
            return _farFixingDate;
        }
        
        internal static SettlementDateResponse FromJson(long messageId, object jsonObject)
        {
            if (!(jsonObject is Dictionary<string, object>))
            {
                throw new ArgumentException("object was not a dictionary");
            }
            return new SettlementDateResponse(messageId, (Dictionary<string, object>) jsonObject);
        }

        internal static SettlementDateResponse FromError(long messageId, object jsonObject)
        {
            if (!(jsonObject is Dictionary<string, object>))
            {
                throw new ArgumentException("object was not a dictionary");
            }
            object paramsObject;
            if (!((Dictionary<string, object>) jsonObject).TryGetValue("params", out paramsObject))
            {
                throw new ArgumentException("could not get params part of message");
            }
            if (!(paramsObject is Dictionary<string, object>))
            {
                throw new ArgumentException("params object was not a dictionary");
            }

            object errorsListObject;
            if (!((Dictionary<string, object>) paramsObject).TryGetValue("errors", out errorsListObject))
            {
                throw new ArgumentException("could not get errors part of message");
            }
            if (!(errorsListObject is List<object>))
            {
                throw new ArgumentException("errors object was not a list");
            }
            
            List<Error> errors = new List<Error>();
            foreach (object errorObject in (List<object>) errorsListObject)
            {
                errors.Add(Error.FromJson(errorObject));
            }
            
            return new SettlementDateResponse(messageId, errors);
        }
    }
}