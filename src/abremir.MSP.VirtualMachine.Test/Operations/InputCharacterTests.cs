using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    public class InputCharacterTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private const Operation _operation = Operation.InputCharacter;

        public InputCharacterTests()
        {
            _program = [(byte)_operation];

            VirtualMachine.SetMemory([], _program);
        }

        [Fact]
        public void ExecuteNextInstruction_InputCharacter_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_InputCharacter_SetsStatus()
        {
            ExecuteNextInstruction_Verify_SetsStatus(Status.Interrupted, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_InputCharacter_SetsInterruptedBy()
        {
            ExecuteNextInstruction_Verify_SetsInterruptedBy(InterruptReason.InputCharacter, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_InputCharacter_RaisesStatusChangedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Interrupted, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_InputCharacter_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_InputCharacter_RaisesInputRequestedEvent()
        {
            var hook = EventHook.For(VirtualMachine)
                .Hook<InputRequestedEventArgs>((virtualMachine, handler) => virtualMachine.InputRequested += handler)
                .Verify(eventArgs => Check.That(eventArgs.IsCharacter).IsTrue())
                .Build();

            VirtualMachine.ExecuteNextInstruction();

            hook.Verify(Called.Once());
        }

        [Fact]
        public void ExecuteNextInstruction_InputCharacter_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [Fact]
        public void ExecuteNextInstruction_InputCharacter_DoesNotUpdateProgramCounter()
        {
            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(program: _program);
        }
    }
}
