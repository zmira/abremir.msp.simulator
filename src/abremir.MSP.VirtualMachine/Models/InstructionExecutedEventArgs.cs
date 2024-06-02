using abremir.MSP.Shared.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    public class InstructionExecutedEventArgs(ushort pc, ushort sp, Operation operation) : BaseInstructionEventArgs(pc, sp, operation)
    {
    }
}
