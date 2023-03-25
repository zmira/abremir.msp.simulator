using System.Collections.Generic;
using System.Linq;
using abremir.MSP.Shared.Constants;

namespace abremir.MSP.VirtualMachine.Memory
{
    public class Stack : VirtualMachineMemory, IStack
    {
        public IReadOnlyCollection<byte> StackData => MemoryData.TakeLast(Constants.MemoryCapacity - SP - 1).ToArray();

        public ushort SP { get; private set; } = Constants.MemoryCapacity - 1;

        public override void Clear()
        {
            base.Clear();
            SP = Constants.MemoryCapacity - 1;
        }

        public bool TryPush(byte value)
        {
            if (SP - 1 < 0)
            {
                return false;
            }

            TrySet(SP, value);

            SP--;

            return true;
        }

        public bool TryPop(out byte value)
        {
            value = 0;

            if (++SP is Constants.MemoryCapacity)
            {
                return false;
            }

            TryGet(SP, out value);

            TrySet(SP, 0);

            return true;
        }
    }
}
