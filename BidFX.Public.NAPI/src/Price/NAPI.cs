using System.Reflection;

namespace BidFX.Public.NAPI.Price
{
    /// <summary>
    /// Provides version information about this implementation of the NAPIClient.
    /// </summary>
    public class NAPI
    {
        /// <summary>
        /// The API implementation name.
        /// </summary>
        public const string Name = "NAPIClient";

        /// <summary>
        /// The assemble reference.
        /// </summary>
        public static readonly Assembly Reference = typeof(NAPI).Assembly;

        /// <summary>
        /// The package name.
        /// </summary>
        public static readonly string Package = Reference.GetName().Name;

        /// <summary>
        /// The version number.
        /// </summary>
        public static readonly string Version = Reference.GetName().Version.ToString();
    }
}