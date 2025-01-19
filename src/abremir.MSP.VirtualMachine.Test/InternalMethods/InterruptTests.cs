using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;
using EventTestingHelper = abremir.MSP.VirtualMachine.Test.Helpers.EventTestingHelper;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class InterruptTests : VirtualMachineTestsBase
    {
        [Theory]
        [InlineData(Status.Interrupted)]
        [InlineData(Status.Halted)]
        [InlineData(Status.Suspended)]
        public void Interrupt_StatusInterrupted_DoesNotInterrupt(Status status)
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(status).Build();

            VirtualMachine.Status.ShouldBe(status);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<StatusChangedEventArgs>((virtualMachine, handler) => virtualMachine.StatusChanged += handler);

            VirtualMachine.Interrupt(InterruptReason.InputCharacter);

            hook.Verify(EventTestingHelper.Called.Never());
        }

        [Fact]
        public void Interrupt_StatusNotInterrupted_SetsInterruptedBy()
        {
            VirtualMachine.InterruptedBy.ShouldBeNull();

            const InterruptReason interruptedBy = InterruptReason.InputCharacter;

            VirtualMachine.Interrupt(interruptedBy);

            VirtualMachine.InterruptedBy.ShouldBe(interruptedBy);
        }

        [Fact]
        public void Interrupt_StatusNotInterrupted_SetsStatusInterrupted()
        {
            VirtualMachine.Status.ShouldNotBe(Status.Interrupted);

            VirtualMachine.Interrupt(InterruptReason.InputCharacter);

            VirtualMachine.Status.ShouldBe(Status.Interrupted);
        }
    }
}
