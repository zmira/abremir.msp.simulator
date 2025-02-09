namespace abremir.MSP.Shared.Test.Extensions
{
    [TestClass]
    public class IntExtensionsTests
    {
        [TestMethod]
        [DataRow(0, (byte)0)]
        [DataRow(127, (byte)127)]
        [DataRow(-128, (byte)128)]
        [DataRow(-1, (byte)255)]
        public void ToTwosComplement_ValidValues_ReturnsExpected(int intValue, byte expectedComplementValue)
        {
            var result = intValue.ToTwosComplement();

            Check.That(expectedComplementValue).Is(result);
        }

        [TestMethod]
        [DataRow(Constants.Constants.Min8BitValue - 1)]
        [DataRow(Constants.Constants.Max8BitValue + 1)]
        public void ToTwosComplement_InvalidValues_ThrowsException(int intValue)
        {
            Check.ThatCode(() => intValue.ToTwosComplement()).Throws<OverflowException>();
        }

        [TestMethod]
        [DataRow(0, new byte[] { 0, 0 })]
        [DataRow(255, new byte[] { 255, 0 })]
        [DataRow(256, new byte[] { 0, 1 })]
        [DataRow(Constants.Constants.MemoryCapacity - 1, new byte[] { 255, 124 })]
        [DataRow(short.MaxValue, new byte[] { 255, 127 })]
        [DataRow(ushort.MaxValue, new byte[] { 255, 255 })]
        public void ToLeastAndMostSignificantBytes_ValidValues_ReturnsExpected(int intValue, byte[] expectedBytes)
        {
            var result = intValue.ToLeastAndMostSignificantBytes();

            Check.That(expectedBytes).Is(result);
        }

        [TestMethod]
        [DataRow(-1)]
        [DataRow(int.MaxValue)]
        [DataRow(ushort.MaxValue + 1)]
        public void ToLeastAndMostSignificantBytes_InvalidValues_ThrowsOverflowException(int intValue)
        {
            Check.ThatCode(() => intValue.ToLeastAndMostSignificantBytes()).Throws<OverflowException>();
        }
    }
}
