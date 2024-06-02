using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class TryGetProgramValueTests : VirtualMachineTestsBase
    {
        [Fact]
        public void TryGetProgramValue_Succeeds_ReturnsTrue()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>()).Returns(true);

            VirtualMachine = new VirtualMachineBuilder().WithProgramMemory(memory).Build();

            var result = VirtualMachine.TryGetProgramValue(Operation.LessThan, 100, out _);

            result.Should().BeTrue();
        }

        [Fact]
        public void TryGetProgramValue_Succeeds_OutputsValue()
        {
            const Operation operation = Operation.Jump;
            byte[] data = [];
            byte[] program = [0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)operation];

            VirtualMachine.SetMemory(data, program);

            VirtualMachine.TryGetProgramValue(Operation.LessThan, (ushort)(program.Length - 1), out var poppedValue);

            poppedValue.Should().Be((byte)operation);
        }

        [Fact]
        public void TryGetProgramValue_Fails_ReturnsFalse()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>()).Returns(false);

            VirtualMachine = new VirtualMachineBuilder().WithProgramMemory(memory).Build();

            var result = VirtualMachine.TryGetProgramValue(Operation.LessThan, 100, out _);

            result.Should().BeFalse();
        }

        [Fact]
        public void TryGetProgramValue_Fails_HaltsWithMemoryAddressViolation()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>()).Returns(false);

            VirtualMachine = new VirtualMachineBuilder().WithProgramMemory(memory).Build();

            VirtualMachine.TryGetProgramValue(Operation.LessThan, 100, out _);

            VirtualMachine.Status.Should().Be(Status.Halted);
            VirtualMachine.HaltedBy.Should().Be(HaltReason.MemoryAddressViolation);
        }
    }
}
