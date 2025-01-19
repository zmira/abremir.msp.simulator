using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class TrySetDataValueTests : VirtualMachineTestsBase
    {
        [Fact]
        public void TrySetDataValue_Succeeds_ReturnsTrue()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>()).Returns(true);

            VirtualMachine = new VirtualMachineBuilder().WithDataMemory(memory).Build();

            var result = VirtualMachine.TrySetDataValue(Operation.LessThan, 100, 5);

            result.ShouldBeTrue();
        }

        [Fact]
        public void TrySetDataValue_Succeeds_RaisesDataMemoryUpdatedEvent()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>()).Returns(true);

            VirtualMachine = new VirtualMachineBuilder().WithDataMemory(memory).Build();

            const ushort address = 100;
            const byte value = 5;

            var hook = EventHook.For(VirtualMachine)
                .Hook<DataMemoryUpdatedEventArgs>((virtualMachine, handler) => virtualMachine.DataMemoryUpdated += handler)
                .Verify(eventArgs => eventArgs.Address.ShouldBe(address))
                .Verify(eventArgs => eventArgs.Value.ShouldBe(value))
                .Build();

            _ = VirtualMachine.TrySetDataValue(Operation.LessThan, address, value);

            hook.Verify(Called.Once());
        }

        [Fact]
        public void TrySetDataValue_Fails_ReturnsFalse()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>()).Returns(false);

            VirtualMachine = new VirtualMachineBuilder().WithDataMemory(memory).Build();

            var result = VirtualMachine.TrySetDataValue(Operation.LessThan, 100, 5);

            result.ShouldBeFalse();
        }

        [Fact]
        public void TrySetDataValue_Fails_HaltsWithMemoryAddressViolation()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>()).Returns(false);

            VirtualMachine = new VirtualMachineBuilder().WithDataMemory(memory).Build();

            VirtualMachine.TrySetDataValue(Operation.LessThan, 100, 5);

            VirtualMachine.Status.ShouldBe(Status.Halted);
            VirtualMachine.HaltedBy.ShouldBe(HaltReason.MemoryAddressViolation);
        }
    }
}
