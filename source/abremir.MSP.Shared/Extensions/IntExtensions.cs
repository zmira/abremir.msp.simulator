namespace abremir.MSP.Shared.Extensions
{
    public static class IntExtensions
    {
        public static byte ToTwosComplement(this int value)
        {
            if (value < Constants.Constants.Min8BitValue || value > Constants.Constants.Max8BitValue)
            {
                throw new OverflowException();
            }

            return value < 0
                ? (byte)((~(-1 * value)) + 1)
                : (byte)value;
        }

        public static byte[] ToLeastAndMostSignificantBytes(this int value)
        {
            if (value < 0 || value > ushort.MaxValue)
            {
                throw new OverflowException();
            }

            var lowerByte = Convert.ToByte(value % 256);
            var upperByte = Convert.ToByte((value - lowerByte) >> 8);

            return [lowerByte, upperByte];
        }
    }
}
