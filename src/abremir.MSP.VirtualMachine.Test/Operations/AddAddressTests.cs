using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    public class AddAddressTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private const Operation _operation = Operation.AddAddress;
        private const byte _offset = 21;
        private const byte _msb = 5;
        private const byte _lsb = 6;

        public AddAddressTests()
        {
            _program = [(byte)_operation];

            VirtualMachine.SetMemory([], _program);
            VirtualMachine.TryPushToStack(_operation, _lsb);
            VirtualMachine.TryPushToStack(_operation, _msb);
            VirtualMachine.TryPushToStack(_operation, _offset);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddress_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddress_PushesResultToStack()
        {
            var expectedAddress = (new[] { _lsb, _msb }.ToMemoryAddress() + _offset.FromTwosComplement()).ToLeastAndMostSignificantBytes();

            VirtualMachine.Stack.ElementAt(0).Should().Be(_offset);
            VirtualMachine.Stack.ElementAt(1).Should().Be(_msb);
            VirtualMachine.Stack.ElementAt(2).Should().Be(_lsb);

            VirtualMachine.ExecuteNextInstruction();

            VirtualMachine.Stack.Count.Should().Be(2);
            VirtualMachine.Stack.ElementAt(0).Should().Be(expectedAddress[1]);
            VirtualMachine.Stack.ElementAt(1).Should().Be(expectedAddress[0]);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddress_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddress_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddress_UpdatesProgramCounter()
        {
            ExecuteNextInstruction_Verify_UpdatesProgramCounter(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopFirstValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopFirstValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopFirstValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopFirstValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopFirstValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopSecondValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = _offset; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopSecondValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = _offset; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopSecondValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = _offset; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopSecondValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = _offset; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopSecondValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = _offset; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopThirdValueFromStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = _offset; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopThirdValueFromStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = _offset; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopThirdValueFromStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = _offset; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopThirdValueFromStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = _offset; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackEmpty, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPopThirdValueFromStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = _offset; return true; },
                    msb => { msb[0] = _msb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [Theory]
        [InlineData(255, 255, 0)]
        [InlineData(255, 124, 1)]
        [InlineData(0, 0, -1)]
        public void ExecuteNextInstruction_AddAddressAddressIsNotValid_SetsStatus(byte lsb, byte msb, int offset)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    off => { off[0] = offset.ToTwosComplement(); return true; },
                    op2 => { op2[0] = msb; return true; },
                    op1 => { op1[0] = lsb; return true; });

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Theory]
        [InlineData(255, 255, 0)]
        [InlineData(255, 124, 1)]
        [InlineData(0, 0, -1)]
        public void ExecuteNextInstruction_AddAddressAddressIsNotValid_SetsHaltedBy(byte lsb, byte msb, int offset)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    off => { off[0] = offset.ToTwosComplement(); return true; },
                    op2 => { op2[0] = msb; return true; },
                    op1 => { op1[0] = lsb; return true; });

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, stack: stack, program: _program);
        }

        [Theory]
        [InlineData(255, 255, 0)]
        [InlineData(255, 124, 1)]
        [InlineData(0, 0, -1)]
        public void ExecuteNextInstruction_AddAddressAddressIsNotValid_RaisesStatusChangedEvent(byte lsb, byte msb, int offset)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    off => { off[0] = offset.ToTwosComplement(); return true; },
                    op2 => { op2[0] = msb; return true; },
                    op1 => { op1[0] = lsb; return true; });

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Theory]
        [InlineData(255, 255, 0)]
        [InlineData(255, 124, 1)]
        [InlineData(0, 0, -1)]
        public void ExecuteNextInstruction_AddAddressAddressIsNotValid_RaisesVirtualMachineHaltedEvent(byte lsb, byte msb, int offset)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    off => { off[0] = offset.ToTwosComplement(); return true; },
                    op2 => { op2[0] = msb; return true; },
                    op1 => { op1[0] = lsb; return true; });

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, stack: stack, program: _program);
        }

        [Theory]
        [InlineData(255, 255, 0)]
        [InlineData(255, 124, 1)]
        [InlineData(0, 0, -1)]
        public void ExecuteNextInstruction_AddAddressAddressIsNotValid_DoesNotUpdateProgramCounter(byte lsb, byte msb, int offset)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    off => { off[0] = offset.ToTwosComplement(); return true; },
                    op2 => { op2[0] = msb; return true; },
                    op1 => { op1[0] = lsb; return true; });

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPushFirstValueToStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = ((int)_offset).ToTwosComplement(); return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
            .Returns(false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPushFirstValueToStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = ((int)_offset).ToTwosComplement(); return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackFull, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPushFirstValueToStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = ((int)_offset).ToTwosComplement(); return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPushFirstValueToStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = ((int)_offset).ToTwosComplement(); return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackFull, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPushFirstValueToStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = ((int)_offset).ToTwosComplement(); return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPushSecondValueToStack_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = ((int)_offset).ToTwosComplement(); return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPushSecondValueToStack_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = ((int)_offset).ToTwosComplement(); return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.StackFull, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPushSecondValueToStack_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = ((int)_offset).ToTwosComplement(); return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPushSecondValueToStack_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = ((int)_offset).ToTwosComplement(); return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.StackFull, stack: stack, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_AddAddressFailsToPushSecondValueToStack_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(
                    offset => { offset[0] = ((int)_offset).ToTwosComplement(); return true; },
                    msb => { msb[0] = _msb; return true; },
                    lsb => { lsb[0] = _lsb; return true; });
            stack.TryPush(Arg.Any<byte>())
                .Returns(true, false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(stack: stack, program: _program);
        }
    }
}
