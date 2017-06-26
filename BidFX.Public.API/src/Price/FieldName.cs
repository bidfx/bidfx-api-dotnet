using System;

namespace BidFX.Public.API.Price
{
    /// <summary>
    /// This class provides a list of price field name constants.
    /// </summary>
    /// <remarks>
    /// Field name constants for typical fields that may be published through the NAPIClient price API.
    /// No provider ever publishes more than a small subset of there fields.
    /// </remarks>
    ///
    public class FieldName
    {
        /// <summary>
        /// The accrued interest. Type is decimal.
        /// </summary>
        public const string AccruedInterest = "AccruedInterest";


        /// <summary>
        /// The ask price. Type is decimal.
        /// </summary>
        public const string Ask = "Ask";


        /// <summary>
        /// The exchange on which the ask price was quoted. Type is string.
        /// This field might be published for a quote from an aggregate market feed such as US equity NBBO.
        /// </summary>
        public const string AskExchange = "AskExchange";


        /// <summary>
        /// The ask forward points as an offset from spot. Type is decimal. Used in FX quotes.
        /// </summary>
        public const string AskForwardPoints = "AskForwardPoints";


        /// <summary>
        /// The ask size of the end leg of a swap. Type is long. Used in FX swaps.
        /// </summary>
        public const string AskEndSize = "AskEndSize";

        /// <summary>
        /// The ask quantity being quoted for in an RFS/RFQ. Type is decimal.
        /// Used in FX when the quantity quoted is down to the penny.
        /// </summary>
        public const string AskQuantity = "AskQuantity";

        /// <summary>
        /// The ask quantity being quoted for in an RFS/RFQ on the end leg of a swap. Type is decimal.
        /// Used in FX when the quantity quoted is down to the penny.
        /// </summary>
        public const string AskEndQuantity = "AskEndQuantity";

        /// <summary>
        /// The ask size. Type is long.
        /// </summary>
        public const string AskSize = "AskSize";

        /// <summary>
        /// The ask spot rate. Type is decimal. Used in FX quotes.
        /// </summary>
        public const string AskSpot = "AskSpot";

        /// <summary>
        /// The ask price tick direction. Type is <see cref="Tick"/>.
        /// </summary>
        public const string AskTick = "AskTick";

        /// <summary>
        /// The time of last ask price or size tick. Type is <see cref="DateTime"/>.
        /// </summary>
        public const string AskTime = "AskTime";

        /// <summary>
        /// The ask yield. Type is decimal.
        /// </summary>
        public const string AskYield = "AskYield";

        /// <summary>
        /// The overnight (O/N) FX forward ask price. Type is decimal.
        /// </summary>
        public const string AskON = "AskON";

        /// <summary>
        /// The tommorow next (T/N) FX forward ask price. Type is decimal.
        /// </summary>
        public const string AskTN = "AskTN";

        /// <summary>
        /// The spot next (S/N) FX forward ask price. Type is decimal.
        /// </summary>
        public const string AskSN = "AskSN";


        /// <summary>
        /// The one week FX forward ask price. Type is decimal.
        /// </summary>
        public const string Ask1W = "Ask1W";

        /// <summary>
        /// The one month FX forward ask price. Type is decimal.
        /// </summary>
        public const string Ask1M = "Ask1M";

        /// <summary>
        /// The two month FX forward ask price. Type is decimal.
        /// </summary>
        public const string Ask2M = "Ask2M";

        /// <summary>
        /// The three month FX forward ask price. Type is decimal.
        /// </summary>
        public const string Ask3M = "Ask3M";

        /// <summary>
        /// The four month FX forward ask price. Type is decimal.
        /// </summary>
        public const string Ask4M = "Ask4M";

        /// <summary>
        /// The five month FX forward ask price. Type is decimal.
        /// </summary>
        public const string Ask5M = "Ask5M";

        /// <summary>
        /// The six month FX forward ask price. Type is decimal.
        /// </summary>
        public const string Ask6M = "Ask6M";

