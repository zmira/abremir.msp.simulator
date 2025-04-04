namespace abremir.MSP.Assembler.Models
{
    public class AssembledData
    {
        public static AssembledData Empty() => new();

        public IReadOnlyDictionary<string, int> DataVariableMap { get; }
        public byte[] Data { get; }

        public AssembledData(IReadOnlyDictionary<string, int> dataVariableMap, byte[] data)
        {
            DataVariableMap = dataVariableMap;
            Data = data;
        }

        private AssembledData()
        {
            DataVariableMap = new Dictionary<string, int>();
            Data = [];
        }
    }
}
