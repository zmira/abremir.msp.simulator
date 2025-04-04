using System;

namespace abremir.MSP.VirtualMachine.Models
{
    public class DataMemoryUpdatedEventArgs(ushort address, byte value) : EventArgs
    {
        public ushort Address { get; } = address;
        public byte Value { get; } = value;
    }
}