        /// <summary>
        /// The one year FX forward ask price. Type is decimal.
        /// </summary>
        public const string Ask1Y = "Ask1Y";

        /// <summary>
        /// The auction imbalance, difference between the buy and the sell orders in the auction order book.
        /// Type is decimal.
        /// </summary>
        public const string AuctionImbalance = "AuctionImbalance";

        /// <summary>
        /// The auction indicative uncrossing price. Type is decimal.
        /// </summary>
        public const string AuctionPrice = "AuctionPrice";

        /// <summary>
        /// The auction indicative uncrossing size. Type is long.
        /// </summary>
        public const string AuctionSize = "AuctionSize";

        /// <summary>
        /// The time of auction price. Type is <see cref="DateTime"/>.
        /// </summary>
        public const string AuctionTime = "AuctionTime";

        /// <summary>
        /// The auction status. Type is string.
        /// </summary>
        public const string AuctionStatus = "AuctionStatus";

        /// <summary>
        /// The spread between the ask yield and the benchmark ask yield. Type is decimal.
        /// </summary>
        public const string BenchmarkAskSpread = "BenchmarkAskSpread";

        /// <summary>
        /// The spread between the bid yield and the benchmark bid yield. Type is decimal.
        /// </summary>
        public const string BenchmarkBidSpread = "BenchmarkBidSpread";

        /// <summary>
        /// The spread between the last yield and the benchmark last yield. Type is decimal.
        /// </summary>
        public const string BenchmarkLastSpread = "BenchmarkLastSpread";

        /// <summary>
        /// The spread between the mid yield and the benchmark mid yield. Type is decimal.
        /// </summary>
        public const string BenchmarkMidSpread = "BenchmarkMidSpread";

        /// <summary>
        /// The ask yield of the benchmark instrument. Type is decimal.
        /// </summary>
        public const string BenchmarkAskYield = "BenchmarkAskYield";

        /// <summary>
        /// The bid yield of the benchmark instrument. Type is decimal.
        /// </summary>
        public const string BenchmarkBidYield = "BenchmarkBidYield";

        /// <summary>
        /// The yield of the last trade price of the benchmark instrument. Type is decimal.
        /// </summary>
        public const string BenchmarkLastYield = "BenchmarkLastYield";

        /// <summary>
        /// The mid yield of the benchmark instrument. Type is decimal.
        /// </summary>
        public const string BenchmarkMidYield = "BenchmarkMidYield";

        /// <summary>
        /// The Par Rate Equivalent of the current order price - for Fixed Income products. Type is decimal.
        /// </summary>
        public const string BidParRateEquivalent = "BidParRateEquivalent";

        /// <summary>
        /// The Par Rate Equivalent of the current order price - for Fixed Income products. Type is decimal.
        /// </summary>
        public const string AskParRateEquivalent = "AskParRateEquivalent";

        /// <summary>
        /// PV01 - Present Value of a 1 basis point change in fixed rate. Type is decimal.
        /// </summary>
        public const string PV01 = "PV01";

        /// <summary>
        /// The Interest Rate of the Instrument - Fixed Income. Type is decimal.
        /// </summary>
        public const string CouponRate = "CouponRate";

        /// <summary>
        /// The Present Value of the Fixed Leg - Fixed Income. Type is decimal.
        /// </summary>
        public const string FixedNPV = "FixedNPV";

        /// <summary>
        /// The bid price. Type is decimal.
        /// </summary>
        public const string Bid = "Bid";

        /// <summary>
        /// The exchange on which the bid price was quoted. Type is string.
        /// This field might be published for a quote from an aggregate market feed such as US equity NBBO.
        /// </summary>
        public const string BidExchange = "BidExchange";

        /// <summary>
        /// The bid forward points as an offset from spot. Type is decimal.  Used in FX quotes.
        /// </summary>
        public const string BidForwardPoints = "BidForwardPoints";

