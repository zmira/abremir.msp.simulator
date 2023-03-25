using abremir.MSP.Shared.Models;

namespace abremir.MSP.Parser.Models
{
    public record TokenListParserValue(IReadOnlyCollection<ParsedData>? Data, IReadOnlyCollection<ParsedInstruction>? Instructions);
}
