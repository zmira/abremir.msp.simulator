using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    public class InputCharacterTests : VirtualMachineTestsBase
    {
        [Fact]
        public void InputCharacter_StatusNotInterrupted_DoesNotPushValueToStack()
        {
            VirtualMachine.InputCharacter(65);

            VirtualMachine.Stack.Where(value => value != 0).Should().BeEmpty();
        }

        [Fact]
        public void InputCharacter_InterruptedByIsNotInputCharacter_DoesNotPushValueToStack()
        {
            var program = new[] { (byte)Operation.InputValue };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            VirtualMachine.InterruptedBy.Should().Be(Enums.InterruptReason.InputValue);

            VirtualMachine.InputCharacter(65);

            VirtualMachine.Stack.Where(value => value != 0).Should().BeEmpty();
        }

        [Fact]
        public void InputCharacter_PushesValueToStack()
        {
            var program = new[] { (byte)Operation.InputCharacter };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            VirtualMachine.InputCharacter(65);

            VirtualMachine.Stack.Where(value => value != 0).Should().NotBeEmpty();
        }

        [Fact]
        public void InputCharacter_ExecutesNextInstruction()
        {
            var program = new[] { (byte)Operation.InputCharacter, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.InputCharacter(65);

            hook.Verify(Called.Once());
        }
    }
}
