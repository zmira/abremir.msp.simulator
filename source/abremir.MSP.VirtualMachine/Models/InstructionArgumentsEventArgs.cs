using abremir.MSP.Shared.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    public class InstructionArgumentsEventArgs(ushort pc, ushort sp, Operation operation, int value, byte? lsb = null, byte? msb = null) : BaseInstructionEventArgs(pc, sp, operation)
    {
        public byte? Lsb { get; } = lsb;
        public byte? Msb { get; } = msb;
        public int Value { get; } = value;
    }
}
