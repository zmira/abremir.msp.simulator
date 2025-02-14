using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    [TestClass]
    public class TrySetDataValueTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void TrySetDataValue_Succeeds_ReturnsTrue()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>()).Returns(true);

            VirtualMachine = new VirtualMachineBuilder().WithDataMemory(memory).Build();

            var result = VirtualMachine.TrySetDataValue(Operation.LessThan, 100, 5);

            Check.That(result).IsTrue();
        }

        [TestMethod]
        public void TrySetDataValue_Succeeds_RaisesDataMemoryUpdatedEvent()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>()).Returns(true);

            VirtualMachine = new VirtualMachineBuilder().WithDataMemory(memory).Build();

            const ushort address = 100;
            const byte value = 5;

            var hook = EventHook.For(VirtualMachine)
                .Hook<DataMemoryUpdatedEventArgs>((virtualMachine, handler) => virtualMachine.DataMemoryUpdated += handler)
                .Verify(eventArgs => Check.That(eventArgs.Address).Is(address))
                .Verify(eventArgs => Check.That(eventArgs.Value).Is(value))
                .Build();

            _ = VirtualMachine.TrySetDataValue(Operation.LessThan, address, value);

            hook.Verify(Called.Once());
        }

        [TestMethod]
        public void TrySetDataValue_Fails_ReturnsFalse()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>()).Returns(false);

            VirtualMachine = new VirtualMachineBuilder().WithDataMemory(memory).Build();

            var result = VirtualMachine.TrySetDataValue(Operation.LessThan, 100, 5);

            Check.That(result).IsFalse();
        }

        [TestMethod]
        public void TrySetDataValue_Fails_HaltsWithMemoryAddressViolation()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>()).Returns(false);

            VirtualMachine = new VirtualMachineBuilder().WithDataMemory(memory).Build();

            VirtualMachine.TrySetDataValue(Operation.LessThan, 100, 5);

            Check.That(VirtualMachine.Status).Is(Status.Halted);
            Check.That(VirtualMachine.HaltedBy).Is(HaltReason.MemoryAddressViolation);
        }
    }
}
