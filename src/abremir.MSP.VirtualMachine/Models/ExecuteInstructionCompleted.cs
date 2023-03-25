using abremir.MSP.Shared.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    internal record ExecuteInstructionCompleted(Operation Operation, ushort? Address = null);
}
