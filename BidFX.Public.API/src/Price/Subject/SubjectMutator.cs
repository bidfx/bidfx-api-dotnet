using System;
using System.Collections.Generic;
using System.Linq;

namespace BidFX.Public.API.Price.Subject
{
    /// <summary>
    /// Takes a new FX Subject and converts it into an old FX Subject.
    /// Temporary class until Highway is updated to accept new subjects.
    /// </summary>
    public static class SubjectMutator
    {
        private const string Account = "Account";
        private const string AllocAccount = "AllocAccount";
        private const string AllocQty = "AllocQty";
        private const string AllocEndQty = "AllocEndty";
        private const string AltUserName = "AltUserName";
        private const string Source = "Source";
        private const string SubClass = "SubClass";
        private const string ValueDate = "ValueDate";
        private const string ValueDate2 = "ValueDate2";
        private const string FixingDate2 = "FixingDate2";
        private const string Quantity2 = "Quantity2";
        private const string QuoteStyle = "QuoteStyle";
        private const string Tenor2 = "Tenor2";
        private const string Currency2 = "Currency2";
        private const string Customer = "Customer";

        private static readonly Dictionary<string, string> ComponentNameMap = new Dictionary<string, string>
        {
            {SubjectComponentName.BuySideAccount, Account},
            {SubjectComponentName.BuySideAllocAccount, AllocAccount},
            {SubjectComponentName.AllocQuantity, AllocQty},
            {SubjectComponentName.FarAllocQuantity, AllocEndQty},
            {SubjectComponentName.OnBehalfOf, AltUserName},
            {SubjectComponentName.LiquidityProvider, Source},
            {SubjectComponentName.DealType, SubClass},
            {SubjectComponentName.SettlementDate, ValueDate},
            {SubjectComponentName.FarSettlementDate, ValueDate2},
            {SubjectComponentName.RequestFor, QuoteStyle},
            {SubjectComponentName.FarFixingDate, FixingDate2},
            {SubjectComponentName.FarQuantity, Quantity2},
            {SubjectComponentName.FarTenor, Tenor2},
            {SubjectComponentName.FarCurrency, Currency2},
            {SubjectComponentName.BuySideId, Customer},
        };

        private static readonly Dictionary<string, string> SourceToSellSideAccountMap = new Dictionary<string, string>
        {
            {"BNPFX", "TSCREENTEST"},
            {"SSFX", "TRADINGSCREEN"},
            {"RBCFX", "TRADINGSCREEN"},
            {"MSFX", "TEST1"},
            {"CSFX", "CSFX_TS"},
            {"JPMCFX", "TESTACCOUNT"},
            {"HSBCFX", "TRDSCR"},
            {"RBSFX", "251539"},
            {"UBSFX", "TRADINGSCREEN_LRGE"},
            {"NOMURAFX", "TradingScreen"},
            {"CITIFX", "TSCREEN"},
            {"COBAFX", "ATTC1"},
        };

        public static Subject ToOldVersion(Subject subject)
        {
            var level = 0;
            var buySideAccount = "";
            var source = "";
            var subjectBuilder = new SubjectBuilder {AutoRefresh = subject.AutoRefresh};
            foreach (var component in subject)
            {
                if (component.Key.Equals("Level"))
                {
                    level = int.Parse(component.Value);
                }
                if (component.Key.Equals("LiquidityProvider"))
                {
                    source = component.Value;
                }
                if (ComponentNameMap.ContainsKey(component.Key))
                {
                    if (component.Key.Equals("BuySideAccount"))
                    {
                        buySideAccount = component.Value;
                    }
                    else
                    {
                        subjectBuilder.SetComponent(ComponentNameMap[component.Key], component.Value);
                    }
                }
                else
                {
                    subjectBuilder.SetComponent(component.Key, component.Value);
                }
            }
            if ("Swap".Equals(subject.LookupValue(SubjectComponentName.DealType)) ||
                "NDS".Equals(subject.LookupValue(SubjectComponentName.DealType)))
            {
                subjectBuilder.SetComponent("LegCount", "2");
            }
            if (subject.LookupValue(SubjectComponentName.User) == null)
            {
                subjectBuilder.SetComponent(SubjectComponentName.User, PriceManager.Username);
            }
            if (!buySideAccount.Equals("") && level == 1 && !source.Equals(""))
            {
                subjectBuilder.SetComponent("Account", SourceToSellSideAccountMap[source]);
            }
            else if (!buySideAccount.Equals("") && level == 2)
            {
                subjectBuilder.SetComponent("Account", buySideAccount);
            }
            subjectBuilder.SetComponent("Customer", "0001");
            subjectBuilder.SetComponent("Exchange", "OTC");
            return subjectBuilder.CreateSubject();
        }
    }
}