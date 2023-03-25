using abremir.MSP.Shared.Models;

namespace abremir.MSP.Assembler.Models
{
    public class AssemblerResult
    {
        public static AssemblerResult Empty() => new();

        public byte[] Data { get; }
        public byte[] Program { get; }
        public ICollection<MspError> Errors { get; }
        public ICollection<LineAddress> LineAddressMap { get; }
        public ICollection<DataAddressVariable> DataAddressVariableMap { get; }

        public AssemblerResult(
            byte[] data,
            byte[] program,
            ICollection<MspError> errors,
            ICollection<LineAddress> lineAddressMap,
            ICollection<DataAddressVariable> dataAddressVariableMap)
        {
            Data = data;
            Program = program;
            Errors = errors;
            LineAddressMap = lineAddressMap;
            DataAddressVariableMap = dataAddressVariableMap;
        }

        private AssemblerResult()
        {
            Data = Array.Empty<byte>();
            Program = Array.Empty<byte>();
            Errors = new List<MspError>();
            LineAddressMap = new List<LineAddress>();
            DataAddressVariableMap = new List<DataAddressVariable>();
        }
    }
}
