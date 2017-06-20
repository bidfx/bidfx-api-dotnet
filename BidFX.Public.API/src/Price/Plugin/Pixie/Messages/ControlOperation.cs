using System;

namespace BidFX.Public.API.Price.Plugin.Pixie.Messages
{
    public enum ControlOperation
    {
        Refresh,
        Toggle
    }

    static class ControlOperationExtenstions
    {
        public static byte GetCode(ControlOperation controlOperation)
        {
            if (controlOperation.Equals(ControlOperation.Refresh))
                return (byte) 'R';
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