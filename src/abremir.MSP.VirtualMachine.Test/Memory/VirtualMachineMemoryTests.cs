using abremir.MSP.Shared.Constants;
using abremir.MSP.VirtualMachine.Memory;

namespace abremir.MSP.VirtualMachine.Test.Memory
{
    public class VirtualMachineMemoryTests
    {
        private IVirtualMachineMemory _virtualMachineMemory;

        public VirtualMachineMemoryTests()
        {
            _virtualMachineMemory = new VirtualMachineMemory();
        }

        [Fact]
        public void SetMemory_LessDataThanCapacity_CopiesAll()
        {
            var data = new byte[] { 1, 2, 3, 4 };

            _virtualMachineMemory.SetMemory(data);

            _virtualMachineMemory.MemoryData.Count(value => value is not 0).Should().Be(data.Length);
        }

        [Fact]
        public void SetMemory_MoreDataThanCapacity_OnlyCopiesEnoughToFillCapacity()
        {
            var data = new byte[Constants.MemoryCapacity + 1];
            Array.Fill<byte>(data, 1, 0, Constants.MemoryCapacity + 1);

            _virtualMachineMemory.SetMemory(data);

            var memoryData = _virtualMachineMemory.MemoryData;

            memoryData.Count(value => value is not 0).Should().NotBe(data.Length);
            memoryData.Count(value => value is not 0).Should().Be(Constants.MemoryCapacity);
        }

        [Fact]
        public void Clear_ResetsMemoryData()
        {
            var data = new byte[] { 1, 2, 3, 4 };

            _virtualMachineMemory.SetMemory(data);

            var memoryData = _virtualMachineMemory.MemoryData.Where(value => value is not 0).ToList();

            _virtualMachineMemory.Clear();

            var newMemoryData = _virtualMachineMemory.MemoryData;

            newMemoryData.Count(value => value is not 0).Should().NotBe(memoryData.Count);
            newMemoryData.All(value => value is 0).Should().BeTrue();
        }

        [Fact]
        public void TryGet_BeyondBounds_ReturnsFalse()
        {
            var result = _virtualMachineMemory.TryGet(Constants.MemoryCapacity, out _);

            result.Should().BeFalse();
        }

        [Fact]
        public void TryGet_AddressWithinBounds_ReturnsTrue()
        {
            var result = _virtualMachineMemory.TryGet(Constants.MemoryCapacity - 1, out _);

            result.Should().BeTrue();
        }

        [Fact]
        public void TryGet_AddressWithinBounds_OutputsValue()
        {
            const ushort address = 3;
            const byte value = 99;
            var data = new byte[] { 1, 2, 3, 4 };
            data[address] = value;

            _virtualMachineMemory.SetMemory(data);

            _virtualMachineMemory.TryGet(address, out var outputValue);

            outputValue.Should().Be(value);
        }

        [Fact]
        public void TrySet_BeyondBounds_ReturnsFalse()
        {
            var result = _virtualMachineMemory.TrySet(Constants.MemoryCapacity, 5);

            result.Should().BeFalse();
        }

        [Fact]
        public void TrySet_WithinBounds_ReturnsTrue()
        {
            var result = _virtualMachineMemory.TrySet(Constants.MemoryCapacity - 1, 5);

            result.Should().BeTrue();
        }

        [Fact]
        public void TrySet_WithinBounds_SetsValue()
        {
            const ushort address = Constants.MemoryCapacity - 100;
            const byte value = 199;

            _virtualMachineMemory.TrySet(address, value);

            var memoryData = _virtualMachineMemory.MemoryData;

            memoryData.ElementAt(address).Should().Be(value);
        }

        [Fact]
        public void TrySet_MemoryIsReadOnly_ReturnsTrue()
        {
            const ushort address = Constants.MemoryCapacity - 100;
            const byte value = 199;

            _virtualMachineMemory = new VirtualMachineMemory(true);

            var result = _virtualMachineMemory.TrySet(address, value);

            result.Should().BeTrue();
        }

        [Fact]
        public void TrySet_MemoryIsReadOnly_DoesNotSetValue()
        {
            const ushort address = Constants.MemoryCapacity - 100;
            const byte value = 199;

            _virtualMachineMemory = new VirtualMachineMemory(true);

            var result = _virtualMachineMemory.TrySet(address, value);

            var memoryData = _virtualMachineMemory.MemoryData;

            memoryData.ElementAt(address).Should().NotBe(value);
            result.Should().BeTrue();
        }

        [Fact]
        public void MemoryData_ReturnsFullContentsOfMemory()
        {
            var data = new byte[] { 1, 2, 3, 4 };

            _virtualMachineMemory.SetMemory(data);

            var memoryData = _virtualMachineMemory.MemoryData;

            memoryData.Count.Should().Be(Constants.MemoryCapacity);
            memoryData.Count(value => value is not 0).Should().Be(data.Length);
        }
    }
}
