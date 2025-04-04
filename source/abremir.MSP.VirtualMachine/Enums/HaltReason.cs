using System.ComponentModel;

namespace abremir.MSP.VirtualMachine.Enums
{
    public enum HaltReason
    {
        [Description("HALT instruction")]
        HaltInstruction,
        [Description("Manual halt of virtual machine")]
        ForceHalt,
        [Description("Divide by zero")]
        DivideByZero = 100,
        [Description("Underflow error")]
        UnderflowError = 101,
        [Description("Overflow error")]
        OverflowError = 102,
        [Description("Stack overflow")]
        StackFull = 103,
        [Description("Stack underflow")]
        StackEmpty = 104,
        [Description("Memory address violation")]
        MemoryAddressViolation = 105,
        [Description("Unknown instruction")]
        UnknownOperation = 106
    }
}
