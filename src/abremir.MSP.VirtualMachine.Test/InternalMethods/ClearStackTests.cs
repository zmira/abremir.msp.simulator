using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    [TestClass]
    public class ClearStackTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void ClearStack_ResetsStack()
        {
            const byte value = 99;
            var program = new byte[] { (byte)Operation.PushValue, value };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            Check.That(VirtualMachine.Stack).Not.IsEmpty();

            VirtualMachine.ClearStack();

            Check.That(VirtualMachine.Stack).IsEmpty();
        }

        [TestMethod]
        public void ClearStack_RaisesStackPointerUpdatedEvent()
        {
            var hook = EventHook.For(VirtualMachine)
                .HookOnly<ushort>((virtualMachine, handler) => virtualMachine.StackPointerUpdated += handler);

            VirtualMachine.ClearStack();

            hook.Verify(Called.Once());
        }
    }
}
