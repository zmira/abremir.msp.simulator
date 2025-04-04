using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    [TestClass]
    public class InterruptTests : VirtualMachineTestsBase
    {
        [TestMethod]
        [DataRow(Status.Interrupted)]
        [DataRow(Status.Halted)]
        [DataRow(Status.Suspended)]
        public void Interrupt_StatusInterrupted_DoesNotInterrupt(Status status)
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(status).Build();

            Check.That(VirtualMachine.Status).Is(status);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<StatusChangedEventArgs>((virtualMachine, handler) => virtualMachine.StatusChanged += handler);

            VirtualMachine.Interrupt(InterruptReason.InputCharacter);

            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [TestMethod]
        public void Interrupt_StatusNotInterrupted_SetsInterruptedBy()
        {
            Check.That(VirtualMachine.InterruptedBy).IsNull();

            const InterruptReason interruptedBy = InterruptReason.InputCharacter;

            VirtualMachine.Interrupt(interruptedBy);

            Check.That(VirtualMachine.InterruptedBy).Is(interruptedBy);
        }

        [TestMethod]
        public void Interrupt_StatusNotInterrupted_SetsStatusInterrupted()
        {
            Check.That(VirtualMachine.Status).IsNotEqualTo(Status.Interrupted);

            VirtualMachine.Interrupt(InterruptReason.InputCharacter);

            Check.That(VirtualMachine.Status).Is(Status.Interrupted);
        }
    }
}
