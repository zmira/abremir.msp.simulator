using System.ComponentModel;

namespace abremir.MSP.Shared.Enums
{
    public enum Warning
    {
        [Description("No variables declared")]
        DataNoVariablesDeclared = 0,
        [Description("Uninitialized Data Memory identifier(s)")]
        DataUninitializedValues = 1,
        [Description("Data Memory identifier declared, but never referenced")]
        DataUnusedVariable = 2,
        [Description("No instructions declared")]
        CodeNoInstructionsDeclared = 3,
        [Description("Label declared, but never referenced")]
        CodeUnusedLabel = 4,
        [Description("Data Memory identifier matches label name")]
        CodeLabelSharesNameWithVariable = 5,
        [Description("PSHA instruction references memory space not reserved by any Data Memory identifier")]
        CodeAddressDoesNotReferenceVariableSpace = 6,
        [Description("HALT instruction not found")]
        CodeNoHaltInstructionDeclared = 7
    }
}
