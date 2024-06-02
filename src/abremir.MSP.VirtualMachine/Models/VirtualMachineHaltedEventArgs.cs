using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    public class VirtualMachineHaltedEventArgs(ushort pc, ushort sp, byte? opCode, Operation? operation, HaltReason reason) : BaseInstructionEventArgs(pc, sp, operation)
    {
        public HaltReason Reason { get; } = reason;
        public byte? OpCode { get; } = opCode;
    }
}
