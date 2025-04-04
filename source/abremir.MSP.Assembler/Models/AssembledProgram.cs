using abremir.MSP.Shared.Models;

namespace abremir.MSP.Assembler.Models
{
    public class AssembledProgram
    {
        public static AssembledProgram Empty() => new();

        public byte[] Program { get; }
        public ICollection<MspError> Errors { get; }
        public List<LineAddress> LineAddressMap { get; }

        public AssembledProgram(byte[] program, ICollection<MspError> errors, List<LineAddress> lineAddressMap)
        {
            Program = program;
            Errors = errors;
            LineAddressMap = lineAddressMap;
        }

        private AssembledProgram()
        {
            Program = [];
            Errors = [];
            LineAddressMap = [];
        }
    }
}