        /// <summary>
        /// The bid size of the end leg of a swap. Type is long.  Used in FX swaps.
        /// </summary>
        public const string BidEndSize = "BidEndSize";

        /// <summary>
        /// The bid quantity being quoted for in an RFS/RFQ. Type is decimal.
        /// Used in FX when the quantity quoted is down to the penny.
        /// </summary>
        public const string BidQuantity = "BidQuantity";

        /// <summary>
        /// The bid quantity being quoted for in an RFS/RFQ on the end leg of a swap. Type is decimal.
        /// Used in FX when the quantity quoted is down to the penny.
        /// </summary>
        public const string BidEndQuantity = "BidEndQuantity";

        /// <summary>
        /// The bid size. Type is long.
        /// </summary>
        public const string BidSize = "BidSize";

        /// <summary>
        /// The bid spot rate. Type is decimal. Used in FX.
        /// </summary>
        public const string BidSpot = "BidSpot";

        /// <summary>
        /// The bid price tick direction. Type is <see cref="Tick"/>.
        /// </summary>
        public const string BidTick = "BidTick";

        /// <summary>
        /// The time of last bid price or size tick. Type is <see cref="DateTime"/>.
        /// </summary>
        public const string BidTime = "BidTime";

        /// <summary>
        /// The bid yield. Type is decimal.
        /// </summary>
        public const string BidYield = "BidYield";

        /// <summary>
        /// The overnight (O/N) FX forward bid price. Type is decimal.
        /// </summary>
        public const string BidON = "BidON";

        /// <summary>
        /// The tommorow next (T/N) FX forward bid price. Type is decimal.
        /// </summary>
        public const string BidTN = "BidTN";

        /// <summary>
        /// The spot next (S/N) FX forward bid price. Type is decimal.
        /// </summary>
        public const string BidSN = "BidSN";

        /// <summary>
        /// The one week FX forward bid price. Type is decimal.
        /// </summary>
        public const string Bid1W = "Bid1W";

        /// <summary>
        /// The one month FX forward bid price. Type is decimal.
        /// </summary>
        public const string Bid1M = "Bid1M";

        /// <summary>
        /// The two month FX forward bid price. Type is decimal.
        /// </summary>
        public const string Bid2M = "Bid2M";

        /// <summary>
        /// The three month FX forward bid price. Type is decimal.
        /// </summary>
        public const string Bid3M = "Bid3M";

        /// <summary>
        /// The four month FX forward bid price. Type is decimal.
        /// </summary>
        public const string Bid4M = "Bid4M";

        /// <summary>
        /// The five month FX forward bid price. Type is decimal.
        /// </summary>
        public const string Bid5M = "Bid5M";

        /// <summary>
        /// The six month FX forward bid price. Type is decimal.
        /// </summary>
        public const string Bid6M = "Bid6M";

        /// <summary>
        /// The one year FX forward bid price. Type is decimal.
        /// </summary>
        public const string Bid1Y = "Bid1Y";

        /// <summary>
        /// The broker code. Type is string.
        /// </summary>
        public const string Broker = "Broker";

        /// <summary>
        /// The market close price. Type is decimal.
        /// </summary>
        public const string Close = "Close";

        /// <summary>
        /// The market close price for the previous day of trading. Type is decimal.
        /// </summary>
        public const string PreviousClose = "PreviousClose";

        /// <summary>
        /// The time of the market close price. Type is <see cref="DateTime"/>.
        /// </summary>
        public const string CloseTime = "CloseTime";

        /// <summary>
        /// Convexity is a measure of the curvature in the relationship between bond prices
        /// and bond yields that demonstrates how the duration of a bond changes as the interest rate changes.
        /// Type is decimal.
        /// </summary>
        public const string Convexity = "Convexity";

        /// <summary>
        /// The dealability status of the price; Indicative, Dealable etc. Type is string.
        /// </summary>
        public const string Dealability = "Dealability";

        /// <summary>
        /// The dividend paid on a share. Type is decimal.
        /// </summary>
        public const string Dividend = "Dividend";

