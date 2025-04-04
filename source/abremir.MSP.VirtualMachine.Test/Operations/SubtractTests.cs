using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    [TestClass]
    public class SubtractTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private const Operation _operation = Operation.Subtract;
        private const byte _operand1 = 22;
        private const byte _operand2 = 11;

        public SubtractTests()
        {
            _program = [(byte)_operation];

            VirtualMachine.SetMemory([], _program);
            VirtualMachine.TryPushToStack(_operation, _operand1);
            VirtualMachine.TryPushToStack(_operation, _operand2);
        }

        [TestMethod]
        public void ExecuteNextInstruction_Subtract_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [TestMethod]
        public void ExecuteNextInstruction_Subtract_PushesResultToStack()
        {
            Check.That(VirtualMachine.Stack.ElementAt(0)).Is(_operand2);
            Check.That(VirtualMachine.Stack.ElementAt(1)).Is(_operand1);

            VirtualMachine.ExecuteNextInstruction();

            Check.That(VirtualMachine.Stack).CountIs(1);
            Check.That(VirtualMachine.Stack.ElementAt(0)).Is((byte)(_operand1 - _operand2));
        }

        [TestMethod]
        public void ExecuteNextInstruction_Subtract_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [TestMethod]
        public void ExecuteNextInstruction_Subtract_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [TestMethod]
        public void ExecuteNextInstruction_Subtract_UpdatesProgramCounter()
        {
            ExecuteNextInstruction_Verify_UpdatesProgramCounter(_operation);
        }

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPopFirstValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPopFirstValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPopFirstValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPopFirstValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPopFirstValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPopSecondValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPopSecondValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPopSecondValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPopSecondValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPopSecondValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
            .Returns(
                    operand2 => { operand2[0] = _operand2; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [TestMethod]
        [DataRow(100, -100)]
        [DataRow(-100, 100)]
        public void ExecuteNextInstruction_SubtractResultIsNotValid_SetsStatus(int operand1, int operand2)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    op2 => { op2[0] = operand2.ToTwosComplement(); return true; },
                    op1 => { op1[0] = operand1.ToTwosComplement(); return true; });

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [TestMethod]
        [DataRow(100, -100, HaltReason.OverflowError)]
        [DataRow(-100, 100, HaltReason.UnderflowError)]
        public void ExecuteNextInstruction_SubtractResultIsNotValid_SetsHaltedBy(int operand1, int operand2, HaltReason reason)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    op2 => { op2[0] = operand2.ToTwosComplement(); return true; },
                    op1 => { op1[0] = operand1.ToTwosComplement(); return true; });

            ExecuteNextInstruction_Verify_SetsHaltedBy(reason, stack: stack, program: _program);
        }

        [TestMethod]
        [DataRow(100, -100)]
        [DataRow(-100, 100)]
        public void ExecuteNextInstruction_SubtractResultIsNotValid_RaisesStatusChangedEvent(int operand1, int operand2)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    op2 => { op2[0] = operand2.ToTwosComplement(); return true; },
                    op1 => { op1[0] = operand1.ToTwosComplement(); return true; });

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [TestMethod]
        [DataRow(100, -100, HaltReason.OverflowError)]
        [DataRow(-100, 100, HaltReason.UnderflowError)]
        public void ExecuteNextInstruction_SubtractResultIsNotValid_RaisesVirtualMachineHaltedEvent(int operand1, int operand2, HaltReason reason)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    op2 => { op2[0] = operand2.ToTwosComplement(); return true; },
                    op1 => { op1[0] = operand1.ToTwosComplement(); return true; });

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(reason, stack: stack, program: _program);
        }

        [TestMethod]
        [DataRow(100, -100)]
        [DataRow(-100, 100)]
        public void ExecuteNextInstruction_SubtractResultIsNotValid_DoesNotUpdateProgramCounter(int operand1, int operand2)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    op2 => { op2[0] = operand2.ToTwosComplement(); return true; },
                    op1 => { op1[0] = operand1.ToTwosComplement(); return true; });

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPushValueToStack_SetsStatus()
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

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPushValueToStack_SetsHaltedBy()
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

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPushValueToStack_RaisesStatusChangedEvent()
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

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPushValueToStack_RaisesVirtualMachineHaltedEvent()
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

        [TestMethod]
        public void ExecuteNextInstruction_SubtractFailsToPushValueToStack_DoesNotUpdateProgramCounter()
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
