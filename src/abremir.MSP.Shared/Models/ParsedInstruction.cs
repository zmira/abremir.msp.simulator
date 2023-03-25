using abremir.MSP.Shared.Enums;

namespace abremir.MSP.Shared.Models
{
    public record ParsedInstruction(int LineNumber, Operation Operation, int? NumericalValue = null, string? TargetTextIdentifier = null, bool? IsRelative = null, string? InstructionLabel = null);
}
