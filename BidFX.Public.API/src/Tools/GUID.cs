/// Copyright (c) 2018 BidFX Systems Ltd. All Rights Reserved.

using System;
using System.Text;

namespace BidFX.Public.API.Price.Tools
{
    /// <summary>This class provides a globally unique identifier (GUID).</summary>
    /// <remarks>
    /// This class provides a globally unique identifier (GUID). The probability of two GUID's being equal is so small that
    /// it can be considered inconceivable. A GUID is therefore an ideal way to uniquely identify an application in a
    /// distributed system.
    /// </remarks>
    /// <author>Paul Sweeny</author>
    internal sealed class GUID
    {
        private readonly byte[] _guid;

        /// <summary>Creates a new GUID.</summary>
        public GUID()
        {
            _guid = Guid.NewGuid().ToByteArray();
        }

        public override string ToString()
        {
            return ByteArrayToString(_guid);
        }

        private static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }
    }
}