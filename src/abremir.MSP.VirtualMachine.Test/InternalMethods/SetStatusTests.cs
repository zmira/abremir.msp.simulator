using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class SetStatusTests : VirtualMachineTestsBase
    {
        [Fact]
        public void SetStatus_StatusIsSame_DoesNotSetStatus()
        {
            const Status status = Status.None;

            Check.That(VirtualMachine.Status).Is(status);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<StatusChangedEventArgs>((virtualMachine, handler) => virtualMachine.StatusChanged += handler);

            VirtualMachine.SetStatus(status);

            Check.That(VirtualMachine.Status).Is(status);
            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [Fact]
        public void SetStatus_StatusIsDifferent_SetsStatus()
        {
            Check.That(VirtualMachine.Status).Is(Status.None);

            const Status status = Status.Running;

            VirtualMachine.SetStatus(status);

            Check.That(VirtualMachine.Status).Is(status);
        }

        [Fact]
        public void SetStatus_StatusIsDifferent_RaisesStatusChangedEventWithNewStatus()
        {
            Check.That(VirtualMachine.Status).Is(Status.None);

            const Status status = Status.Interrupted;

            var hook = EventHook.For(VirtualMachine)
                .Hook<StatusChangedEventArgs>((virtualMachine, handler) => virtualMachine.StatusChanged += handler)
                .Verify(eventArgs => Check.That(eventArgs.NewStatus).Is(status))
                .Build();

            VirtualMachine.SetStatus(status);

            Check.That(VirtualMachine.Status).Is(status);
            hook.Verify(Called.Once());
        }
    }
}
