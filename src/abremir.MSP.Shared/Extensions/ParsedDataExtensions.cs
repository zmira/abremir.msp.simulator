using System.Collections;
using abremir.MSP.Shared.Models;

namespace abremir.MSP.Shared.Extensions
{
    public static class ParsedDataExtensions
    {
        public static IReadOnlyCollection<byte> GetMemoryMap(this IReadOnlyCollection<ParsedData> parsedData)
        {
            if (parsedData.Count is 0)
            {
                return Array.Empty<byte>();
            }

            var lastAllocation = parsedData.OrderByDescending(data => data.Address).First();
            var lastAllocatedAddress = lastAllocation.Address + lastAllocation.Size;

            var memoryMap = new BitArray(lastAllocatedAddress);

            foreach (var data in parsedData.OrderBy(pData => pData.Address))
            {
                var tempMemoryMap = new BitArray(lastAllocatedAddress);

                for (var offset = 0; offset < data.Size; offset++)
                {
                    tempMemoryMap.Set(data.Address + offset, true);
                }

                memoryMap = memoryMap.Or(tempMemoryMap);
            }

            return memoryMap.Cast<bool>().Select(bit => (byte)(bit ? 1 : 0)).ToList();
        }
    }
}
