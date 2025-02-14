using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    [TestClass]
    public class RunTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void Run_StatusRunningAndModeRun_DoesNotExecuteNextInstruction()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Status.Running).WithMode(Mode.Run).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.Run();

            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [TestMethod]
        public void Run_NotStatusRunningAndModeRun_ExecutesNextInstruction()
        {
            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.Run();

            hook.Verify(Called.AtLeast(1));
        }
    }
}
