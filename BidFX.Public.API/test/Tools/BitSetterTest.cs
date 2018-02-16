using NUnit.Framework;

namespace BidFX.Public.API.Price.Tools
{
    public class BitSetterTest
    {
        [Test]
        public void TestChangeBit()
        {
            const int bitset = 0x33;
            Assert.AreEqual(0x33, BitSetter.ChangeBit(bitset, 0, true));
            Assert.AreEqual(0x32, BitSetter.ChangeBit(bitset, 0, false));

            Assert.AreEqual(0x33, BitSetter.ChangeBit(bitset, 1, true));
            Assert.AreEqual(0x31, BitSetter.ChangeBit(bitset, 1, false));

            Assert.AreEqual(0x37, BitSetter.ChangeBit(bitset, 2, true));
            Assert.AreEqual(0x33, BitSetter.ChangeBit(bitset, 2, false));
        }

        [Test]
        public void TestSetBitInEmptyBitSet()
        {
            const uint bitset = 0;
            Assert.AreEqual(1 << 0, BitSetter.SetBit(bitset, 0));
            Assert.AreEqual(1 << 1, BitSetter.SetBit(bitset, 1));
            Assert.AreEqual(1 << 9, BitSetter.SetBit(bitset, 9));
            Assert.AreEqual(1 << 24, BitSetter.SetBit(bitset, 24));
            Assert.AreEqual(1 << 30, BitSetter.SetBit(bitset, 30));
        }

        [Test]
        public void TestSetBit()
        {
            const int bitset = 0x55;
            Assert.AreEqual(0x55, BitSetter.SetBit(bitset, 0));
            Assert.AreEqual(0x57, BitSetter.SetBit(bitset, 1));
            Assert.AreEqual(0x255, BitSetter.SetBit(bitset, 9));
            Assert.AreEqual(0x01000055, BitSetter.SetBit(bitset, 24));
            Assert.AreEqual(0x80000055, BitSetter.SetBit(bitset, 31));
        }

        [Test]
        public void TestClearBitInEmptyBitSetIsAlwaysZero()
        {
            const int bitset = 0;
            Assert.AreEqual(0, BitSetter.ClearBit(bitset, 0));
            Assert.AreEqual(0, BitSetter.ClearBit(bitset, 1));
            Assert.AreEqual(0, BitSetter.ClearBit(bitset, 9));
            Assert.AreEqual(0, BitSetter.ClearBit(bitset, 24));
            Assert.AreEqual(0, BitSetter.ClearBit(bitset, 31));
        }

        [Test]
        public void TestClearBitInFullBitSetBit()
        {
            uint bitset = 0xffffffff;
            Assert.AreEqual(0xfffffffe, BitSetter.ClearBit(bitset, 0));
            Assert.AreEqual(0xfffffffd, BitSetter.ClearBit(bitset, 1));
            Assert.AreEqual(0xfffffdff, BitSetter.ClearBit(bitset, 9));
            Assert.AreEqual(0xfeffffff, BitSetter.ClearBit(bitset, 24));
            Assert.AreEqual(0x7fffffff, BitSetter.ClearBit(bitset, 31));
        }

        [Test]
        public void TestClearBit()
        {
            uint bitset = (uint) 0x55555555;
            Assert.AreEqual(0x55555554, BitSetter.ClearBit(bitset, 0));
            Assert.AreEqual(0x55555555, BitSetter.ClearBit(bitset, 1));
            Assert.AreEqual(0x55555555, BitSetter.ClearBit(bitset, 9));
            Assert.AreEqual(0x54555555, BitSetter.ClearBit(bitset, 24));
            Assert.AreEqual(0x55555555, BitSetter.ClearBit(bitset, 31));
        }

        [Test]
        public void TestIsSetInEmptyBitset()
        {
            const int bitset = 0;
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 0));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 1));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 10));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 31));
        }

        [Test]
        public void TestIsSetInFullBitset()
        {
            const uint bitset = 0xFFFFFFFF;
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 0));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 1));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 10));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 31));
        }

        [Test]
        public void TestIsSetWithSomeBitsSet()
        {
            const int bitset = 0x33333333;
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 0));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 1));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 2));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 3));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 4));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 5));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 6));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 7));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 8));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 9));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 10));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 11));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 12));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 13));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 14));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 15));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 16));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 17));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 18));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 19));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 20));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 21));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 22));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 23));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 24));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 25));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 26));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 27));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 28));
            Assert.AreEqual(true, BitSetter.IsSet(bitset, 29));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 30));
            Assert.AreEqual(false, BitSetter.IsSet(bitset, 31));
        }
    }
}