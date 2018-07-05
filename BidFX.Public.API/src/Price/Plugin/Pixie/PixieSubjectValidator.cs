﻿using System.Reflection;
using System.Text.RegularExpressions;
using BidFX.Public.API.Price.Subject;
using log4net;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    internal static class PixieSubjectValidator
    {
        private static readonly ILog Log =
            LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        private static readonly Regex _settlementDateRegex = new Regex(@"^2[0-9][0-9][0-9](0[1-9]|1[0-2])(0[1-9]|[1-2][0-9]|3[0-1])$");
        private static readonly Regex _tenorRegex = new Regex(@"^((BD|SPOT|[STO]/N)|[1-4]W|([1-9]|[1-2][0-9]|3[0-6])M|([1-9]|1[0-9]|20)Y|IMM[HMUZ]|BMF[UVXZFGHJKMNQ])$");
        
        public static bool ValidateSubject(Subject.Subject subject, IApiEventHandler inApiEventHandler)
        {
            string dealType = subject.GetComponent(SubjectComponentName.DealType);
            if (dealType == null)
            {
                inApiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "DealType is null");
                return false;
            }

            if (dealType.Equals(CommonComponents.Forward) || dealType.Equals(CommonComponents.NDF) ||
                dealType.Equals(CommonComponents.Swap) || dealType.Equals(CommonComponents.NDS))
            {
                if (!CheckForwardFields(subject, inApiEventHandler))
                {
                    return false;
                }
            }

            if (dealType.Equals(CommonComponents.Swap) || dealType.Equals(CommonComponents.NDS))
            {
                if (!CheckNDFFields(subject, inApiEventHandler))
                {
                    return false;
                }
            }

            return true;
        }

        private static bool CheckForwardFields(Subject.Subject subject, IApiEventHandler inApiEventHandler)
        {
            string tenor = subject.GetComponent(SubjectComponentName.Tenor);
            string settlementDate = subject.GetComponent(SubjectComponentName.SettlementDate);
            if (tenor == null && settlementDate == null)
            {
                inApiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "Tenor and SettlementDate are both null");
                Log.InfoFormat("Tenor and SettlementDate are both null");
                return false;
            }

            if (settlementDate != null && !_settlementDateRegex.IsMatch(settlementDate.ToUpper()))
            {
                inApiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "Invalid SettlementDate");
                Log.InfoFormat("Invalid SettlementDate {0}", settlementDate);
                return false;
            }

            if (tenor != null && settlementDate == null && !_tenorRegex.IsMatch(tenor.ToUpper()))
            {
                inApiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "Unsupported Tenor");
                Log.InfoFormat("Unsupported Tenor {0}", tenor);
                return false;
            }

            if (settlementDate == null && "BD".Equals(tenor.ToUpper()))
            {
                inApiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "SettlementDate is null for BD tenor");
                Log.InfoFormat("SettlementDate is null for BD tenor");
                return false;
            }

            return true;
        }
        
        private static bool CheckNDFFields(Subject.Subject subject, IApiEventHandler inApiEventHandler)
        {
            string farTenor = subject.GetComponent(SubjectComponentName.FarTenor);
            string farSettlementDate = subject.GetComponent(SubjectComponentName.FarSettlementDate);
            if (farTenor == null && farSettlementDate == null)
            {
                inApiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "FarTenor and FarSettlementDate are both null");
                Log.InfoFormat("FarTenor and FarSettlementDate are both null");
                return false;
            }

            if (farSettlementDate != null && !_settlementDateRegex.IsMatch(farSettlementDate.ToUpper()))
            {
                inApiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "Invalid Far SettlementDate");
                Log.InfoFormat("Invalid FarSettlementDate {0}", farSettlementDate);
                return false;
            }

            if (farTenor != null && farSettlementDate == null && !_tenorRegex.IsMatch(farTenor.ToUpper()))
            {
                inApiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "Unsupported Far Tenor");
                Log.InfoFormat("Unsupported FarTenor {0}", farTenor);
                return false;
            }

            return true;
        }
    }
}