using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class TryPopFromStackTests : VirtualMachineTestsBase
    {
        [Fact]
        public void TryPopFromStack_Succeeds_ReturnsTrue()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>()).Returns(true);

            VirtualMachine = new VirtualMachineBuilder().WithStack(stack).Build();

            var result = VirtualMachine.TryPopFromStack(Operation.LessThan, out _);

            result.ShouldBeTrue();
        }

        [Fact]
        public void TryPopFromStack_Succeeds_OutputsValue()
        {
            const byte value = 99;
            var program = new byte[] { (byte)Operation.PushValue, value };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            _ = VirtualMachine.TryPopFromStack(Operation.LessThan, out var poppedValue);

            poppedValue.ShouldBe(value);
        }

        [Fact]
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

        [Fact]
        public void TryPopFromStack_Fails_ReturnsFalse()
        {
            var result = VirtualMachine.TryPopFromStack(Operation.LessThan, out _);

            result.ShouldBeFalse();
        }

        [Fact]
        public void TryPopFromStack_Fails_HaltsWithStackEmpty()
        {
            VirtualMachine.TryPopFromStack(Operation.LessThan, out _);

            VirtualMachine.Status.ShouldBe(Status.Halted);
            VirtualMachine.HaltedBy.ShouldBe(HaltReason.StackEmpty);
        }
    }
}
