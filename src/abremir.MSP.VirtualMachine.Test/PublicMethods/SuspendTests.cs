using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    public class SuspendTests : VirtualMachineTestsBase
    {
        [Fact]
        public void Suspend_StatusIsNotRunning_DoesNotUpdateStatusToSuspended()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Enums.Status.None).WithMode(Enums.Mode.Run).Build();

            VirtualMachine.Suspend();

            Check.That(VirtualMachine.Status).IsNotEqualTo(Enums.Status.Suspended);
        }

        [Fact]
        public void Suspend_ModeIsNotRun_DoesNotUpdateStatusToSuspended()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Enums.Status.Running).WithMode(Enums.Mode.Step).Build();

            VirtualMachine.Suspend();

            Check.That(VirtualMachine.Status).IsNotEqualTo(Enums.Status.Suspended);
        }

        [Fact]
        public void Suspend_UpdatesStatusToSuspended()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Enums.Status.Running).WithMode(Enums.Mode.Run).Build();

            VirtualMachine.Suspend();

            Check.That(VirtualMachine.Status).Is(Enums.Status.Suspended);
        }
    }
}
