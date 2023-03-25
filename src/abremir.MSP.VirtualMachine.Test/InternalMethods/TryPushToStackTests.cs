using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class TryPushToStackTests : VirtualMachineTestsBase
    {
        [Fact]
        public void TryPushToStack_Succeeds_ReturnsTrue()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>()).Returns(true);

            VirtualMachine = new VirtualMachineBuilder().WithStack(stack).Build();

            var result = VirtualMachine.TryPushToStack(Operation.LessThan, 5);

            result.Should().BeTrue();
        }

        [Fact]
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

        [Fact]
        public void TryPushToStack_Fails_ReturnsFalse()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>()).Returns(false);

            VirtualMachine = new VirtualMachineBuilder().WithStack(stack).Build();

            var result = VirtualMachine.TryPushToStack(Operation.LessThan, 5);

            result.Should().BeFalse();
        }

        [Fact]
        public void TryPushToStack_Fails_HaltsWithSTackFull()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>()).Returns(false);

            VirtualMachine = new VirtualMachineBuilder().WithStack(stack).Build();

            VirtualMachine.TryPushToStack(Operation.LessThan, 5);

            VirtualMachine.Status.Should().Be(Status.Halted);
            VirtualMachine.HaltedBy.Should().Be(HaltReason.StackFull);
        }
    }
}
