﻿using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    [TestClass]
    public class StoreValueTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private readonly byte[] _data;
        private const Operation _operation = Operation.StoreValue;
        private const byte _lsb = 99;
        private const byte _msb = 33;
        private const byte _value = 55;
#pragma warning disable IDE0230 // Use UTF-8 string literal
        private readonly int _dataAddress = new[] { _lsb, _msb }.ToMemoryAddress();
#pragma warning restore IDE0230 // Use UTF-8 string literal

        public StoreValueTests()
        {
            _program = [(byte)_operation];
            _data = new byte[_dataAddress + 1];
            _data.Initialize();

            VirtualMachine.SetMemory(_data, _program);
            VirtualMachine.TryPushToStack(_operation, _lsb);
            VirtualMachine.TryPushToStack(_operation, _msb);
            VirtualMachine.TryPushToStack(_operation, _value);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValue_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValue_SetsResultInDataMemory()
        {
            Check.That(VirtualMachine.Data.ElementAt(_dataAddress)).IsNotEqualTo(_value);

            VirtualMachine.ExecuteNextInstruction();

            Check.That(VirtualMachine.Data.ElementAt(_dataAddress)).Is(_value);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValue_PopsValueFromStack()
        {
            Check.That(VirtualMachine.Stack).Not.IsEmpty();

            VirtualMachine.ExecuteNextInstruction();

            Check.That(VirtualMachine.Stack).IsEmpty();
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValue_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValue_UpdatesProgramCounter()
        {
            ExecuteNextInstruction_Verify_UpdatesProgramCounter(_operation);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopFirstValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopSecondValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopThirdValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopFirstValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopSecondValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopThirdValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopFirstValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopSecondValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopThirdValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopFirstValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopSecondValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopThirdValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopFirstValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopSecondValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToPopThirdValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToSetValueInDataMemory_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToSetValueInDataMemory_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToSetValueInDataMemory_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToSetValueInDataMemory_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, dataMemory: memory, stack: stack, data: _data, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_StoreValueFailsToSetValueInDataMemory_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    value => { value[0] = _value; return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TrySet(Arg.Any<ushort>(), Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(dataMemory: memory, stack: stack, data: _data, program: _program);
        }
    }
}
