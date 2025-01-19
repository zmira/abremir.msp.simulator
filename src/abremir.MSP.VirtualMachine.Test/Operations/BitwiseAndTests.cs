using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    public class BitwiseAndTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private const Operation _operation = Operation.BitwiseAnd;
        private const byte _operand1 = 1;
        private const byte _operand2 = 1;

        public BitwiseAndTests()
        {
            _program = [(byte)_operation];

            VirtualMachine.SetMemory([], _program);
            VirtualMachine.TryPushToStack(_operation, _operand1);
            VirtualMachine.TryPushToStack(_operation, _operand2);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAnd_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [Theory]
        [InlineData(0b0001, 0b0001, 0b0001)]
        [InlineData(0b1010, 0b0101, 0b0000)]
        [InlineData(0b1011, 0b0010, 0b0010)]
        [InlineData(0b0000, 0b0000, 0b0000)]
        public void ExecuteNextInstruction_BitwiseAnd_PushesResultToStack(byte operand1, byte operand2, byte expectedResult)
        {
            while (VirtualMachine.Stack.Count != 0)
            {
                VirtualMachine.TryPopFromStack(Operation.BitwiseAnd, out _);
            }

            VirtualMachine.TryPushToStack(Operation.BitwiseAnd, operand1);
            VirtualMachine.TryPushToStack(Operation.BitwiseAnd, operand2);

            VirtualMachine.ExecuteNextInstruction();

            VirtualMachine.Stack.Count.ShouldBe(1);
            VirtualMachine.Stack.ElementAt(0).ShouldBe(expectedResult);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAnd_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAnd_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAnd_UpdatesProgramCounter()
        {
            ExecuteNextInstruction_Verify_UpdatesProgramCounter(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPopFirstValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPopFirstValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPopFirstValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPopFirstValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPopFirstValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPopSecondValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPopSecondValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPopSecondValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPopSecondValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPopSecondValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPushValueToStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    operand1 => { operand1[0] = _operand1; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPushValueToStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    operand1 => { operand1[0] = _operand1; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackFull, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPushValueToStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    operand1 => { operand1[0] = _operand1; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPushValueToStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    operand1 => { operand1[0] = _operand1; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackFull, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_BitwiseAndFailsToPushValueToStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    operand1 => { operand1[0] = _operand1; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }
    }
}
