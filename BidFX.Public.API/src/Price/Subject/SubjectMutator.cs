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

        private static readonly Dictionary<string, string> ComponentNameMap = new Dictionary<string, string>
        {
            {SubjectComponentName.BuySideAccount, Account},
            {SubjectComponentName.AllocBuySideAccount, AllocAccount},
            {SubjectComponentName.AllocQuantity, AllocQty},
            {SubjectComponentName.AllocEndQuantity, AllocEndQty},
            {SubjectComponentName.OnBehalfOf, AltUserName},
            {SubjectComponentName.LiquidityProvider, Source},
            {SubjectComponentName.DealType, SubClass},
            {SubjectComponentName.SettlementDate, ValueDate},
            {SubjectComponentName.SettlementDate2, ValueDate2}
        };

        private static readonly Dictionary<string, string> OldComponentNameMap = ComponentNameMap.Reverse();

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
            if (!buySideAccount.Equals("") && level == 1 && !source.Equals(""))
            {
                subjectBuilder.SetComponent("Account", SourceToSellSideAccountMap[source]);
            }
            else if (!buySideAccount.Equals("") && level == 2)
            {
                subjectBuilder.SetComponent("Accout", buySideAccount);
            }
            subjectBuilder.SetComponent("Customer", "0001");
            return subjectBuilder.CreateSubject();
        }

        public static Subject ToNewVersion(Subject subject)
        {
            var subjectBuilder = new SubjectBuilder {AutoRefresh = subject.AutoRefresh};
            foreach (var component in subject)
            {
                if(component.Key.Equals("Customer")) continue;
                if (OldComponentNameMap.ContainsKey(component.Key))
                {
                    subjectBuilder.SetComponent(OldComponentNameMap[component.Key], component.Value);
                }
                else
                {
                    subjectBuilder.SetComponent(component.Key, component.Value);
                }
            }
            subjectBuilder.SetComponent(SubjectComponentName.BuySideAccount, "FX_ACCT");
            return subjectBuilder.CreateSubject();
        }
        

        public static Dictionary<TValue, TKey> Reverse<TKey, TValue>(this Dictionary<TKey, TValue> toReverse)
        {
            var dictionary = new Dictionary<TValue, TKey>();
            foreach (var entry in toReverse)
            {
                dictionary[entry.Value] = entry.Key;
            }
            return dictionary;
        }
    }
}