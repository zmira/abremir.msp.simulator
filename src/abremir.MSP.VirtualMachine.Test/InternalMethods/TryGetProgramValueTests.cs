using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    [TestClass]
    public class TryGetProgramValueTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void TryGetProgramValue_Succeeds_ReturnsTrue()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>()).Returns(true);

            VirtualMachine = new VirtualMachineBuilder().WithProgramMemory(memory).Build();

            var result = VirtualMachine.TryGetProgramValue(Operation.LessThan, 100, out _);

            Check.That(result).IsTrue();
        }

        [TestMethod]
        public void TryGetProgramValue_Succeeds_OutputsValue()
        {
            const Operation operation = Operation.Jump;
            byte[] data = [];
            byte[] program = [0, 0, 0, 0, 0, 0, 0, 0, 0, (byte)operation];

            VirtualMachine.SetMemory(data, program);

            VirtualMachine.TryGetProgramValue(Operation.LessThan, (ushort)(program.Length - 1), out var poppedValue);

            Check.That(poppedValue).Is((byte)operation);
        }

        [TestMethod]
        public void TryGetProgramValue_Fails_ReturnsFalse()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>()).Returns(false);

            VirtualMachine = new VirtualMachineBuilder().WithProgramMemory(memory).Build();

            var result = VirtualMachine.TryGetProgramValue(Operation.LessThan, 100, out _);

            Check.That(result).IsFalse();
        }

        [TestMethod]
        public void TryGetProgramValue_Fails_HaltsWithMemoryAddressViolation()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>()).Returns(false);

            VirtualMachine = new VirtualMachineBuilder().WithProgramMemory(memory).Build();

            VirtualMachine.TryGetProgramValue(Operation.LessThan, 100, out _);

            Check.That(VirtualMachine.Status).Is(Status.Halted);
            Check.That(VirtualMachine.HaltedBy).Is(HaltReason.MemoryAddressViolation);
        }
    }
}
