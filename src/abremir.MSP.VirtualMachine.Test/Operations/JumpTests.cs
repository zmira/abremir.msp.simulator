using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    public class JumpTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private const Operation _operation = Operation.Jump;
        private const byte _lsb = 99;
        private const byte _msb = 33;

        public JumpTests()
        {
            _program = new byte[] { (byte)_operation, _lsb, _msb };

            VirtualMachine.SetMemory(Array.Empty<byte>(), _program);
        }

        [Fact]
        public void ExecuteNextInstruction_Jump_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_Jump_RaisesInstructionArgumentsEvent()
        {
#pragma warning disable IDE0230 // Use UTF-8 string literal
            ExecuteNextInstruction_Verify_RaisesInstructionArgumentsEvent(_operation, new[] { _lsb, _msb }.ToMemoryAddress());
#pragma warning restore IDE0230 // Use UTF-8 string literal
        }

        [Fact]
        public void ExecuteNextInstruction_Call_UpdatesProgramCounterToNewAddress()
        {
#pragma warning disable IDE0230 // Use UTF-8 string literal
            ExecuteNextInstruction_Verify_UpdatesProgramCounterToNewAddress((ushort)new[] { _lsb, _msb }.ToMemoryAddress());
#pragma warning restore IDE0230 // Use UTF-8 string literal
        }

        [Fact]
        public void ExecuteNextInstruction_Jump_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [Fact]
        public void ExecuteNextInstruction_Jump_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_JumpFailsToGetFirstValueFromProgramMemory_SetsStatus()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_JumpFailsToGetFirstValueFromProgramMemory_SetsHaltedBy()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_JumpFailsToGetFirstValueFromProgramMemory_RaisesStatusChangedEvent()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_JumpFailsToGetFirstValueFromProgramMemory_RaisesVirtualMachineHaltedEvent()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_JumpFailsToGetFirstValueFromProgramMemory_DoesNotUpdateProgramCounter()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_JumpFailsToGetSecondValueFromProgramMemory_SetsStatus()
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
        public void ExecuteNextInstruction_JumpFailsToGetSecondValueFromProgramMemory_SetsHaltedBy()
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
        public void ExecuteNextInstruction_JumpFailsToGetSecondValueFromProgramMemory_RaisesStatusChangedEvent()
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
        public void ExecuteNextInstruction_JumpFailsToGetSecondValueFromProgramMemory_RaisesVirtualMachineHaltedEvent()
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
        public void ExecuteNextInstruction_JumpFailsToGetSecondValueFromProgramMemory_DoesNotUpdateProgramCounter()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    lsb => { lsb[1] = _lsb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(programMemory: memory, program: _program);
        }

        [Theory]
        [InlineData(255, 255)]
        [InlineData(255, 125)]
        public void ExecuteNextInstruction_JumpAddressIsNotValid_SetsStatus(byte lsb, byte msb)
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    l => { l[1] = lsb; return true; },
                    m => { m[1] = msb; return true; });

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, programMemory: memory, program: _program);
        }

        [Theory]
        [InlineData(255, 255)]
        [InlineData(255, 125)]
        public void ExecuteNextInstruction_JumpAddressIsNotValid_SetsHaltedBy(byte lsb, byte msb)
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    l => { l[1] = lsb; return true; },
                    m => { m[1] = msb; return true; });

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, programMemory: memory, program: _program);
        }

        [Theory]
        [InlineData(255, 255)]
        [InlineData(255, 125)]
        public void ExecuteNextInstruction_JumpAddressIsNotValid_RaisesStatusChangedEvent(byte lsb, byte msb)
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    l => { l[1] = lsb; return true; },
                    m => { m[1] = msb; return true; });

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, programMemory: memory, program: _program);
        }

        [Theory]
        [InlineData(255, 255)]
        [InlineData(255, 125)]
        public void ExecuteNextInstruction_JumpAddressIsNotValid_RaisesVirtualMachineHaltedEvent(byte lsb, byte msb)
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    l => { l[1] = lsb; return true; },
                    m => { m[1] = msb; return true; });

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, programMemory: memory, program: _program);
        }

        [Theory]
        [InlineData(255, 255)]
        [InlineData(255, 125)]
        public void ExecuteNextInstruction_JumpAddressIsNotValid_DoesNotUpdateProgramCounter(byte lsb, byte msb)
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    l => { l[1] = lsb; return true; },
                    m => { m[1] = msb; return true; });

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(programMemory: memory, program: _program);
        }
    }
}
