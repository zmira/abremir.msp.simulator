using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    [TestClass]
    public class SetModeTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void SetMode_ModeIsSame_DoesNotSetMode()
        {
            const Mode mode = Mode.None;

            Check.That(VirtualMachine.Mode).Is(mode);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<ModeChangedEventArgs>((virtualMachine, handler) => virtualMachine.ModeChanged += handler);

            VirtualMachine.SetMode(mode);

            Check.That(VirtualMachine.Mode).Is(mode);
            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [TestMethod]
        public void SetMode_ModeIsDifferent_SetsMode()
        {
            Check.That(VirtualMachine.Mode).Is(Mode.None);

            const Mode mode = Mode.Run;

            VirtualMachine.SetMode(mode);

            Check.That(VirtualMachine.Mode).Is(mode);
        }

        [TestMethod]
        public void SetMode_ModeIsDifferent_RaisesModeChangedEventWithNewMode()
        {
            Check.That(VirtualMachine.Mode).Is(Mode.None);

            const Mode mode = Mode.Step;

            var hook = EventHook.For(VirtualMachine)
                .Hook<ModeChangedEventArgs>((virtualMachine, handler) => virtualMachine.ModeChanged += handler)
                .Verify(eventArgs => Check.That(eventArgs.NewMode).Is(mode))
                .Build();

            VirtualMachine.SetMode(mode);

            Check.That(VirtualMachine.Mode).Is(mode);
            hook.Verify(Called.Once());
        }
    }
}
