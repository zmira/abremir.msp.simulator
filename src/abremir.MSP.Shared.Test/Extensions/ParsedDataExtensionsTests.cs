using abremir.MSP.Shared.Models;

namespace abremir.MSP.Shared.Test.Extensions
{
    public class ParsedDataExtensionsTests
    {
        [Fact]
        public void GetMemoryMap_EmptyCollection_ReturnsEmptyArray()
        {
            var parsedData = new List<ParsedData>();

            var result = parsedData.GetMemoryMap();

            result.Should().BeEmpty();
        }

        [Fact]
        public void GetMemoryMap_CollectionWithData_ReturnsMemoryMap()
        {
            var parsedData = new List<ParsedData>
            {
                new(0, "a", 0, 1, null),
                new(0, "b", 2, 1, null),
                new(0, "c", 5, 2, null),
                new(0, "d", 10, 1, null),
                new(0, "e", 31, 1, null),
                new(0, "f", Constants.Constants.MemoryCapacity - 1, 1, null)
            };

            var result = parsedData.GetMemoryMap().ToList();

            result.Should().NotBeEmpty();
            parsedData.ForEach(data =>
            {
                for (var offset = 0; offset < data.Size; offset++)
                {
                    result[data.Address + offset].Should().Be(1);
                }
            });
        }
    }
}
