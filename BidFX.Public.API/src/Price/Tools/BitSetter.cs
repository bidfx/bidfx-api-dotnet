namespace BidFX.Public.API.Price.Tools
{
    public class BitSetter
    {
        public static uint ChangeBit(uint bitSet, int bitPos, bool on)
        {
            return on ? SetBit(bitSet, bitPos) : ClearBit(bitSet, bitPos);
        }

        public static uint SetBit(uint bitSet, int bitPos)
        {
            return bitSet | (uint) 1 << bitPos;
        }

        public static uint ClearBit(uint bitSet, int bitPos)
        {
            return bitSet & (uint) ~(1 << bitPos);
        }

        public static bool IsSet(uint bitSet, int bitPos)
        {
            return (bitSet & 1 << bitPos) != 0;
        }
    }
}