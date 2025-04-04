using System.ComponentModel;

namespace abremir.MSP.VirtualMachine.Enums
{
    public enum InterruptReason
    {
        [Description("Waiting for numeric input")]
        InputValue,
        [Description("Waiting for character input")]
        InputCharacter
    }
}
