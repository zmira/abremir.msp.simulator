using abremir.MSP.Shared.Models;

namespace abremir.MSP.Compiler.Models
{
    public class CompilerResult
    {
        public ICollection<MspError> Errors { get; } = new List<MspError>();
        public ICollection<MspWarning> Warnings { get; } = new List<MspWarning>();
        public ICollection<LineAddress> LineAddressMap { get; private set; } = new List<LineAddress>();
        public ICollection<DataAddressVariable> DataAddressVariableMap { get; private set; } = new List<DataAddressVariable>();

        public byte[] Data { get; private set; } = Array.Empty<byte>();
        public byte[] Program { get; private set; } = Array.Empty<byte>();

        public void AddError(MspError error) => Errors.Add(error);

        public void AddErrors(ICollection<MspError> errors)
        {
            foreach (var error in errors)
            {
                Errors.Add(error);
            }
        }

        public void AddWarnings(ICollection<MspWarning> warnings)
        {
            foreach (var warning in warnings)
            {
                Warnings.Add(warning);
            }
        }

        public void SetMemory(byte[] data, byte[] program, ICollection<LineAddress> lineAddessMap, ICollection<DataAddressVariable> dataAddressVariableMap)
        {
            Data = data;
            Program = program;
            LineAddressMap = lineAddessMap;
            DataAddressVariableMap = dataAddressVariableMap;
        }
    }
}
