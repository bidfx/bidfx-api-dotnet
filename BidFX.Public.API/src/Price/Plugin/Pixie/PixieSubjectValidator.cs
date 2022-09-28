using System.Reflection;
using System.Text.RegularExpressions;
using BidFX.Public.API.Price.Subject;
using BidFX.Public.API.Price.Tools;
using Serilog;
using Serilog.Core;

namespace BidFX.Public.API.Price.Plugin.Pixie
{
    internal static class PixieSubjectValidator
    {
        private static readonly ILogger Log = Serilog.Log.ForContext(Constants.SourceContextPropertyName, "PixieSubjectValidator");
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

                if (dealType.Equals(CommonComponents.Swap) || dealType.Equals(CommonComponents.NDS))
                {
                    if (!CheckFarFields(subject, inApiEventHandler))
                    {
                        return false;
                    }
                }
            }
            else
            {
                string tenor = subject.GetComponent(SubjectComponentName.Tenor);
                string settlementDate = subject.GetComponent(SubjectComponentName.SettlementDate);

                if ((settlementDate != null && !_settlementDateRegex.IsMatch(settlementDate.ToUpper())) ||
                    (tenor != null && !_tenorRegex.IsMatch(tenor.ToUpper())))
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
                Log.Information("Tenor and SettlementDate are both null");
                return false;
            }

            if (settlementDate != null && !_settlementDateRegex.IsMatch(settlementDate.ToUpper()))
            {
                inApiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "Invalid SettlementDate");
                Log.Information("Invalid SettlementDate {settlementDate}", settlementDate);
                return false;
            }

            if (tenor != null && settlementDate == null && !_tenorRegex.IsMatch(tenor.ToUpper()))
            {
                inApiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "Unsupported Tenor");
                Log.Information("Unsupported Tenor {tenor}", tenor);
                return false;
            }

            if (settlementDate == null && "BD".Equals(tenor.ToUpper()))
            {
                inApiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "SettlementDate is null for BD tenor");
                Log.Information("SettlementDate is null for BD tenor");
                return false;
            }

            return true;
        }
        
        private static bool CheckFarFields(Subject.Subject subject, IApiEventHandler inApiEventHandler)
        {
            string farTenor = subject.GetComponent(SubjectComponentName.FarTenor);
            string farSettlementDate = subject.GetComponent(SubjectComponentName.FarSettlementDate);
            if (farTenor == null && farSettlementDate == null)
            {
                inApiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "FarTenor and FarSettlementDate are both null");
                Log.Information("FarTenor and FarSettlementDate are both null");
                return false;
            }

            if (farSettlementDate != null && !_settlementDateRegex.IsMatch(farSettlementDate.ToUpper()))
            {
                inApiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "Invalid Far SettlementDate");
                Log.Information("Invalid FarSettlementDate {farSettlementDate}", farSettlementDate);
                return false;
            }

            if (farTenor != null && farSettlementDate == null && !_tenorRegex.IsMatch(farTenor.ToUpper()))
            {
                inApiEventHandler.OnSubscriptionStatus(subject, SubscriptionStatus.REJECTED, "Unsupported Far Tenor");
                Log.Information("Unsupported FarTenor {farTenor}", farTenor);
                return false;
            }

            return true;
        }
    }
}