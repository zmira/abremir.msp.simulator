using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    public class OutputValueTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private const Operation _operation = Operation.OutputValue;
        private const byte _value = 200;

        public OutputValueTests()
        {
            _program = [(byte)_operation];

            VirtualMachine.SetMemory([], _program);
            VirtualMachine.TryPushToStack(_operation, _value);
        }

        [Fact]
        public void ExecuteNextInstruction_OutputValue_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_OutputValue_RaisesOutputEmittedEvent()
        {
            var hook = EventHook.For(VirtualMachine)
                .Hook<OutputEmittedEventArgs>((virtualMachine, handler) => virtualMachine.OutputEmitted += handler)
                .Verify(eventArgs => eventArgs.IsCharacter.Should().BeFalse())
                .Verify(eventArgs => eventArgs.Value.Should().Be(_value.FromTwosComplement()))
                .Build();

            VirtualMachine.ExecuteNextInstruction();

            hook.Verify(Called.Once());
        }

        [Fact]
        public void ExecuteNextInstruction_OutputValue_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [Fact]
        public void ExecuteNextInstruction_OutputValue_PopsValueFromStack()
        {
            VirtualMachine.Stack.Should().NotBeEmpty();

            VirtualMachine.ExecuteNextInstruction();

            VirtualMachine.Stack.Should().BeEmpty();
        }

        [Fact]
        public void ExecuteNextInstruction_OutputValue_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_OutputValue_UpdatesProgramCounter()
        {
            ExecuteNextInstruction_Verify_UpdatesProgramCounter(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_OutputValueFailsToPopValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_OutputValueFailsToPopValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_OutputValueFailsToPopValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_OutputValueFailsToPopValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_OutputValueFailsToPopValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }
    }
}
