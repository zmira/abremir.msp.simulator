using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    public class InputTests : VirtualMachineTestsBase
    {
        [Fact]
        public void Input_StatusNotInterrupted_DoesNotPushValueToStack()
        {
            VirtualMachine.Input(1);

            VirtualMachine.Stack.Where(value => value != 0).Should().BeEmpty();
        }

        [Fact]
        public void Input_InterruptedByIsNotInputValue_DoesNotPushValueToStack()
        {
            var program = new[] { (byte)Operation.InputCharacter };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            VirtualMachine.InterruptedBy.Should().Be(Enums.InterruptReason.InputCharacter);

            VirtualMachine.Input(1);

            VirtualMachine.Stack.Where(value => value != 0).Should().BeEmpty();
        }

        [Fact]
        public void Input_PushesValueToStack()
        {
            var program = new[] { (byte)Operation.InputValue };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            VirtualMachine.Input(1);

            VirtualMachine.Stack.Where(value => value != 0).Should().NotBeEmpty();
        }

        [Fact]
        public void Input_ExecutesNextInstruction()
        {
            var program = new[] { (byte)Operation.InputValue, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.Input(1);

            hook.Verify(Called.Once());
        }
    }
}
