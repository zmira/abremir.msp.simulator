using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    public class StoreAddressTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private readonly byte[] _data;
        private const Operation _operation = Operation.StoreAddress;
        private const byte _lsb = 99;
        private const byte _msb = 33;
        private const byte _lsbValue = 00;
        private const byte _msbValue = 11;
#pragma warning disable IDE0230 // Use UTF-8 string literal
        private readonly int _dataAddress = new[] { _lsb, _msb }.ToMemoryAddress();
#pragma warning restore IDE0230 // Use UTF-8 string literal

        public StoreAddressTests()
        {
            _program = [(byte)_operation];
            _data = new byte[_dataAddress + 2];
            _data.Initialize();

            VirtualMachine.SetMemory(_data, _program);
            VirtualMachine.TryPushToStack(_operation, _lsb);
            VirtualMachine.TryPushToStack(_operation, _msb);
            VirtualMachine.TryPushToStack(_operation, _lsbValue);
            VirtualMachine.TryPushToStack(_operation, _msbValue);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddress_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddress_SetsResultInDataMemory()
        {
            VirtualMachine.Data.ElementAt(_dataAddress).ShouldBe((byte)0);
            VirtualMachine.Data.ElementAt(_dataAddress + 1).ShouldBe((byte)0);

            VirtualMachine.ExecuteNextInstruction();

            VirtualMachine.Data.ElementAt(_dataAddress).ShouldBe(_lsbValue);
            VirtualMachine.Data.ElementAt(_dataAddress + 1).ShouldBe(_msbValue);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddress_PopsValuesFromStack()
        {
            VirtualMachine.Stack.ShouldNotBeEmpty();

            VirtualMachine.ExecuteNextInstruction();

            VirtualMachine.Stack.ShouldBeEmpty();
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddress_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddress_UpdatesProgramCounter()
        {
            ExecuteNextInstruction_Verify_UpdatesProgramCounter(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopFirstValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopSecondValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopThirdValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopFourthValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopFirstValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopSecondValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopThirdValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopFourthValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopFirstValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopSecondValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopThirdValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopFourthValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopFirstValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopSecondValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopThirdValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopFourthValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopFirstValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopSecondValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopThirdValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToPopFourthValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToSetFirstValueInDataMemory_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToSetSecondValueInDataMemory_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToSetFirstValueInDataMemory_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToSetSecondValueInDataMemory_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToSetFirstValueInDataMemory_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToSetSecondValueInDataMemory_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToSetFirstValueInDataMemory_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToSetSecondValueInDataMemory_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_StoreAddressFailsToSetFirstValueInDataMemory_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    msbValue => { msbValue[0] = _msbValue; return true; },
                    lsbValue => { lsbValue[0] = _lsbValue; return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(dataMemory: memory, stack: stack, data: _data, program: _program);
        }
    }
}