        /// <summary>
        /// The dividend date. Type is <see cref="DateTime"/>.
        /// </summary>
        public const string DividendDate = "DividendDate";

        /// <summary>
        /// The dividend type. Type is string.
        /// </summary>
        public const string DividendType = "DividendType";

        /// <summary>
        /// The DV01. Type is decimal.
        /// </summary>
        public const string DV01 = "DV01";

        /// <summary>
        /// The latest reported earnings per share. Type is decimal.
        /// </summary>
        public const string Earnings = "Earnings";

        /// <summary>
        /// The exchange on which the price is quoted. Type is string.
        /// This field might be published for a quote from an aggregate market feed such as US equity NBBO.
        /// </summary>
        public const string Exchange = "Exchange";

        /// <summary>
        /// The ex-marker code, for example: Ex-Dividend, Ex-Capitalisation, Ex-Rights. Type is string.
        /// </summary>
        public const string ExMarker = "ExMarker";

        /// <summary>
        /// The ex-marker start date. Type is <see cref="DateTime"/>.
        /// </summary>
        public const string ExMarkerDate = "ExMarkerDate";

        /// <summary>
        /// The session high price. Type is decimal.
        /// </summary>
        public const string High = "High";

        /// <summary>
        /// The time of session high price. Type is <see cref="DateTime"/>.
        /// </summary>
        public const string HighTime = "HighTime";

        /// <summary>
        /// The implied ask price. Type is decimal.
        /// </summary>
        public const string ImpliedAsk = "ImpliedAsk";

        /// <summary>
        /// The implied ask size for future contracts. Type is long.
        /// </summary>
        public const string ImpliedAskSize = "ImpliedAskSize";

        /// <summary>
        /// The implied bid price. Type is decimal.
        /// </summary>
        public const string ImpliedBid = "ImpliedBid";

        /// <summary>
        /// The implied bid size for future contracts. Type is long.
        /// </summary>
        public const string ImpliedBidSize = "ImpliedBidSize";

        /// <summary>
        /// The last trade price. Type is decimal.
        /// </summary>
        public const string Last = "Last";

        /// <summary>
        /// The exchange on which the last trade price was executed. Type is string.
        /// This field might be published for a quote from an aggregate market feed such as US equity NBBO.
        /// </summary>
        public const string LastExchange = "LastExchange";

        /// <summary>
        /// The last trade size. Type is long.
        /// </summary>
        public const string LastSize = "LastSize";

        /// <summary>
        /// The tick direction of the last trade price. Type is <see cref="Tick"/>.
        /// </summary>
        public const string LastTick = "LastTick";

        /// <summary>
        /// The time of the last trade. Type is <see cref="DateTime"/>.
        /// </summary>
        public const string LastTime = "LastTime";

        /// <summary>
        /// The yield of the last trade price. Type is decimal.
        /// </summary>
        public const string LastYield = "LastYield";

        /// <summary>
        /// The execution time of the last trade. Type is <see cref="DateTime"/>.
        /// </summary>
        public const string ExecTime = "ExecTime";

        /// <summary>
        /// The session low price. Type is decimal.
        /// </summary>
        public const string Low = "Low";

        /// <summary>
        /// The time of the session low price. Type is <see cref="DateTime"/>.
        /// </summary>
        public const string LowTime = "LowTime";

        /// <summary>
        /// The code of the market maker or firm quoting the price (bid and ask). Type is string.
        /// </summary>
        public const string MarketMaker = "MarketMaker";

        /// <summary>
        /// The market phase indicator code. Type is string. Values are exchange dependent.
        /// </summary>
        public const string MarketPhase = "MarketPhase";

        /// <summary>
        /// The market sector code. Type is string. Values are exchange dependent.
        /// </summary>
        public const string MarketSector = "MarketSector";

        /// <summary>
        /// The market segment code. Type is string. Values are exchange dependent.
        /// </summary>
        public const string MarketSegment = "MarketSegment";

