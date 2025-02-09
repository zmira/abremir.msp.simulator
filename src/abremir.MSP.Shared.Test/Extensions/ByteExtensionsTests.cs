namespace abremir.MSP.Shared.Test.Extensions
{
    public class ByteExtensionsTests
    {
        [Theory]
        [InlineData(0, 0)]
        [InlineData(127, 127)]
        [InlineData(128, -128)]
        [InlineData(255, -1)]
        public void FromTwosComplement(byte byteValue, int expectedDecomplementedValue)
        {
            var result = byteValue.FromTwosComplement();

            Check.That(expectedDecomplementedValue).Is(result);
        }

        [Theory]
        [InlineData(new byte[] { 0, 0 }, 0)]
        [InlineData(new byte[] { 255, 0 }, 255)]
        [InlineData(new byte[] { 0, 1 }, 256)]
        [InlineData(new byte[] { 255, 124 }, Constants.Constants.MemoryCapacity - 1)]
        public void ToMemoryAddress(byte[] byteValue, int expectedIntValue)
        {
            var result = byteValue.ToMemoryAddress();

            Check.That(expectedIntValue).Is(result);
        }

        [Theory]
        [InlineData(new byte[] { })]
        [InlineData(new byte[] { 0 })]
        [InlineData(new byte[] { 255, 124, 0 })]
        public void ToMemoryAddress_InvalidNumberOfElements_ThrowsArgumentOutOfRangeException(byte[] byteValue)
        {
            Check.ThatCode(byteValue.ToMemoryAddress).Throws<ArgumentOutOfRangeException>();
        }
    }
}
