namespace abremir.MSP.Shared.Extensions
{
    public static class ByteExtensions
    {
        public static sbyte FromTwosComplement(this byte value)
        {
            return (sbyte)((value & 0x80) is 0
                ? value
                : ((byte)~value + 1) * -1);
        }

        public static int ToMemoryAddress(this byte[] value)
        {
            if (value?.Length is not 2)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Array must have only two elements");
            }

            return (value[1] << 8) + value[0];
        }
    }
}
