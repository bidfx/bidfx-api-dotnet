using System.Reflection;

namespace TS.Pisa
{
    /// <summary>
    /// Provides version information about this implementation of the Pisa API.
    /// </summary>
    public class Pisa
    {
        /// <summary>
        /// The API implementation name.
        /// </summary>
        public const string Name = "Pisa.NET";

        /// <summary>
        /// The assemble reference.
        /// </summary>
        public static readonly Assembly Reference = typeof(Pisa).Assembly;

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