using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class RaiseInputRequestedEventTests : VirtualMachineTestsBase
    {
        [Fact]
        public void RaiseInputRequestedEvent_StatusNotInterrupted_DoesNotRaiseInputRequestedEvent()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Status.Running).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InputRequestedEventArgs>((virtualMachine, handler) => virtualMachine.InputRequested += handler);

            _ = VirtualMachine.RaiseInputRequestedEvent();

            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [Fact]
        public void RaiseInputRequestedEvent_StatusNotInterrupted_ReturnsFalse()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Status.Running).Build();

            var result = VirtualMachine.RaiseInputRequestedEvent();

            result.Should().BeFalse();
        }

        [Fact]
        public void RaiseInputRequestedEvent_InterruptedByIsNull_DoesNotRaiseInputRequestedEvent()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Status.Interrupted).Build();

            VirtualMachine.HaltedBy.Should().BeNull();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InputRequestedEventArgs>((virtualMachine, handler) => virtualMachine.InputRequested += handler);

            _ = VirtualMachine.RaiseInputRequestedEvent();

            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [Fact]
        public void RaiseInputRequestedEvent_InterruptedByIsNull_ReturnsFalse()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Status.Interrupted).Build();

            VirtualMachine.HaltedBy.Should().BeNull();

            var result = VirtualMachine.RaiseInputRequestedEvent();

            result.Should().BeFalse();
        }

        [Theory]
        [InlineData(InterruptReason.InputValue, false)]
        [InlineData(InterruptReason.InputCharacter, true)]
        public void RaiseInputRequestedEvent_StatusInterruptedAndInterruptedByNotNull_RaisesInputRequestedEvent(InterruptReason interruptReason, bool expectedIsCharacterFlag)
        {
            VirtualMachine.Interrupt(interruptReason);

            var hook = EventHook.For(VirtualMachine)
                .Hook<InputRequestedEventArgs>((virtualMachine, handler) => virtualMachine.InputRequested += handler)
                .Verify(eventArgs => eventArgs.IsCharacter.Should().Be(expectedIsCharacterFlag))
                .Build();

            _ = VirtualMachine.RaiseInputRequestedEvent();
        }

        [Theory]
        [InlineData(InterruptReason.InputValue)]
        [InlineData(InterruptReason.InputCharacter)]
        public void RaiseInputRequestedEvent_StatusInterruptedAndInterruptedByNotNull_ReturnsTrue(InterruptReason interruptReason)
        {
            VirtualMachine.Interrupt(interruptReason);

            var result = VirtualMachine.RaiseInputRequestedEvent();

            result.Should().BeTrue();
        }
    }
}
