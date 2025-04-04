using abremir.MSP.Parser.Models;

namespace abremir.MSP.Parser.Extensions
{
    internal static class InstructionExtension
    {
        internal static bool IsUnaryOperation(this Instruction instruction)
        {
            return instruction.NumericalValue is null
                && instruction.TextIdentifier is null
                && instruction.IsRelative is null;
        }
    }
}