        /// <summary>
        /// The modified duration. Type is decimal.
        /// </summary>
        public const string ModDuration = "ModDuration";

        /// <summary>
        /// The bid-ask mid price yield. Type is decimal.
        /// </summary>
        public const string MidYield = "MidYield";

        /// <summary>
        /// The product name of the instrument. Type is string.
        /// </summary>
        public const string Name = "Name";

        /// <summary>
        /// The net change from open price of last trade price. Type is decimal.
        /// </summary>
        public const string NetChange = "NetChange";

        /// <summary>
        /// The number of ask quotes or orders associated with the best ask price.
        /// Type is int.
        /// </summary>
        public const string NumAsks = "NumAsks";

        /// <summary>
        /// The number of bid quotes or orders associated with the best bid price.
        /// Type is int.
        /// </summary>
        public const string NumBids = "NumBids";

        /// <summary>
        /// The session opening price. Type is decimal.
        /// </summary>
        public const string Open = "Open";

        /// <summary>
        /// The amount of open interest (in an option for example). Type is long.
        /// </summary>
        public const string OpenInterest = "OpenInterest";

        /// <summary>
        /// The time of the session opening price. Type is <see cref="DateTime"/>.
        /// </summary>
        public const string OpenTime = "OpenTime";

        /// <summary>
        /// The origin time of the price update as generated at the origin liquidity provider, exchange or dealer.
        /// This time will always come from the source system and will be accurate to the millisecond.
        /// Type is <see cref="DateTime"/>.
        /// </summary>
        public const string OriginTime = "OriginTime";

        /// <summary>
        /// The percentage net change from open price of last trade price. Type is decimal.
        /// </summary>
        public const string PercentChange = "PercentChange";

        /// <summary>
        /// The pip (smallest quotable unit of currency). Type is decimal.
        /// </summary>
        public const string Pip = "Pip";

        /// <summary>
        /// The price basis code. Type is string.
        /// </summary>
        public const string PriceBasis = "PriceBasis";

        /// <summary>
        /// The price ID used to uniquely identify a dealable price. Type is string.
        /// </summary>
        public const string PriceID = "PriceID";

        /// <summary>
        /// The price-earnings ratio. Type is decimal.
        /// </summary>
        public const string PERatio = "PERatio";

        /// <summary>
        /// The quote condition code. Type is string.
        /// </summary>
        public const string QuoteCondition = "QuoteCondition";

        /// <summary>
        /// The sequence number of the latest update; may refer to either the
        /// channel or instrument level sequence, according to the exchange.
        /// Type is long.
        /// </summary>
        public const string SequenceNum = "SequenceNum";

        /// <summary>
        /// The settlement price at the end of the trading session. Type is decimal.
        /// </summary>
        public const string SettlementPrice = "SettlementPrice";

        /// <summary>
        /// The settlement type. Type is string.
        /// </summary>
        public const string SettlementType = "SettlementType";

        /// <summary>
        /// The number of shares outstanding (issued to the public). Type is long.
        /// </summary>
        public const string SharesOutstanding = "SharesOutstanding";

        /// <summary>
        /// The strike price of an option. Type is decimal.
        /// </summary>
        public const string Strike = "Strike";

        /// <summary>
        /// The system delay time, measured in minutes, applied to the price. Type is int.
        /// </summary>
        public const string SystemDelay = "SystemDelay";

        /// <summary>
        /// The estimated system latency measured in milliseconds. Type is long.
        /// </summary>
        public const string SystemLatency = "SystemLatency";

        /// <summary>
        /// The route taken through the pricing infrastructure for the price. Type is string.
        /// </summary>
        public const string SystemRoute = "SystemRoute";

        /// <summary>
        /// The system time of the last price update within the pricing infrastructure.
        /// This will normally be populated by the price server closest to the source of liquidity
        /// but not necessarity the the source itself. Should be to millisecond preceision.
        /// Type is <see cref="DateTime"/>.
        /// </summary>
        public const string SystemTime = "SystemTime";

