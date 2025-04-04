using abremir.MSP.Assembler.Models;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.Shared.Models;

namespace abremir.MSP.Assembler.Assemblers
{
    public class DataAssembler : IDataAssembler
    {
        public AssembledData Assemble(IReadOnlyCollection<ParsedData> parsedData)
        {
            if (parsedData.Count is 0)
            {
                return AssembledData.Empty();
            }

            var dataVariableMap = parsedData.ToDictionary(data => data.VariableName, data => data.Address);
            var dataMemory = BuildDataMemory(parsedData);

            return new AssembledData(dataVariableMap, dataMemory);
        }

        private static byte[] BuildDataMemory(IReadOnlyCollection<ParsedData> parsedData)
        {
            var lastAllocation = parsedData.OrderByDescending(data => data.Address).First();
            var lastAllocatedAddress = lastAllocation.Address + lastAllocation.Size;

            var dataMemory = new byte[lastAllocatedAddress];
            dataMemory.Initialize();

            foreach (var data in parsedData)
            {
                var start = data.Address;
                var values = data.Values;

                if (data.Values is null)
                {
                    continue;
                }

                data.Values
                    .Select(v => v.ToTwosComplement())
                    .ToArray()
                    .CopyTo(dataMemory, data.Address);
            }

            return dataMemory;
        }
    }
}
