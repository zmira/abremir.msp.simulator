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

            VirtualMachine.Stack.Where(value => value != 0).ShouldBeEmpty();
        }

        [Fact]
        public void Input_InterruptedByIsNotInputValue_DoesNotPushValueToStack()
        {
            byte[] program = [(byte)Operation.InputCharacter];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            VirtualMachine.InterruptedBy.ShouldBe(Enums.InterruptReason.InputCharacter);

            VirtualMachine.Input(1);

            VirtualMachine.Stack.Where(value => value != 0).ShouldBeEmpty();
        }

        [Fact]
        public void Input_PushesValueToStack()
        {
            byte[] program = [(byte)Operation.InputValue];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            VirtualMachine.Input(1);

            VirtualMachine.Stack.Where(value => value != 0).ShouldNotBeEmpty();
        }

        [Fact]
        public void Input_ExecutesNextInstruction()
        {
            byte[] program = [(byte)Operation.InputValue, (byte)Operation.Halt];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.Input(1);

            hook.Verify(Called.Once());
        }
    }
}
