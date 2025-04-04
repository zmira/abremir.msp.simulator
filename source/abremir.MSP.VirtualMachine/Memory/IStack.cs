using System.Collections.Generic;

namespace abremir.MSP.VirtualMachine.Memory
{
    public interface IStack
    {
        IReadOnlyCollection<byte> StackData { get; }
        ushort SP { get; }

        void Clear();
        bool TryPush(byte value);
        bool TryPop(out byte value);
    }
}
