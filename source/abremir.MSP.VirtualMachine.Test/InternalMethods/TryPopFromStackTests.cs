using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    [TestClass]
    public class TryPopFromStackTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void TryPopFromStack_Succeeds_ReturnsTrue()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>()).Returns(true);

            VirtualMachine = new VirtualMachineBuilder().WithStack(stack).Build();

            var result = VirtualMachine.TryPopFromStack(Operation.LessThan, out _);

            Check.That(result).IsTrue();
        }

        [TestMethod]
        public void TryPopFromStack_Succeeds_OutputsValue()
        {
            const byte value = 99;
            var program = new byte[] { (byte)Operation.PushValue, value };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            _ = VirtualMachine.TryPopFromStack(Operation.LessThan, out var poppedValue);

            Check.That(poppedValue).Is(value);
        }

        [TestMethod]
        public void TryPopFromStack_Succeeds_RaisesStackPointerUpdatedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>()).Returns(true);

            VirtualMachine = new VirtualMachineBuilder().WithStack(stack).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<ushort>((virtualMachine, handler) => virtualMachine.StackPointerUpdated += handler);

            _ = VirtualMachine.TryPopFromStack(Operation.LessThan, out _);

            hook.Verify(Called.Once());
        }

        [TestMethod]
        public void TryPopFromStack_Fails_ReturnsFalse()
        {
            var result = VirtualMachine.TryPopFromStack(Operation.LessThan, out _);

            Check.That(result).IsFalse();
        }

        [TestMethod]
        public void TryPopFromStack_Fails_HaltsWithStackEmpty()
        {
            VirtualMachine.TryPopFromStack(Operation.LessThan, out _);

            Check.That(VirtualMachine.Status).Is(Status.Halted);
            Check.That(VirtualMachine.HaltedBy).Is(HaltReason.StackEmpty);
        }
    }
}
