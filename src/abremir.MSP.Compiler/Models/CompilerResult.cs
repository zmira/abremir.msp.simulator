using abremir.MSP.Shared.Models;

namespace abremir.MSP.Compiler.Models
{
    public class CompilerResult
    {
        public ICollection<MspError> Errors { get; } = [];
        public ICollection<MspWarning> Warnings { get; } = [];
        public ICollection<LineAddress> LineAddressMap { get; private set; } = [];
        public ICollection<DataAddressVariable> DataAddressVariableMap { get; private set; } = [];

        public byte[] Data { get; private set; } = [];
        public byte[] Program { get; private set; } = [];

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

        public void SetMemory(byte[] data, byte[] program, ICollection<LineAddress> lineAddressMap, ICollection<DataAddressVariable> dataAddressVariableMap)
        {
            Data = data;
            Program = program;
            LineAddressMap = lineAddressMap;
            DataAddressVariableMap = dataAddressVariableMap;
        }
    }
}
