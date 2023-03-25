using abremir.MSP.Shared.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    public class InstructionArgumentsEventArgs : BaseInstructionEventArgs
    {
        public byte? Lsb { get; }
        public byte? Msb { get; }
        public int Value { get; }

        public InstructionArgumentsEventArgs(ushort pc, ushort sp, Operation operation, int value, byte? lsb = null, byte? msb = null)
            : base(pc, sp, operation)
        {
            Value = value;
            Lsb = lsb;
            Msb = msb;
        }
    }
}
