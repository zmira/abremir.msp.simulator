using System.Collections.Generic;

namespace abremir.MSP.VirtualMachine.Memory
{
    public interface IVirtualMachineMemory
    {
        IReadOnlyCollection<byte> MemoryData { get; }

        void SetMemory(byte[] data);
        void Clear();
        bool TryGet(ushort address, out byte value);
        bool TrySet(ushort address, byte value);
    }
}
