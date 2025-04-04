using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    [TestClass]
    public class BitwiseNotTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private const Operation _operation = Operation.BitwiseNot;
        private const byte _operand = 1;

        public BitwiseNotTests()
        {
            _program = [(byte)_operation];

            VirtualMachine.SetMemory([], _program);
            VirtualMachine.TryPushToStack(_operation, _operand);
        }

        [TestMethod]
        public void ExecuteNextInstruction_BitwiseNot_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [TestMethod]
        [DataRow((byte)0b0001, (byte)0b11111110)]
        [DataRow((byte)0b1010, (byte)0b11110101)]
        [DataRow((byte)0b1011, (byte)0b11110100)]
        [DataRow((byte)0b0000, (byte)0b11111111)]
        public void ExecuteNextInstruction_BitwiseNot_PushesResultToStack(byte operand, byte expectedResult)
        {
            while (VirtualMachine.Stack.Count != 0)
            {
                VirtualMachine.TryPopFromStack(Operation.BitwiseNot, out _);
            }

            VirtualMachine.TryPushToStack(Operation.BitwiseNot, operand);

            VirtualMachine.ExecuteNextInstruction();

            Check.That(VirtualMachine.Stack).CountIs(1);
            Check.That(VirtualMachine.Stack.ElementAt(0)).Is(expectedResult);
        }

        [TestMethod]
        public void ExecuteNextInstruction_BitwiseNot_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [TestMethod]
        public void ExecuteNextInstruction_BitwiseNot_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [TestMethod]
        public void ExecuteNextInstruction_BitwiseNot_UpdatesProgramCounter()
        {
            ExecuteNextInstruction_Verify_UpdatesProgramCounter(_operation);
        }

        [TestMethod]
        public void ExecuteNextInstruction_BitwiseNotFailsToPopValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_BitwiseNotFailsToPopValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_BitwiseNotFailsToPopValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_BitwiseNotFailsToPopValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_BitwiseNotFailsToPopValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_BitwiseNotFailsToPushValueToStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(operand => { operand[0] = _operand; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_BitwiseNotFailsToPushValueToStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(operand => { operand[0] = _operand; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackFull, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_BitwiseNotFailsToPushValueToStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(operand => { operand[0] = _operand; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_BitwiseNotFailsToPushValueToStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(operand => { operand[0] = _operand; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackFull, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_BitwiseNotFailsToPushValueToStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(operand => { operand[0] = _operand; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }
    }
}
