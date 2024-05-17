using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;
using NSubstitute;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    public class CallTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private const Operation _operation = Operation.Call;
        private const byte _lsb = 99;
        private const byte _msb = 33;

        public CallTests()
        {
            _program = new byte[] { (byte)_operation, _lsb, _msb };

            VirtualMachine.SetMemory(Array.Empty<byte>(), _program);
        }

        [Fact]
        public void ExecuteNextInstruction_Call_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_Call_RaisesInstructionArgumentsEvent()
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
        public void ExecuteNextInstruction_Call_PushesNextInstructionAddressToStack()
        {
            var pc = VirtualMachine.PC;

            VirtualMachine.ExecuteNextInstruction();

            var nextInstructionAddress = pc + Operation.Call.GetNumberOfMemoryCellsOccupied();
            var memoryAddressInStack = new[] { VirtualMachine.Stack.ElementAt(1), VirtualMachine.Stack.ElementAt(0) }.ToMemoryAddress();

            memoryAddressInStack.Should().Be(nextInstructionAddress);
        }

        [Fact]
        public void ExecuteNextInstruction_Call_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [Fact]
        public void ExecuteNextInstruction_Call_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_CallFailsToGetFirstValueFromProgramMemory_SetsStatus()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_CallFailsToGetFirstValueFromProgramMemory_SetsHaltedBy()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_CallFailsToGetFirstValueFromProgramMemory_RaisesStatusChangedEvent()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_CallFailsToGetFirstValueFromProgramMemory_RaisesVirtualMachineHaltedEvent()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_CallFailsToGetFirstValueFromProgramMemory_DoesNotUpdateProgramCounter()
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(programMemory: memory, program: _program);
        }

        [Fact]
        public void ExecuteNextInstruction_CallFailsToGetSecondValueFromProgramMemory_SetsStatus()
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
        public void ExecuteNextInstruction_CallFailsToGetSecondValueFromProgramMemory_SetsHaltedBy()
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
        public void ExecuteNextInstruction_CallFailsToGetSecondValueFromProgramMemory_RaisesStatusChangedEvent()
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
        public void ExecuteNextInstruction_CallFailsToGetSecondValueFromProgramMemory_RaisesVirtualMachineHaltedEvent()
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
        public void ExecuteNextInstruction_CallFailsToGetSecondValueFromProgramMemory_DoesNotUpdateProgramCounter()
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
        public void ExecuteNextInstruction_CallAddressIsNotValid_SetsStatus(byte lsb, byte msb)
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
        public void ExecuteNextInstruction_CallAddressIsNotValid_SetsHaltedBy(byte lsb, byte msb)
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
        public void ExecuteNextInstruction_CallAddressIsNotValid_RaisesStatusChangedEvent(byte lsb, byte msb)
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
        public void ExecuteNextInstruction_CallAddressIsNotValid_RaisesVirtualMachineHaltedEvent(byte lsb, byte msb)
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
        public void ExecuteNextInstruction_CallAddressIsNotValid_DoesNotUpdateProgramCounter(byte lsb, byte msb)
        {
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    l => { l[1] = lsb; return true; },
                    m => { m[1] = msb; return true; });

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(programMemory: memory, program: _program);
        }

        [Theory]
        [InlineData(253, 124)]
        public void ExecuteNextInstruction_CallNextInstructionAddressIsNotValid_SetsStatus(byte lsb, byte msb)
        {
            var program = new byte[Constants.MemoryCapacity];
            Array.Clear(program, 0, Constants.MemoryCapacity);
            var callInstructionAddress = new[] { lsb, msb }.ToMemoryAddress();
            program[0] = (byte)Operation.Jump;
            program[1] = lsb;
            program[2] = msb;
            program[callInstructionAddress] = (byte)Operation.Call;

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, virtualMachine: VirtualMachine);
        }

        [Theory]
        [InlineData(253, 124)]
        public void ExecuteNextInstruction_CallNextInstructionAddressIsNotValid_SetsHaltedBy(byte lsb, byte msb)
        {
            var program = new byte[Constants.MemoryCapacity];
            Array.Clear(program, 0, Constants.MemoryCapacity);
            var callInstructionAddress = new[] { lsb, msb }.ToMemoryAddress();
            program[0] = (byte)Operation.Jump;
            program[1] = lsb;
            program[2] = msb;
            program[callInstructionAddress] = (byte)Operation.Call;

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, virtualMachine: VirtualMachine);
        }

        [Theory]
        [InlineData(253, 124)]
        public void ExecuteNextInstruction_CallNextInstructionAddressIsNotValid_RaisesStatusChangedEvent(byte lsb, byte msb)
        {
            var program = new byte[Constants.MemoryCapacity];
            Array.Clear(program, 0, Constants.MemoryCapacity);
            var callInstructionAddress = new[] { lsb, msb }.ToMemoryAddress();
            program[0] = (byte)Operation.Jump;
            program[1] = lsb;
            program[2] = msb;
            program[callInstructionAddress] = (byte)Operation.Call;

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, virtualMachine: VirtualMachine);
        }

        [Theory]
        [InlineData(253, 124)]
        public void ExecuteNextInstruction_CallNextInstructionAddressIsNotValid_RaisesVirtualMachineHaltedEvent(byte lsb, byte msb)
        {
            var program = new byte[Constants.MemoryCapacity];
            Array.Clear(program, 0, Constants.MemoryCapacity);
            var callInstructionAddress = new[] { lsb, msb }.ToMemoryAddress();
            program[0] = (byte)Operation.Jump;
            program[1] = lsb;
            program[2] = msb;
            program[callInstructionAddress] = (byte)Operation.Call;

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, virtualMachine: VirtualMachine);
        }

        [Theory]
        [InlineData(253, 124)]
        public void ExecuteNextInstruction_CallNextInstructionAddressIsNotValid_DoesNotUpdateProgramCounter(byte lsb, byte msb)
        {
            var program = new byte[Constants.MemoryCapacity];
            Array.Clear(program, 0, Constants.MemoryCapacity);
            var callInstructionAddress = new[] { lsb, msb }.ToMemoryAddress();
            program[0] = (byte)Operation.Jump;
            program[1] = lsb;
            program[2] = msb;
            program[callInstructionAddress] = (byte)Operation.Call;

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(virtualMachine: VirtualMachine);
        }
    }
}
