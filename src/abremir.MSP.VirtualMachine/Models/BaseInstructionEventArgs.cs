using System;
using abremir.MSP.Shared.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    public class BaseInstructionEventArgs : EventArgs
    {
        public ushort PC { get; }
        public ushort SP { get; }
        public Operation? Operation { get; }

        public BaseInstructionEventArgs(ushort pc, ushort sp, Operation? operation)
        {
            PC = pc;
            SP = sp;
            Operation = operation;
        }
    }
}
