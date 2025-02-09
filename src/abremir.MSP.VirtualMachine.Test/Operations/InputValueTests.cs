using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    public class InputValueTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private const Operation _operation = Operation.InputValue;

        public InputValueTests()
        {
            _program = [(byte)_operation];

            VirtualMachine.SetMemory([], _program);
        }

        [Fact]
        public void ExecuteNextInstruction_InputValue_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_InputValue_SetsStatus()
        {
            ExecuteNextInstruction_Verify_SetsStatus(Status.Interrupted, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_InputValue_SetsInterruptedBy()
        {
            ExecuteNextInstruction_Verify_SetsInterruptedBy(InterruptReason.InputValue, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_InputValue_RaisesStatusChangedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Interrupted, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_InputValue_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_InputValue_RaisesInputRequestedEvent()
        {
            var hook = EventHook.For(VirtualMachine)
                .Hook<InputRequestedEventArgs>((virtualMachine, handler) => virtualMachine.InputRequested += handler)
                .Verify(eventArgs => Check.That(eventArgs.IsCharacter).IsFalse())
                .Build();

            VirtualMachine.ExecuteNextInstruction();

            hook.Verify(Called.Once());
        }

        [Fact]
        public void ExecuteNextInstruction_InputValue_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [Fact]
        public void ExecuteNextInstruction_InputValue_DoesNotUpdateProgramCounter()
        {
            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(program: _program);
        }
    }
}
