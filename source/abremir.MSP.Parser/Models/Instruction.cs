using abremir.MSP.Shared.Enums;

namespace abremir.MSP.Parser.Models
{
    internal record Instruction(int LineNumber, Operation Operation, int? NumericalValue = null, string? TextIdentifier = null, bool? IsRelative = null);
}
