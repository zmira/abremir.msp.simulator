using System;
using abremir.MSP.Shared.Enums;

namespace abremir.MSP.VirtualMachine.Models
{
    public class BaseInstructionEventArgs(ushort pc, ushort sp, Operation? operation) : EventArgs
    {
        public ushort PC { get; } = pc;
        public ushort SP { get; } = sp;
        public Operation? Operation { get; } = operation;
    }
}
