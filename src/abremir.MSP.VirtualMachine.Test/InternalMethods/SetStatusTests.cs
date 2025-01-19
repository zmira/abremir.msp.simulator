using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class SetStatusTests : VirtualMachineTestsBase
    {
        [Fact]
        public void SetStatus_StatusIsSame_DoesNotSetStatus()
        {
            const Status status = Status.None;

            VirtualMachine.Status.ShouldBe(status);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<StatusChangedEventArgs>((virtualMachine, handler) => virtualMachine.StatusChanged += handler);

            VirtualMachine.SetStatus(status);

            VirtualMachine.Status.ShouldBe(status);
            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [Fact]
        public void SetStatus_StatusIsDifferent_SetsStatus()
        {
            VirtualMachine.Status.ShouldBe(Status.None);

            const Status status = Status.Running;

            VirtualMachine.SetStatus(status);

            VirtualMachine.Status.ShouldBe(status);
        }

        [Fact]
        public void SetStatus_StatusIsDifferent_RaisesStatusChangedEventWithNewStatus()
        {
            VirtualMachine.Status.ShouldBe(Status.None);

            const Status status = Status.Interrupted;

            var hook = EventHook.For(VirtualMachine)
                .Hook<StatusChangedEventArgs>((virtualMachine, handler) => virtualMachine.StatusChanged += handler)
                .Verify(eventArgs => eventArgs.NewStatus.ShouldBe(status))
                .Build();

            VirtualMachine.SetStatus(status);

            VirtualMachine.Status.ShouldBe(status);
            hook.Verify(Called.Once());
        }
    }
}
