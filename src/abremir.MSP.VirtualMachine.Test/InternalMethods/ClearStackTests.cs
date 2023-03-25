using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class ClearStackTests : VirtualMachineTestsBase
    {
        [Fact]
        public void ClearStack_ResetsStack()
        {
            const byte value = 99;
            var program = new byte[] { (byte)Operation.PushValue, value };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            VirtualMachine.Stack.Should().NotBeEmpty();

            VirtualMachine.ClearStack();

            VirtualMachine.Stack.Should().BeEmpty();
        }

        [Fact]
        public void ClearStack_RaisesStackPointerUpdatedEvent()
        {
            var hook = EventHook.For(VirtualMachine)
                .HookOnly<ushort>((virtualMachine, handler) => virtualMachine.StackPointerUpdated += handler);

            VirtualMachine.ClearStack();

            hook.Verify(Called.Once());
        }
    }
}
