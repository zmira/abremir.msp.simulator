using System;
using System.Collections.Generic;
using abremir.MSP.Shared.Constants;

namespace abremir.MSP.VirtualMachine.Memory
{
    public class VirtualMachineMemory : IVirtualMachineMemory
    {
        private byte[] InternalMemory { get; } = new byte[Constants.MemoryCapacity];

        public IReadOnlyCollection<byte> MemoryData => [.. InternalMemory];

        private readonly bool _isReadOnly;

        public VirtualMachineMemory(bool isReadOnly = false)
        {
            Clear();
            _isReadOnly = isReadOnly;
        }

        public virtual void Clear()
        {
            Array.Clear(InternalMemory, 0, Constants.MemoryCapacity);
        }

        public void SetMemory(byte[] data)
        {
            Array.Copy(data, InternalMemory, data.Length > Constants.MemoryCapacity ? Constants.MemoryCapacity : data.Length);
        }

        public bool TryGet(ushort address, out byte value)
        {
            value = 0;

            if (address >= Constants.MemoryCapacity)
            {
                return false;
            }

            value = InternalMemory[address];

            return true;
        }

        public bool TrySet(ushort address, byte value)
        {
            if (_isReadOnly)
            {
                return true;
            }

            if (address >= Constants.MemoryCapacity)
            {
                return false;
            }

            InternalMemory[address] = value;

            return true;
        }
    }
}
