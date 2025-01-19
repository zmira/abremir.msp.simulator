using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    public class PushValueTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private const Operation _operation = Operation.PushValue;
        private const byte _value = 99;

        public PushValueTests()
        {
            _program = [(byte)_operation, _value];

            VirtualMachine.SetMemory([], _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValue_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValue_RaisesInstructionArgumentsEvent()
        {
            ExecuteNextInstruction_Verify_RaisesInstructionArgumentsEvent(_operation, _value);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValue_PushesResultToStack()
        {
            VirtualMachine.Stack.ShouldBeEmpty();

            VirtualMachine.ExecuteNextInstruction();

            VirtualMachine.Stack.ElementAt(0).ShouldBe(_value);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValue_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [Fact]
        public void ExecuteNextInstruction_PushValue_RaisesOperationExecuted()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValue_UpdatesProgramCounter()
        {
            ExecuteNextInstruction_Verify_UpdatesProgramCounter(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValueFailsToGetValueFromProgramMemory_SetsStatus()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValueFailsToGetValueFromProgramMemory_SetsHaltedBy()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValueFailsToGetValueFromProgramMemory_RaisesStatusChangedEvent()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValueFailsToGetValueFromProgramMemory_RaisesVirtualMachineHaltedEvent()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValueFailsToGetValueFromProgramMemory_DoesNotUpdateProgramCounter()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValueFailsToPushValueToStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValueFailsToPushValueToStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackFull, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValueFailsToPushValueToStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValueFailsToPushValueToStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackFull, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushValueFailsToPushValueToStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }
    }
}
