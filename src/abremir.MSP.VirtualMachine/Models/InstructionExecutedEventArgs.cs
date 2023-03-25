using abremir.MSP.Shared.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    public class InstructionExecutedEventArgs : BaseInstructionEventArgs
    {
        public InstructionExecutedEventArgs(ushort pc, ushort sp, Operation operation)
            : base(pc, sp, operation)
        {
        }
    }
}
