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

                if (settlementDate != null && !_settlementDateRegex.IsMatch(settlementDate.ToUpper()))
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

            return true;
        }
    }
}