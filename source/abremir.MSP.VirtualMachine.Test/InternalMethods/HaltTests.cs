using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    [TestClass]
    public class HaltTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void Halt_StatusHalted_DoesNotHalt()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Status.Halted).Build();

            Check.That(VirtualMachine.Status).Is(Status.Halted);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<VirtualMachineHaltedEventArgs>((virtualMachine, handler) => virtualMachine.VirtualMachineHalted += handler);

            VirtualMachine.Halt(null, null, HaltReason.HaltInstruction);

            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [TestMethod]
        public void Halt_StatusNotHalted_SetsHaltedBy()
        {
            Check.That(VirtualMachine.HaltedBy).IsNull();

            const HaltReason haltedBy = HaltReason.HaltInstruction;

            VirtualMachine.Halt(null, null, haltedBy);

            Check.That(VirtualMachine.HaltedBy).Is(haltedBy);
        }

        [TestMethod]
        public void Halt_StatusNotHalted_SetsStatusHalted()
        {
            Check.That(VirtualMachine.Status).IsNotEqualTo(Status.Halted);

            VirtualMachine.Halt(null, null, HaltReason.HaltInstruction);

            Check.That(VirtualMachine.Status).Is(Status.Halted);
        }

        [TestMethod]
        public void Halt_StatusNotHalted_RaisesVirtualMachineHaltedEventWithReason()
        {
            const HaltReason reason = HaltReason.HaltInstruction;

            var hook = EventHook.For(VirtualMachine)
                .Hook<VirtualMachineHaltedEventArgs>((virtualMachine, handler) => virtualMachine.VirtualMachineHalted += handler)
                .Verify(eventArgs => Check.That(eventArgs.Reason).Is(reason))
                .Build();

            VirtualMachine.Halt(null, null, reason);

            hook.Verify(Called.Once());
        }
    }
}
