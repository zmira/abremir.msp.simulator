namespace abremir.MSP.Shared.Test.Extensions
{
    [TestClass]
    public class ByteExtensionsTests
    {
        [TestMethod]
        [DataRow((byte)0, 0)]
        [DataRow((byte)127, 127)]
        [DataRow((byte)128, -128)]
        [DataRow((byte)255, -1)]
        public void FromTwosComplement(byte byteValue, int expectedDecomplementedValue)
        {
            var result = byteValue.FromTwosComplement();

            Check.That(expectedDecomplementedValue).Is(result);
        }

        [TestMethod]
        [DataRow(new byte[] { 0, 0 }, 0)]
        [DataRow(new byte[] { 255, 0 }, 255)]
        [DataRow(new byte[] { 0, 1 }, 256)]
        [DataRow(new byte[] { 255, 124 }, Constants.Constants.MemoryCapacity - 1)]
        public void ToMemoryAddress(byte[] byteValue, int expectedIntValue)
        {
            var result = byteValue.ToMemoryAddress();

            Check.That(expectedIntValue).Is(result);
        }

        [TestMethod]
        [DataRow(new byte[] { })]
        [DataRow(new byte[] { 0 })]
        [DataRow(new byte[] { 255, 124, 0 })]
        public void ToMemoryAddress_InvalidNumberOfElements_ThrowsArgumentOutOfRangeException(byte[] byteValue)
        {
            Check.ThatCode(byteValue.ToMemoryAddress).Throws<ArgumentOutOfRangeException>();
        }
    }
}
