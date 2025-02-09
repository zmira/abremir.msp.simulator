using abremir.MSP.Shared.Constants;
using abremir.MSP.VirtualMachine.Memory;

namespace abremir.MSP.VirtualMachine.Test.Memory
{
    [TestClass]
    public class VirtualMachineMemoryTests
    {
        private VirtualMachineMemory _virtualMachineMemory;

        public VirtualMachineMemoryTests()
        {
            _virtualMachineMemory = new VirtualMachineMemory();
        }

        [TestMethod]
        public void SetMemory_LessDataThanCapacity_CopiesAll()
        {
            var data = new byte[] { 1, 2, 3, 4 };

            _virtualMachineMemory.SetMemory(data);

            Check.That(_virtualMachineMemory.MemoryData.Count(value => value is not 0)).Is(data.Length);
        }

        [TestMethod]
        public void SetMemory_MoreDataThanCapacity_OnlyCopiesEnoughToFillCapacity()
        {
            var data = new byte[Constants.MemoryCapacity + 1];
            Array.Fill<byte>(data, 1, 0, Constants.MemoryCapacity + 1);

            _virtualMachineMemory.SetMemory(data);

            var memoryData = _virtualMachineMemory.MemoryData;

            Check.That(memoryData.Count(value => value is not 0)).IsNotEqualTo(data.Length);
            Check.That(memoryData.Count(value => value is not 0)).Is(Constants.MemoryCapacity);
        }

        [TestMethod]
        public void Clear_ResetsMemoryData()
        {
            var data = new byte[] { 1, 2, 3, 4 };

            _virtualMachineMemory.SetMemory(data);

            var memoryData = _virtualMachineMemory.MemoryData.Where(value => value is not 0).ToList();

            _virtualMachineMemory.Clear();

            var newMemoryData = _virtualMachineMemory.MemoryData;

            Check.That(newMemoryData.Count(value => value is not 0)).IsNotEqualTo(memoryData.Count);
            Check.That(newMemoryData.All(value => value is 0)).IsTrue();
        }

        [TestMethod]
        public void TryGet_BeyondBounds_ReturnsFalse()
        {
            var result = _virtualMachineMemory.TryGet(Constants.MemoryCapacity, out _);

            Check.That(result).IsFalse();
        }

        [TestMethod]
        public void TryGet_AddressWithinBounds_ReturnsTrue()
        {
            var result = _virtualMachineMemory.TryGet(Constants.MemoryCapacity - 1, out _);

            Check.That(result).IsTrue();
        }

        [TestMethod]
        public void TryGet_AddressWithinBounds_OutputsValue()
        {
            const ushort address = 3;
            const byte value = 99;
            var data = new byte[] { 1, 2, 3, 4 };
            data[address] = value;

            _virtualMachineMemory.SetMemory(data);

            _virtualMachineMemory.TryGet(address, out var outputValue);

            Check.That(outputValue).Is(value);
        }

        [TestMethod]
        public void TrySet_BeyondBounds_ReturnsFalse()
        {
            var result = _virtualMachineMemory.TrySet(Constants.MemoryCapacity, 5);

            Check.That(result).IsFalse();
        }

        [TestMethod]
        public void TrySet_WithinBounds_ReturnsTrue()
        {
            var result = _virtualMachineMemory.TrySet(Constants.MemoryCapacity - 1, 5);

            Check.That(result).IsTrue();
        }

        [TestMethod]
        public void TrySet_WithinBounds_SetsValue()
        {
            const ushort address = Constants.MemoryCapacity - 100;
            const byte value = 199;

            _virtualMachineMemory.TrySet(address, value);

            var memoryData = _virtualMachineMemory.MemoryData;

            Check.That(memoryData.ElementAt(address)).Is(value);
        }

        [TestMethod]
        public void TrySet_MemoryIsReadOnly_ReturnsTrue()
        {
            const ushort address = Constants.MemoryCapacity - 100;
            const byte value = 199;

            _virtualMachineMemory = new VirtualMachineMemory(true);

            var result = _virtualMachineMemory.TrySet(address, value);

            Check.That(result).IsTrue();
        }

        [TestMethod]
        public void TrySet_MemoryIsReadOnly_DoesNotSetValue()
        {
            const ushort address = Constants.MemoryCapacity - 100;
            const byte value = 199;

            _virtualMachineMemory = new VirtualMachineMemory(true);

            var result = _virtualMachineMemory.TrySet(address, value);

            var memoryData = _virtualMachineMemory.MemoryData;

            Check.That(memoryData.ElementAt(address)).IsNotEqualTo(value);
            Check.That(result).IsTrue();
        }

        [TestMethod]
        public void MemoryData_ReturnsFullContentsOfMemory()
        {
            var data = new byte[] { 1, 2, 3, 4 };

            _virtualMachineMemory.SetMemory(data);

            var memoryData = _virtualMachineMemory.MemoryData;

            Check.That(memoryData).CountIs(Constants.MemoryCapacity);
            Check.That(memoryData.Count(value => value is not 0)).Is(data.Length);
        }
    }
}
