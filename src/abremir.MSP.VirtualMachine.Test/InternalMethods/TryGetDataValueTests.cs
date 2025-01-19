using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class TryGetDataValueTests : VirtualMachineTestsBase
    {
        [Fact]
        public void TryGetDataValue_Succeeds_ReturnsTrue()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>()).Returns(true);

            VirtualMachine = new VirtualMachineBuilder().WithDataMemory(memory).Build();

            var result = VirtualMachine.TryGetDataValue(Operation.LessThan, 100, out _);

            result.ShouldBeTrue();
        }

        [Fact]
        public void TryGetDataValue_Succeeds_OutputsValue()
        {
            const byte value = 99;
            var data = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, value };
            byte[] program = [];

            VirtualMachine.SetMemory(data, program);

            VirtualMachine.TryGetDataValue(Operation.LessThan, (ushort)(data.Length - 1), out var poppedValue);

            poppedValue.ShouldBe(value);
        }

        [Fact]
        public void TryGetDataValue_Fails_ReturnsFalse()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>()).Returns(false);

            VirtualMachine = new VirtualMachineBuilder().WithDataMemory(memory).Build();

            var result = VirtualMachine.TryGetDataValue(Operation.LessThan, 100, out _);

            result.ShouldBeFalse();
        }

        [Fact]
        public void TryGetDataValue_Fails_HaltsWithMemoryAddressViolation()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>()).Returns(false);

            VirtualMachine = new VirtualMachineBuilder().WithDataMemory(memory).Build();

            VirtualMachine.TryGetDataValue(Operation.LessThan, 100, out _);

            VirtualMachine.Status.ShouldBe(Status.Halted);
            VirtualMachine.HaltedBy.ShouldBe(HaltReason.MemoryAddressViolation);
        }
    }
}
