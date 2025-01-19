using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.Shared.Models;

namespace abremir.MSP.Assembler.Test.Assemblers
{
    public class DataAssemblerTests
    {
        private readonly DataAssembler _assembler;

        public DataAssemblerTests()
        {
            _assembler = new DataAssembler();
        }

        [Fact]
        public void Assemble_WithoutData_ReturnsEmpty()
        {
            var result = _assembler.Assemble([]);

            result.ShouldNotBeNull();
            result.DataVariableMap.ShouldBeEmpty();
            result.Data.ShouldBeEmpty();
        }

        [Fact]
        public void Assemble_WithData_ReturnsDataMemoryAndVariableMap()
        {
            List<ParsedData> parsedData =
            [
                new(1, "a", 0, 1, null),
                new(2, "b", 1, 10, [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]),
                new(3, "c", 500, 3, [9, 99, Constants.Max8BitValue]),
                new(4, "d", Constants.MemoryCapacity - 1, 1, [Constants.Min8BitValue + 1])
            ];

            var result = _assembler.Assemble(parsedData);

            result.ShouldNotBeNull();
            result.DataVariableMap.ShouldNotBeEmpty();
            result.Data.ShouldNotBeEmpty();
            parsedData.ForEach(data =>
            {
                result.DataVariableMap.ContainsKey(data.VariableName).ShouldBeTrue();
                result.DataVariableMap[data.VariableName].ShouldBe(data.Address);

                for (var valueIndex = 0; valueIndex < (data.Values ?? []).Length; valueIndex++)
                {
                    result.Data[data.Address + valueIndex].ShouldBe(data.Values![valueIndex].ToTwosComplement());
                }
            });
        }
    }
}