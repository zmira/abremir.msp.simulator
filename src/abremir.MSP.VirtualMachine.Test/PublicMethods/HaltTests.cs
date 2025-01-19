using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    public class HaltTests : VirtualMachineTestsBase
    {
        [Fact]
        public void Halt_HaltsExecutionWithForceHaltReason()
        {
            var hook = EventHook.For(VirtualMachine)
                .Hook<VirtualMachineHaltedEventArgs>((virtualMachine, handler) => virtualMachine.VirtualMachineHalted += handler)
                .Verify(eventArgs => eventArgs.Reason.ShouldBe(Enums.HaltReason.ForceHalt))
                .Build();

            VirtualMachine.Halt();

            hook.Verify(Called.Once());
        }
    }
}
