using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    public class VirtualMachineHaltedEventArgs : BaseInstructionEventArgs
    {
        public HaltReason Reason { get; }
        public byte? OpCode { get; }

        public VirtualMachineHaltedEventArgs(ushort pc, ushort sp, byte? opCode, Operation? operation, HaltReason reason)
            : base(pc, sp, operation)
        {
            OpCode = opCode;
            Reason = reason;
        }
    }
}
