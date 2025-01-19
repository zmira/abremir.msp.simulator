using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class HaltTests : VirtualMachineTestsBase
    {
        [Fact]
        public void Halt_StatusHalted_DoesNotHalt()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Status.Halted).Build();

            VirtualMachine.Status.ShouldBe(Status.Halted);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<VirtualMachineHaltedEventArgs>((virtualMachine, handler) => virtualMachine.VirtualMachineHalted += handler);

            VirtualMachine.Halt(null, null, HaltReason.HaltInstruction);

            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [Fact]
        public void Halt_StatusNotHalted_SetsHaltedBy()
        {
            VirtualMachine.HaltedBy.ShouldBeNull();

            const HaltReason haltedBy = HaltReason.HaltInstruction;

            VirtualMachine.Halt(null, null, haltedBy);

            VirtualMachine.HaltedBy.ShouldBe(haltedBy);
        }

        [Fact]
        public void Halt_StatusNotHalted_SetsStatusHalted()
        {
            VirtualMachine.Status.ShouldNotBe(Status.Halted);

            VirtualMachine.Halt(null, null, HaltReason.HaltInstruction);

            VirtualMachine.Status.ShouldBe(Status.Halted);
        }

        [Fact]
        public void Halt_StatusNotHalted_RaisesVirtualMachineHaltedEventWithReason()
        {
            const HaltReason reason = HaltReason.HaltInstruction;

            var hook = EventHook.For(VirtualMachine)
                .Hook<VirtualMachineHaltedEventArgs>((virtualMachine, handler) => virtualMachine.VirtualMachineHalted += handler)
                .Verify(eventArgs => eventArgs.Reason.ShouldBe(reason))
                .Build();

            VirtualMachine.Halt(null, null, reason);

            hook.Verify(Called.Once());
        }
    }
}
