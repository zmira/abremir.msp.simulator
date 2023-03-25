using abremir.MSP.Assembler.Models;
using abremir.MSP.Shared.Models;

namespace abremir.MSP.Assembler.Assemblers
{
    public interface IDataAssembler
    {
        AssembledData Assemble(IReadOnlyCollection<ParsedData> parsedData);
    }
}
