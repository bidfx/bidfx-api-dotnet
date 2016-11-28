namespace TS.Pisa
{
    /// <summary>
    /// This class provides a list of price field name constants.
    /// </summary>
    /// <remarks>
    /// Field name constants for typical fields that may be published through the Pisa price API.
    /// No provider will publish more than a small subset of there fields.
    /// </remarks>
    ///
    public class FieldName
    {
        public const string AccruedInterest = "AccruedInterest";
        public const string Ask = "Ask";
        public const string AskExchange = "AskExchange";
        public const string AskForwardPoints = "AskForwardPoints";
        public const string AskEndSize = "AskEndSize";
        public const string AskQuantity = "AskQuantity";
        public const string AskEndQuantity = "AskEndQuantity";
        public const string AskSize = "AskSize";
        public const string AskSpot = "AskSpot";
        public const string AskTick = "AskTick";
        public const string AskTime = "AskTime";
        public const string AskYield = "AskYield";
        public const string AuctionImbalance = "AuctionImbalance";
        public const string AuctionPrice = "AuctionPrice";
        public const string AuctionSize = "AuctionSize";
        public const string AuctionTime = "AuctionTime";
        public const string AuctionStatus = "AuctionStatus";
        public const string BenchmarkAskSpread = "BenchmarkAskSpread";
        public const string BenchmarkBidSpread = "BenchmarkBidSpread";
        public const string BenchmarkLastSpread = "BenchmarkLastSpread";
        public const string BenchmarkMidSpread = "BenchmarkMidSpread";
        public const string BenchmarkAskYield = "BenchmarkAskYield";
        public const string BenchmarkBidYield = "BenchmarkBidYield";
        public const string BenchmarkLastYield = "BenchmarkLastYield";
        public const string BenchmarkMidYield = "BenchmarkMidYield";
        public const string BidParRateEquivalent = "BidParRateEquivalent";
        public const string AskParRateEquivalent = "AskParRateEquivalent";
        public const string Pv01 = "PV01";
        public const string CouponRate = "CouponRate";
        public const string FixedNPV = "FixedNPV";
        public const string Bid = "Bid";
        public const string BidExchange = "BidExchange";
        public const string BidForwardPoints = "BidForwardPoints";
        public const string BidEndSize = "BidEndSize";
        public const string BidQuantity = "BidQuantity";
        public const string BidEndQuantity = "BidEndQuantity";
        public const string BidSize = "BidSize";
        public const string BidSpot = "BidSpot";
        public const string BidTick = "BidTick";
        public const string BidTime = "BidTime";
        public const string BidYield = "BidYield";
        public const string Broker = "Broker";
        public const string Close = "Close";
        public const string PreviousClose = "PreviousClose";
        public const string CloseTime = "CloseTime";
        public const string Convexity = "Convexity";
        public const string Dealability = "Dealability";
        public const string Dividend = "Dividend";
        public const string DividendDate = "DividendDate";
        public const string DividendType = "DividendType";
        public const string Dv01 = "DV01";
        public const string Earnings = "Earnings";
        public const string Exchange = "Exchange";
        public const string ExMarker = "ExMarker";
        public const string ExMarkerDate = "ExMarkerDate";
        public const string High = "High";
        public const string HighTime = "HighTime";
        public const string ImpliedAsk = "ImpliedAsk";
        public const string ImpliedAskSize = "ImpliedAskSize";
        public const string ImpliedBid = "ImpliedBid";
        public const string ImpliedBidSize = "ImpliedBidSize";
        public const string Last = "Last";
        public const string LastExchange = "LastExchange";
        public const string LastSize = "LastSize";
        public const string LastTick = "LastTick";
        public const string LastTime = "LastTime";
        public const string LastYield = "LastYield";
        public const string ExecTime = "ExecTime";
        public const string Low = "Low";
        public const string LowTime = "LowTime";
        public const string MarketMaker = "MarketMaker";
        public const string MarketPhase = "MarketPhase";
        public const string MarketSector = "MarketSector";
        public const string MarketSegment = "MarketSegment";
        public const string ModDuration = "ModDuration";
        public const string MidYield = "MidYield";
        public const string Name = "Name";
        public const string NetChange = "NetChange";
        public const string NumAsks = "NumAsks";
        public const string NumBids = "NumBids";
        public const string Open = "Open";
        public const string OpenInterest = "OpenInterest";
        public const string OpenTime = "OpenTime";
        public const string PercentChange = "PercentChange";
        public const string Pip = "Pip";
        public const string PriceBasis = "PriceBasis";
        public const string PriceID = "PriceID";
        public const string PERatio = "PERatio";
        public const string QuoteCondition = "QuoteCondition";
        public const string SequenceNum = "SequenceNum";
        public const string SettlementPrice = "SettlementPrice";
        public const string SettlementType = "SettlementType";
        public const string SharesOutstanding = "SharesOutstanding";
        public const string Strike = "Strike";
        public const string SystemTime = "SystemTime";
        public const string TimeOfUpdate = "TimeOfUpdate";
        public const string TradingCurrency = "TradingCurrency";
        public const string TradingStatus = "TradingStatus";
        public const string TradingStop = "TradingStop";
        public const string TradeType = "TradeType";
        public const string Vwap = "VWAP";
        public const string Volume = "Volume";
        public const string YearHigh = "YearHigh";
        public const string YearLow = "YearLow";
        public const string ValidUntilTime = "ValidUntilTime";
        public const string ValidDuration = "ValidDuration";
        public const string TheoreticalOpen = "TheoreticalOpen";
        public const string UpperThresholdPrice = "UpperThresholdPrice";
        public const string LowerThresholdPrice = "LowerThresholdPrice";
        public const string BidUnderSize = "BidUnderSize";
        public const string AskOverSize = "AskOverSize";
        public const string CountdownTime = "CountdownTime";
        public const string ReferencePrice = "ReferencePrice";
        public const string ShortSaleRestricted = "ShortSaleRestricted";
    }
}