using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    [TestClass]
    public class TryPushToStackTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void TryPushToStack_Succeeds_ReturnsTrue()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>()).Returns(true);

            VirtualMachine = new VirtualMachineBuilder().WithStack(stack).Build();

            var result = VirtualMachine.TryPushToStack(Operation.LessThan, 5);

            Check.That(result).IsTrue();
        }

        [TestMethod]
        public void TryPushToStack_Succeeds_RaisesStackPointerUpdatedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>()).Returns(true);

            VirtualMachine = new VirtualMachineBuilder().WithStack(stack).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<ushort>((virtualMachine, handler) => virtualMachine.StackPointerUpdated += handler);

            _ = VirtualMachine.TryPushToStack(Operation.LessThan, 5);

            hook.Verify(Called.Once());
        }

        [TestMethod]
        public void TryPushToStack_Fails_ReturnsFalse()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>()).Returns(false);

            VirtualMachine = new VirtualMachineBuilder().WithStack(stack).Build();

            var result = VirtualMachine.TryPushToStack(Operation.LessThan, 5);

            Check.That(result).IsFalse();
        }

        [TestMethod]
        public void TryPushToStack_Fails_HaltsWithSTackFull()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>()).Returns(false);

            VirtualMachine = new VirtualMachineBuilder().WithStack(stack).Build();

            VirtualMachine.TryPushToStack(Operation.LessThan, 5);

            Check.That(VirtualMachine.Status).Is(Status.Halted);
            Check.That(VirtualMachine.HaltedBy).Is(HaltReason.StackFull);
        }
    }
}
