using System.Collections.Generic;

namespace BidFX.Public.API.Enums
{
    internal class FxTenor
    {
        private static readonly FxTenor TODAY = new FxTenor("TOD", "O/N");
        private static readonly FxTenor TOMORROW_NEXT = new FxTenor("TOM", "T/N");
        private static readonly FxTenor SPOT = new FxTenor("SPOT", "SPOT");
        private static readonly FxTenor SPOT_NEXT = new FxTenor("SPOT_NEXT", "S/N");
        private static readonly FxTenor W1 = new FxTenor("1W", "1W");
        private static readonly FxTenor W2 = new FxTenor("2W", "2W");
        private static readonly FxTenor W3 = new FxTenor("3W", "3W");
        private static readonly FxTenor M1 = new FxTenor("1M", "1M");
        private static readonly FxTenor M2 = new FxTenor("2M", "2M");
        private static readonly FxTenor M3 = new FxTenor("3M", "3M");
        private static readonly FxTenor M4 = new FxTenor("4M", "4M");
        private static readonly FxTenor M5 = new FxTenor("5M", "5M");
        private static readonly FxTenor M6 = new FxTenor("6M", "6M");
        private static readonly FxTenor M7 = new FxTenor("7M", "7M");
        private static readonly FxTenor M8 = new FxTenor("8M", "8M");
        private static readonly FxTenor M9 = new FxTenor("9M", "9M");
        private static readonly FxTenor M10 = new FxTenor("10M", "10M");
        private static readonly FxTenor M11 = new FxTenor("11M", "11M");
        private static readonly FxTenor Y1 = new FxTenor("1Y", "1Y");
        private static readonly FxTenor Y2 = new FxTenor("2Y", "2Y");
        private static readonly FxTenor Y3 = new FxTenor("3Y", "3Y");
        private static readonly FxTenor IMMH = new FxTenor("IMMH", "IMMH");
        private static readonly FxTenor IMMM = new FxTenor("IMMM", "IMMM");
        private static readonly FxTenor IMMU = new FxTenor("IMMU", "IMMU");
        private static readonly FxTenor IMMZ = new FxTenor("IMMZ", "IMMZ");
        private static readonly FxTenor BROKEN = new FxTenor("BD", "BD");
        
        private readonly string _prettyName;
        private readonly string _bizString;
        
        private FxTenor(string prettyName, string bizString)
        { 
            _prettyName = prettyName;
            _bizString = bizString;
        }

        internal string GetBizString()
        {
            return _bizString;
        }

        private static readonly Dictionary<string, FxTenor> TenorMap = new Dictionary<string, FxTenor>();

        static FxTenor()
        {
            AddFxTenorToMap(TODAY);
            AddFxTenorToMap(TOMORROW_NEXT);
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
            TenorMap.TryGetValue(name.ToUpper(), out tenor);
            return tenor;
        }
    }
}