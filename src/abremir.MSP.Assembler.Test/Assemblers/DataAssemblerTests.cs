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
            var result = _assembler.Assemble(new List<ParsedData>());

            result.Should().NotBeNull();
            result.DataVariableMap.Should().BeEmpty();
            result.Data.Should().BeEmpty();
        }

        [Fact]
        public void Assemble_WithData_ReturnsDataMemoryAndVariableMap()
        {
            var parsedData = new List<ParsedData>
            {
                new(1, "a", 0, 1, null),
                new(2, "b", 1, 10, new[]{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }),
                new(3, "c", 500, 3, new[] { 9, 99, Constants.Max8BitValue }),
                new(4, "d", Constants.MemoryCapacity - 1, 1, new[]{ Constants.Min8BitValue + 1 })
            };

            var result = _assembler.Assemble(parsedData);

            result.Should().NotBeNull();
            result.DataVariableMap.Should().NotBeEmpty();
            result.Data.Should().NotBeEmpty();
            parsedData.ForEach(data =>
            {
                result.DataVariableMap.ContainsKey(data.VariableName).Should().BeTrue();
                result.DataVariableMap[data.VariableName].Should().Be(data.Address);

                for (var valueIndex = 0; valueIndex < (data.Values ?? Array.Empty<int>()).Length; valueIndex++)
                {
                    result.Data[data.Address + valueIndex].Should().Be(data.Values![valueIndex].ToTwosComplement());
                }
            });
        }
    }
}