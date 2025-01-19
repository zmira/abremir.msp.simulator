using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    public class LoadAddressTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private readonly byte[] _data;
        private const Operation _operation = Operation.LoadAddress;
        private const byte _lsb = 99;
        private const byte _msb = 33;
        private const byte _newLsb = 88;
        private const byte _newMsb = 11;

        public LoadAddressTests()
        {
            _program = [(byte)_operation];

#pragma warning disable IDE0230 // Use UTF-8 string literal
            var _dataAddress = new[] { _lsb, _msb }.ToMemoryAddress();
#pragma warning restore IDE0230 // Use UTF-8 string literal
            _data = new byte[_dataAddress + 2];
            _data.Initialize();
            _data[_dataAddress] = _newLsb;
            _data[_dataAddress + 1] = _newMsb;

            VirtualMachine.SetMemory(_data, _program);
            VirtualMachine.TryPushToStack(_operation, _lsb);
            VirtualMachine.TryPushToStack(_operation, _msb);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddress_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddress_PushesResultToStack()
        {
            VirtualMachine.Stack.ElementAt(0).ShouldNotBe(_newMsb);
            VirtualMachine.Stack.ElementAt(1).ShouldNotBe(_newLsb);

            VirtualMachine.ExecuteNextInstruction();

            VirtualMachine.Stack.ElementAt(0).ShouldBe(_newMsb);
            VirtualMachine.Stack.ElementAt(1).ShouldBe(_newLsb);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddress_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddress_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddress_UpdatesProgramCounter()
        {
            ExecuteNextInstruction_Verify_UpdatesProgramCounter(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPopFirstValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPopFirstValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPopFirstValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPopFirstValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPopFirstValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPopSecondValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPopSecondValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPopSecondValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPopSecondValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPopSecondValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToGetFirstValueFromDataMemory_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToGetFirstValueFromDataMemory_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToGetFirstValueFromDataMemory_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToGetFirstValueFromDataMemory_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToGetFirstValueFromDataMemory_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToGetSecondValueFromDataMemory_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToGetSecondValueFromDataMemory_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToGetSecondValueFromDataMemory_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToGetSecondValueFromDataMemory_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToGetSecondValueFromDataMemory_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPushFirstValueToStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPushFirstValueToStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackFull, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPushFirstValueToStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPushFirstValueToStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackFull, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPushFirstValueToStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPushSecondValueToStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPushSecondValueToStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackFull, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPushSecondValueToStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPushSecondValueToStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackFull, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_LoadAddressFailsToPushSecondValueToStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, data: _data, program: _program);
        }
    }
}
