namespace BidFX.Public.API.Price.Subject
{
    public class SubjectComponentName
    {
        /// <summary>
        /// Account code component field name.
        /// </summary>
        public const string BuySideAccount = "BuySideAccount";

        /// <summary>
        /// Allocation account component field name.
        /// </summary>
        public const string BuySideAllocAccount = "BuySideAllocAccount";

        /// <summary>
        /// Allocation quantity component field name.
        /// </summary>
        public const string AllocQuantity = "AllocQuantity";

        /// <summary>
        /// Far allocation quantity component field name.
        /// </summary>
        public const string FarAllocQuantity = "FarAllocQuantity";

        /// <summary>
        /// Alternate user name component field name used for receiving pricing for another user.
        /// </summary>
        public const string OnBehalfOf = "OnBehalfOf";

        /// <summary>
        /// Asset class component field name.
        /// </summary>
        public const string AssetClass = "AssetClass";

        /// <summary>
        /// Currency component field name.
        /// </summary>
        public const string Currency = "Currency";

        /// <summary>
        /// Currency 2 component field name used for FX Swaps.
        /// </summary>
        public const string FarCurrency = "FarCurrency";

        /// <summary>
        /// Buy side ID compoenent field name.
        /// </summary>
        public const string BuySideId = "BuySideID";

        /// <summary>
        /// Fixing date needed for FX NDF subscriptions.
        /// </summary>
        public const string FixingDate = "FixingDate";

        /// <summary>
        /// Fixing date needed for FX NDF subscriptions.
        /// </summary>
        public const string FarFixingDate = "FarFixingDate";

        /// <summary>
        /// Price level component field name.
        /// </summary>
        public const string Level = "Level";

        /// <summary>
        /// Number of allocations component field name.
        /// </summary>
        public const string NumAllocs = "NumAllocs";

        /// <summary>
        /// Quantity component field name.
        /// </summary>
        public const string Quantity = "Quantity";

        /// <summary>
        /// Quantity 2 component field name used for FX Swaps.
        /// </summary>
        public const string FarQuantity = "FarQuantity";

        /// <summary>
        /// Price source component field name.
        /// </summary>
        public const string LiquidityProvider = "LiquidityProvider";

        /// <summary>
        /// Used to sub-categorise an asset class.
        /// </summary>
        public const string DealType = "DealType";

        /// <summary>
        /// Symbol component field name.
        /// </summary>
        public const string Symbol = "Symbol";

        /// <summary>
        /// FX Tenor name component field name.
        /// </summary>
        public const string Tenor = "Tenor";

        /// <summary>
        /// FX Tenor 2 name component field name used for FX Swaps.
        /// </summary>
        public const string FarTenor = "FarTenor";

        /// <summary>
        /// Quote style component field name: for distinguising requests for quote versus request for stream.
        /// </summary>
        public const string RequestFor = "RequestFor";

        /// <summary>
        /// User name component field name.
        /// </summary>
        public const string User = "User";

        /// <summary>
        /// Value date component field name
        /// </summary>
        public const string SettlementDate = "SettlementDate";

        /// <summary>
        /// Value date 2 component field name used for FX Swaps
        /// </summary>
        public const string FarSettlementDate = "FarSettlementDate";
    }
}