        /// <summary>
        /// The time of the last price update as given by the price source or vendor feed.
        /// May only be precise to the second. This field is replaced by <see cref="OriginTime"/> for newer feeds.
        /// Type is <see cref="DateTime"/>.
        /// </summary>
        public const string TimeOfUpdate = "TimeOfUpdate";

        /// <summary>
        /// The trading currency code. Type is string.
        /// </summary>
        public const string TradingCurrency = "TradingCurrency";

        /// <summary>
        /// The trading status code. Type is string.
        /// </summary>
        public const string TradingStatus = "TradingStatus";

        /// <summary>
        /// The trading Stop code. Type is string.
        /// </summary>
        public const string TradingStop = "TradingStop";

        /// <summary>
        /// The trade type code. Type is string.
        /// </summary>
        public const string TradeType = "TradeType";

        /// <summary>
        /// The Uniform Practice Code UPC-71 restricted indicator for NASDAQ
        /// stocks. Type is Boolean.
        /// </summary>
        public const string UPCRestricted = "UPCRestricted";

        /// <summary>
        /// The volume-weighted average price. Type is decimal.
        /// </summary>
        public const string VWAP = "VWAP";

        /// <summary>
        /// The cummulative trade volume since the start of the trading session. Type is long.
        /// </summary>
        public const string Volume = "Volume";

        /// <summary>
        /// The year high price. Type is decimal.
        /// </summary>
        public const string YearHigh = "YearHigh";

        /// <summary>
        /// The year low price. Type is decimal.
        /// </summary>
        public const string YearLow = "YearLow";

        /// <summary>
        /// The time until which a quote is valid (used for FX and FI RFQ prices). Type is <see cref="DateTime"/>.
        /// </summary>
        public const string ValidUntilTime = "ValidUntilTime";

        /// <summary>
        /// The valid duration of a quote in milliseconds (used for FX and FI RFQ prices). Type is long.
        /// </summary>
        public const string ValidDuration = "ValidDuration";

        /// <summary>
        /// The theoretical opening price. Type is double.
        /// </summary>
        public const string TheoreticalOpen = "TheoreticalOpen";

        /// <summary>
        /// The upper threshold price. Type is double.
        /// </summary>
        public const string UpperThresholdPrice = "UpperThresholdPrice";

        /// <summary>
        /// The lower threshold price. Type is double.
        /// </summary>
        public const string LowerThresholdPrice = "LowerThresholdPrice";

        /// <summary>
        /// The aggregated number of bids under the market depth. Type is a long
        /// </summary>
        public const string BidUnderSize = "BidUnderSize";

        /// <summary>
        /// The aggregated number of asks over the market depth. Type is a long
        /// </summary>
        public const string AskOverSize = "AskOverSize";

        /// <summary>
        /// A countdown field, the content of which is exchange specific
        /// For example, on SWX when a market is stopped it will reopen a set interval
        /// later, and this field will contain the countdown in seconds until that reopen.
        /// </summary>
        public const string CountdownTime = "CountdownTime";

        /// <summary>
        /// A reference price, the content of which is exchange specific
        /// For example, BMF and Bovespa will sometimes publish a reference price
        /// which is the mid price of the current trading range, but under
        /// some circumstances will instead use the settlement or closing
        /// price instead.
        /// </summary>
        public const string ReferencePrice = "ReferencePrice";

        /// <summary>
        /// Best Bid/Offer limit indicator.
        /// Type of the field is string.
        /// </summary>
        public const string BBOLimit = "BBOLimit";

        /// <summary>
        /// National Best Bid/Offer Limit indicator.
        /// Type of the field is string.
        /// </summary>
        public const string NBBOLimit = "NBBOLimit";

        /// <summary>
        /// Short Sale Restriction indicator for US markets.
        /// Type is string.
        /// </summary>
        public const string ShortSaleRestricted = "ShortSaleRestricted";
    }
}