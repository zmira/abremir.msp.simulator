using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    [TestClass]
    public class RaiseInputRequestedEventTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void RaiseInputRequestedEvent_StatusNotInterrupted_DoesNotRaiseInputRequestedEvent()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Status.Running).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InputRequestedEventArgs>((virtualMachine, handler) => virtualMachine.InputRequested += handler);

            _ = VirtualMachine.RaiseInputRequestedEvent();

            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [TestMethod]
        public void RaiseInputRequestedEvent_StatusNotInterrupted_ReturnsFalse()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Status.Running).Build();

            var result = VirtualMachine.RaiseInputRequestedEvent();

            Check.That(result).IsFalse();
        }

        [TestMethod]
        public void RaiseInputRequestedEvent_InterruptedByIsNull_DoesNotRaiseInputRequestedEvent()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Status.Interrupted).Build();

            Check.That(VirtualMachine.HaltedBy).IsNull();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InputRequestedEventArgs>((virtualMachine, handler) => virtualMachine.InputRequested += handler);

            _ = VirtualMachine.RaiseInputRequestedEvent();

            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [TestMethod]
        public void RaiseInputRequestedEvent_InterruptedByIsNull_ReturnsFalse()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Status.Interrupted).Build();

            Check.That(VirtualMachine.HaltedBy).IsNull();

            var result = VirtualMachine.RaiseInputRequestedEvent();

            Check.That(result).IsFalse();
        }

        [TestMethod]
        [DataRow(InterruptReason.InputValue, false)]
        [DataRow(InterruptReason.InputCharacter, true)]
        public void RaiseInputRequestedEvent_StatusInterruptedAndInterruptedByNotNull_RaisesInputRequestedEvent(InterruptReason interruptReason, bool expectedIsCharacterFlag)
        {
            VirtualMachine.Interrupt(interruptReason);

            var hook = EventHook.For(VirtualMachine)
                .Hook<InputRequestedEventArgs>((virtualMachine, handler) => virtualMachine.InputRequested += handler)
                .Verify(eventArgs => Check.That(eventArgs.IsCharacter).Is(expectedIsCharacterFlag))
                .Build();

            _ = VirtualMachine.RaiseInputRequestedEvent();
        }

        [TestMethod]
        [DataRow(InterruptReason.InputValue)]
        [DataRow(InterruptReason.InputCharacter)]
        public void RaiseInputRequestedEvent_StatusInterruptedAndInterruptedByNotNull_ReturnsTrue(InterruptReason interruptReason)
        {
            VirtualMachine.Interrupt(interruptReason);

            var result = VirtualMachine.RaiseInputRequestedEvent();

            Check.That(result).IsTrue();
        }
    }
}
