using abremir.MSP.Shared.Models;

namespace abremir.MSP.Shared.Test.Extensions
{
    [TestClass]
    public class ParsedDataExtensionsTests
    {
        [TestMethod]
        public void GetMemoryMap_EmptyCollection_ReturnsEmptyArray()
        {
            List<ParsedData> parsedData = [];

            var result = parsedData.GetMemoryMap();

            Check.That(result).IsEmpty();
        }

        [TestMethod]
        public void GetMemoryMap_CollectionWithData_ReturnsMemoryMap()
        {
            List<ParsedData> parsedData =
            [
                new(0, "a", 0, 1, null),
                new(0, "b", 2, 1, null),
                new(0, "c", 5, 2, null),
                new(0, "d", 10, 1, null),
                new(0, "e", 31, 1, null),
                new(0, "f", Constants.Constants.MemoryCapacity - 1, 1, null)
            ];

            var result = parsedData.GetMemoryMap().ToList();

            Check.That(result).Not.IsEmpty();
            parsedData.ForEach(data =>
            {
                for (var offset = 0; offset < data.Size; offset++)
                {
                    Check.That(result[data.Address + offset]).Is((byte)1);
                }
            });
        }
    }
}
