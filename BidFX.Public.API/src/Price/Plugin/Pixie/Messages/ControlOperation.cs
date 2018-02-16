using System;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    /// <summary>
    /// Subscription control operations enumeration.
    /// </summary>
    internal enum ControlOperation
    {
        /// <summary>
        /// Refreshes an existing subscription by sending a subscribe message to its feed. This control is used to
        /// implement Pisa subscription refresh for example to recove from previous subscription errors.
        /// </summary>
        Refresh,

        /// <summary>
        /// Toggles an existing subscription by sending an unsubscribe message then a subscribe message to its feed.
        /// This control is used to force RFQs to be refreshed by the broker.
        /// </summary>
        Toggle
    }

    internal static class ControlOperationExtenstions
    {
        public static byte GetCode(this ControlOperation controlOperation)
        {
            if (controlOperation.Equals(ControlOperation.Refresh))
            {
                return (byte) 'R';
            }

            return (byte) 'T';
        }

        public static ControlOperation FromCode(int code)
        {
            switch (code)
            {
                case 'R':
                    return ControlOperation.Refresh;
                case 'T':
                    return ControlOperation.Toggle;
            }

            throw new ArgumentException("unknown subscription control type code '" + (char) code + "'");
        }
    }
}