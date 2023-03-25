using abremir.MSP.Shared.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    public class InstructionExecutingEventArgs : BaseInstructionEventArgs
    {
        public InstructionExecutingEventArgs(ushort pc, ushort sp, Operation operation)
            : base(pc, sp, operation)
        {
        }
    }
}
