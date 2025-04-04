using abremir.MSP.Shared.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    public class InstructionExecutingEventArgs(ushort pc, ushort sp, Operation operation) : BaseInstructionEventArgs(pc, sp, operation)
    {
    }
}
