using abremir.MSP.Assembler.Models;
using abremir.MSP.Shared.Models;

namespace abremir.MSP.Assembler.Assemblers
{
    public interface IProgramAssembler
    {
        AssembledProgram Assemble(IReadOnlyCollection<ParsedInstruction> parsedInstructions, IReadOnlyDictionary<string, int> dataVariableMap);
    }
}
