/// Copyright (c) 2018 BidFX Systems LTD. All Rights Reserved.

using System.Reflection;

namespace BidFX.Public.API
{
    /// <summary>
    /// Provides version information about this implementation of the Public-API.
    /// </summary>
    public class PublicApi
    {
        /// <summary>
        /// The API implementation name.
        /// </summary>
        public static readonly string Name = "Public API .Net";

        /// <summary>
        /// The assemble reference.
        /// </summary>
        public static readonly Assembly Reference = typeof(PublicApi).Assembly;

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