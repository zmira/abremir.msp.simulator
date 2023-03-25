using System;

namespace abremir.MSP.VirtualMachine.Models
{
    public class DataMemoryUpdatedEventArgs : EventArgs
    {
        public ushort Address { get; }
        public byte Value { get; }

        public DataMemoryUpdatedEventArgs(ushort address, byte value)
        {
            Address = address;
            Value = value;
        }
    }
}
