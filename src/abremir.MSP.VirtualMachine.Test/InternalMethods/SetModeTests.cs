using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;
using EventTestingHelper = abremir.MSP.VirtualMachine.Test.Helpers.EventTestingHelper;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class SetModeTests : VirtualMachineTestsBase
    {
        [Fact]
        public void SetMode_ModeIsSame_DoesNotSetMode()
        {
            const Mode mode = Mode.None;

            VirtualMachine.Mode.ShouldBe(mode);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<ModeChangedEventArgs>((virtualMachine, handler) => virtualMachine.ModeChanged += handler);

            VirtualMachine.SetMode(mode);

            VirtualMachine.Mode.ShouldBe(mode);
            hook.Verify(EventTestingHelper.Called.Never());
        }

        [Fact]
        public void SetMode_ModeIsDifferent_SetsMode()
        {
            VirtualMachine.Mode.ShouldBe(Mode.None);

            const Mode mode = Mode.Run;

            VirtualMachine.SetMode(mode);

            VirtualMachine.Mode.ShouldBe(mode);
        }

        [Fact]
        public void SetMode_ModeIsDifferent_RaisesModeChangedEventWithNewMode()
        {
            VirtualMachine.Mode.ShouldBe(Mode.None);

            const Mode mode = Mode.Step;

            var hook = EventHook.For(VirtualMachine)
                .Hook<ModeChangedEventArgs>((virtualMachine, handler) => virtualMachine.ModeChanged += handler)
                .Verify(eventArgs => eventArgs.NewMode.ShouldBe(mode))
                .Build();

            VirtualMachine.SetMode(mode);

            VirtualMachine.Mode.ShouldBe(mode);
            hook.Verify(Called.Once());
        }
    }
}
