using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.PublicMethods
{
    [TestClass]
    public class InputCharacterTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void InputCharacter_StatusNotInterrupted_DoesNotPushValueToStack()
        {
            VirtualMachine.InputCharacter(65);

            Check.That(VirtualMachine.Stack.Where(value => value != 0)).IsEmpty();
        }

        [TestMethod]
        public void InputCharacter_InterruptedByIsNotInputCharacter_DoesNotPushValueToStack()
        {
            byte[] program = [(byte)Operation.InputValue];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            Check.That(VirtualMachine.InterruptedBy).Is(Enums.InterruptReason.InputValue);

            VirtualMachine.InputCharacter(65);

            Check.That(VirtualMachine.Stack.Where(value => value != 0)).IsEmpty();
        }

        [TestMethod]
        public void InputCharacter_PushesValueToStack()
        {
            byte[] program = [(byte)Operation.InputCharacter];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            VirtualMachine.InputCharacter(65);

            Check.That(VirtualMachine.Stack.Where(value => value != 0)).Not.IsEmpty();
        }

        [TestMethod]
        public void InputCharacter_ExecutesNextInstruction()
        {
            byte[] program = [(byte)Operation.InputCharacter, (byte)Operation.Halt];

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.InputCharacter(65);

            hook.Verify(Called.Once());
        }
    }
}
