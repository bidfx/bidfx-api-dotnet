using System;
using System.Collections.Generic;
using System.Linq;

namespace BidFX.Public.API.Enums
{
    internal class FxTenor
    {
        private static readonly FxTenor TODAY = new FxTenor("TOD", "O/N", "TOD");
        private static readonly FxTenor TOMORROW_NIGHT = new FxTenor("TOM", "T/N", "TOM");
        private static readonly FxTenor SPOT = new FxTenor("SPOT", "SPOT", "SPOT");
        private static readonly FxTenor SPOT_NEXT = new FxTenor("SPOT_NEXT", "S/N", "SPOT_NEXT");
        private static readonly FxTenor W1 = new FxTenor("1W", "1W", "1W");
        private static readonly FxTenor W2 = new FxTenor("2W", "2W", "2W");
        private static readonly FxTenor W3 = new FxTenor("3W", "3W", "3W");
        private static readonly FxTenor M1 = new FxTenor("1M", "1M", "1M");
        private static readonly FxTenor M2 = new FxTenor("2M", "2M", "2M");
        private static readonly FxTenor M3 = new FxTenor("3M", "3M", "3M");
        private static readonly FxTenor M4 = new FxTenor("4M", "4M", "4M");
        private static readonly FxTenor M5 = new FxTenor("5M", "5M", "5M");
        private static readonly FxTenor M6 = new FxTenor("6M", "6M", "6M");
        private static readonly FxTenor M7 = new FxTenor("7M", "7M", "7M");
        private static readonly FxTenor M8 = new FxTenor("8M", "8M", "8M");
        private static readonly FxTenor M9 = new FxTenor("9M", "9M", "9M");
        private static readonly FxTenor M10 = new FxTenor("10M", "10M", "10M");
        private static readonly FxTenor M11 = new FxTenor("11M", "11M", "11M");
        private static readonly FxTenor Y1 = new FxTenor("1Y", "1Y", "1Y");
        private static readonly FxTenor Y2 = new FxTenor("2Y", "2Y", "2Y");
        private static readonly FxTenor Y3 = new FxTenor("3Y", "3Y", "3Y");
        private static readonly FxTenor IMMH = new FxTenor("IMMH", "IMMH", "IMMH");
        private static readonly FxTenor IMMM = new FxTenor("IMMM", "IMMM", "IMMM");
        private static readonly FxTenor IMMU = new FxTenor("IMMU", "IMMU", "IMMU");
        private static readonly FxTenor IMMZ = new FxTenor("IMMZ", "IMMZ", "IMMZ");
        private static readonly FxTenor BROKEN = new FxTenor("BD", "BD", "BD");
        
        private readonly string _prettyName;
        private readonly string _bizString;
        private readonly string _omEndpointString;
        
        private FxTenor(string prettyName, string bizString, string omEndpointString)
        { 
            _prettyName = prettyName;
            _bizString = bizString;
            _omEndpointString = omEndpointString;
        }

        internal string GetBizString()
        {
            return _bizString;
        }

        internal string GetRestString()
        {
            return _omEndpointString;
        }

        private static readonly Dictionary<string, FxTenor> TenorMap = new Dictionary<string, FxTenor>();

        static FxTenor()
        {
            AddFxTenorToMap(TODAY);
            AddFxTenorToMap(TOMORROW_NIGHT);
            AddFxTenorToMap(SPOT);
            AddFxTenorToMap(SPOT_NEXT);
            AddFxTenorToMap(W1);
            AddFxTenorToMap(W2);
            AddFxTenorToMap(W3);
            AddFxTenorToMap(M1);
            AddFxTenorToMap(M2);
            AddFxTenorToMap(M3);
            AddFxTenorToMap(M4);
            AddFxTenorToMap(M5);
            AddFxTenorToMap(M6);
            AddFxTenorToMap(M7);
            AddFxTenorToMap(M8);
            AddFxTenorToMap(M9);
            AddFxTenorToMap(M10);
            AddFxTenorToMap(M11);
            AddFxTenorToMap(Y1);
            AddFxTenorToMap(Y2);
            AddFxTenorToMap(Y3);
            AddFxTenorToMap(IMMH);
            AddFxTenorToMap(IMMM);
            AddFxTenorToMap(IMMU);
            AddFxTenorToMap(IMMZ);
            AddFxTenorToMap(BROKEN);
        }

        private static void AddFxTenorToMap(FxTenor tenor)
        {
            TenorMap[tenor._prettyName.ToUpper()] = tenor;
        }

        public static FxTenor GetTenor(string name)
        {
            FxTenor tenor;
            TenorMap.TryGetValue(name.Trim().ToUpper(), out tenor);
            if (tenor == null)
            {
                throw new ArgumentException("Invalid tenor: " + name +". Valid tenors: "+ 
                                            string.Join(", ", TenorMap.Values.Select(x => x._prettyName)));
            }
            return tenor;
        }
    }
}