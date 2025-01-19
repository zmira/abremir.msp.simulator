namespace abremir.MSP.Shared.Test.Extensions
{
    public class IntExtensionsTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(127, 127)]
        [InlineData(-128, 128)]
        [InlineData(-1, 255)]
        public void ToTwosComplement_ValidValues_ReturnsExpected(int intValue, byte expectedComplementValue)
        {
            var result = intValue.ToTwosComplement();

            expectedComplementValue.ShouldBe(result);
        }

        [Theory]
        [InlineData(Constants.Constants.Min8BitValue - 1)]
        [InlineData(Constants.Constants.Max8BitValue + 1)]
        public void ToTwosComplement_InvalidValues_ThrowsException(int intValue)
        {
            Assert.Throws<OverflowException>(() => intValue.ToTwosComplement());
        }

        [Theory]
        [InlineData(0, new byte[] { 0, 0 })]
        [InlineData(255, new byte[] { 255, 0 })]
        [InlineData(256, new byte[] { 0, 1 })]
        [InlineData(Constants.Constants.MemoryCapacity - 1, new byte[] { 255, 124 })]
        [InlineData(short.MaxValue, new byte[] { 255, 127 })]
        [InlineData(ushort.MaxValue, new byte[] { 255, 255 })]
        public void ToLeastAndMostSignificantBytes_ValidValues_ReturnsExpected(int intValue, byte[] expectedBytes)
        {
            var result = intValue.ToLeastAndMostSignificantBytes();

            expectedBytes.ShouldBeEquivalentTo(result);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(int.MaxValue)]
        [InlineData(ushort.MaxValue + 1)]
        public void ToLeastAndMostSignificantBytes_InvalidValues_ThrowsOverflowException(int intValue)
        {
            Assert.Throws<OverflowException>(() => intValue.ToLeastAndMostSignificantBytes());
        }
    }
}
