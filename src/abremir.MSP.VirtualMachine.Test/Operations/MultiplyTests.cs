﻿using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    public class MultiplyTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private const Operation _operation = Operation.Multiply;
        private const byte _operand1 = 8;
        private const byte _operand2 = 9;

        public MultiplyTests()
        {
            _program = new byte[] { (byte)_operation };

            VirtualMachine.SetMemory(Array.Empty<byte>(), _program);
            VirtualMachine.TryPushToStack(_operation, _operand1);
            VirtualMachine.TryPushToStack(_operation, _operand2);
        }

        [Fact]
        public void ExecuteNextInstruction_Multiply_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_Multiply_PushesResultToStack()
        {
            VirtualMachine.Stack.ElementAt(0).Should().Be(_operand2);
            VirtualMachine.Stack.ElementAt(1).Should().Be(_operand1);

            VirtualMachine.ExecuteNextInstruction();

            VirtualMachine.Stack.Count.Should().Be(1);
            VirtualMachine.Stack.ElementAt(0).Should().Be(_operand1 * _operand2);
        }

        [Fact]
        public void ExecuteNextInstruction_Multiply_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [Fact]
        public void ExecuteNextInstruction_Multiply_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_Multiply_UpdatesProgramCounter()
        {
            ExecuteNextInstruction_Verify_UpdatesProgramCounter(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_MultiplyFailsToPopFirstValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_MultiplyFailsToPopFirstValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_MultiplyFailsToPopFirstValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_MultiplyFailsToPopFirstValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_MultiplyFailsToPopFirstValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_MultiplyFailsToPopSecondValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_MultiplyFailsToPopSecondValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_MultiplyFailsToPopSecondValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_MultiplyFailsToPopSecondValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_MultiplyFailsToPopSecondValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [Theory]
        [InlineData(100, 10)]
        [InlineData(-100, 10)]
        public void ExecuteNextInstruction_MultiplyResultIsNotValid_SetsStatus(int operand1, int operand2)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    op2 => { op2[0] = operand2.ToTwosComplement(); return true; },
                    op1 => { op1[0] = operand1.ToTwosComplement(); return true; });

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Theory]
        [InlineData(100, 10, HaltReason.OverflowError)]
        [InlineData(-100, 10, HaltReason.UnderflowError)]
        public void ExecuteNextInstruction_MultiplyResultIsNotValid_SetsHaltedBy(int operand1, int operand2, HaltReason reason)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    op2 => { op2[0] = operand2.ToTwosComplement(); return true; },
                    op1 => { op1[0] = operand1.ToTwosComplement(); return true; });

            ExecuteNextInstruction_Verify_SetsHaltedBy(reason, stack: stack, program: _program);
        }

        [Theory]
        [InlineData(100, 10)]
        [InlineData(-100, 10)]
        public void ExecuteNextInstruction_MultiplyResultIsNotValid_RaisesStatusChangedEvent(int operand1, int operand2)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    op2 => { op2[0] = operand2.ToTwosComplement(); return true; },
                    op1 => { op1[0] = operand1.ToTwosComplement(); return true; });

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Theory]
        [InlineData(100, 10, HaltReason.OverflowError)]
        [InlineData(-100, 10, HaltReason.UnderflowError)]
        public void ExecuteNextInstruction_MultiplyResultIsNotValid_RaisesVirtualMachineHaltedEvent(int operand1, int operand2, HaltReason reason)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    op2 => { op2[0] = operand2.ToTwosComplement(); return true; },
                    op1 => { op1[0] = operand1.ToTwosComplement(); return true; });

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(reason, stack: stack, program: _program);
        }

        [Theory]
        [InlineData(100, 10)]
        [InlineData(-100, 10)]
        public void ExecuteNextInstruction_MultiplyResultIsNotValid_DoesNotUpdateProgramCounter(int operand1, int operand2)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    op2 => { op2[0] = operand2.ToTwosComplement(); return true; },
                    op1 => { op1[0] = operand1.ToTwosComplement(); return true; });

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_MultiplyFailsToPushValueToStack_SetsStatus()
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
        public void ExecuteNextInstruction_MultiplyFailsToPushValueToStack_SetsHaltedBy()
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
        public void ExecuteNextInstruction_MultiplyFailsToPushValueToStack_RaisesStatusChangedEvent()
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
        public void ExecuteNextInstruction_MultiplyFailsToPushValueToStack_RaisesVirtualMachineHaltedEvent()
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
        public void ExecuteNextInstruction_MultiplyFailsToPushValueToStack_DoesNotUpdateProgramCounter()
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
