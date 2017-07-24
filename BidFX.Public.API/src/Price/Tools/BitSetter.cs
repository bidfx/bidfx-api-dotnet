namespace BidFX.Public.API.Price.Tools
{
    /// <summary>
    /// This class provides basic bit manipulation functions
    /// </summary>
    internal class BitSetter
    {
        /// <summary>
        /// Changes a bit in a bit-set
        /// </summary>
        /// <param name="bitSet">the bit-set to operate upon</param>
        /// <param name="bitPos">the bit position (0-31) to change</param>
        /// <param name="on">the new bit (true = 1, false = 0)</param>
        /// <returns>the updated bit-set</returns>
        public static uint ChangeBit(uint bitSet, int bitPos, bool on)
        {
            return on ? SetBit(bitSet, bitPos) : ClearBit(bitSet, bitPos);
        }

        /// <summary>
        /// Set a bit in a bit-set
        /// </summary>
        /// <param name="bitSet">the bit-set to operate upon</param>
        /// <param name="bitPos">the bit position (0-31) to set</param>
        /// <returns>the updated bit-set</returns>
        public static uint SetBit(uint bitSet, int bitPos)
        {
            return bitSet | (uint) 1 << bitPos;
        }

        /// <summary>
        /// Clears a bit in a bit-set
        /// </summary>
        /// <param name="bitSet">the bit-set to operate upon</param>
        /// <param name="bitPos">the bit position (0-31) to clear</param>
        /// <returns>the updated bit-set</returns>
        public static uint ClearBit(uint bitSet, int bitPos)
        {
            return bitSet & (uint) ~(1 << bitPos);
        }

        /// <summary>
        /// Checks if a bit is set in a bit-set
        /// </summary>
        /// <param name="bitSet">the bit-set to check</param>
        /// <param name="bitPos">the bit position (0-31) to check</param>
        /// <returns>true if the bit is set and false otherwise</returns>
        public static bool IsSet(uint bitSet, int bitPos)
        {
            return (bitSet & 1 << bitPos) != 0;
        }
    }
}