using abremir.MSP.Assembler.Models;
using abremir.MSP.Shared.Models;

namespace abremir.MSP.Assembler
{
    public interface IAssembler
    {
        AssemblerResult Assemble(IReadOnlyCollection<ParsedData> parsedData, IReadOnlyCollection<ParsedInstruction> parsedInstructions);
    }
}
