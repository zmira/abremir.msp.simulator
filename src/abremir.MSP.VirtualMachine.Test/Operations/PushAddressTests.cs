using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    public class PushAddressTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private const Operation _operation = Operation.PushAddress;
        private const byte _lsb = 99;
        private const byte _msb = 33;

        public PushAddressTests()
        {
            _program = [(byte)_operation, _lsb, _msb];

            VirtualMachine.SetMemory([], _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddress_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddress_RaisesInstructionArgumentsEvent()
        {
#pragma warning disable IDE0230 // Use UTF-8 string literal
            ExecuteNextInstruction_Verify_RaisesInstructionArgumentsEvent(_operation, new[] { _lsb, _msb }.ToMemoryAddress());
#pragma warning restore IDE0230 // Use UTF-8 string literal
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddress_PushesResultToStack()
        {
            VirtualMachine.Stack.Should().BeEmpty();

            VirtualMachine.ExecuteNextInstruction();

            VirtualMachine.Stack.ElementAt(0).Should().Be(_msb);
            VirtualMachine.Stack.ElementAt(1).Should().Be(_lsb);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddress_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddress_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddress_UpdatesProgramCounter()
        {
            ExecuteNextInstruction_Verify_UpdatesProgramCounter(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToGetFirstValueFromProgramMemory_SetsStatus()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToGetFirstValueFromProgramMemory_SetsHaltedBy()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToGetFirstValueFromProgramMemory_RaisesStatusChangedEvent()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToGetFirstValueFromProgramMemory_RaisesVirtualMachineHaltedEvent()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToGetFirstValueFromProgramMemory_DoesNotUpdateProgramCounter()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToGetSecondValueFromProgramMemory_SetsStatus()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    lsb => { lsb[1] = _lsb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToGetSecondValueFromProgramMemory_SetsHaltedBy()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    lsb => { lsb[1] = _lsb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToGetSecondValueFromProgramMemory_RaisesStatusChangedEvent()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    lsb => { lsb[1] = _lsb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToGetSecondValueFromProgramMemory_RaisesVirtualMachineHaltedEvent()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    lsb => { lsb[1] = _lsb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToGetSecondValueFromProgramMemory_DoesNotUpdateProgramCounter()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    lsb => { lsb[1] = _lsb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToPushFirstValueToStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToPushFirstValueToStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackFull, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToPushFirstValueToStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToPushFirstValueToStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackFull, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToPushFirstValueToStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToPushSecondValueToStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToPushSecondValueToStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackFull, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToPushSecondValueToStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToPushSecondValueToStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackFull, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_PushAddressFailsToPushSecondValueToStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }
    }
}
