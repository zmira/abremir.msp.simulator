using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    [TestClass]
    public class JumpIfFalseTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private const Operation _operation = Operation.JumpIfFalse;
        private const byte _lsb = 99;
        private const byte _msb = 33;

        public JumpIfFalseTests()
        {
            _program = [(byte)_operation, _lsb, _msb];

            VirtualMachine.SetMemory([], _program);
            VirtualMachine.TryPushToStack(_operation, 0);
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalse_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalse_RaisesInstructionArgumentsEvent()
        {
#pragma warning disable IDE0230 // Use UTF-8 string literal
            ExecuteNextInstruction_Verify_RaisesInstructionArgumentsEvent(_operation, new[] { _lsb, _msb }.ToMemoryAddress());
#pragma warning restore IDE0230 // Use UTF-8 string literal
        }

        [TestMethod]
        public void ExecuteNextInstruction_Call_UpdatesProgramCounterToNewAddress()
        {
#pragma warning disable IDE0230 // Use UTF-8 string literal
            ExecuteNextInstruction_Verify_UpdatesProgramCounterToNewAddress((ushort)new[] { _lsb, _msb }.ToMemoryAddress());
#pragma warning restore IDE0230 // Use UTF-8 string literal
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalse_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalse_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalseFailsToGetFirstValueFromProgramMemory_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalseFailsToGetFirstValueFromProgramMemory_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalseFailsToGetFirstValueFromProgramMemory_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalseFailsToGetFirstValueFromProgramMemory_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalseFailsToGetFirstValueFromProgramMemory_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalseFailsToGetSecondValueFromProgramMemory_SetsStatus()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    lsb => { lsb[1] = _lsb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalseFailsToGetSecondValueFromProgramMemory_SetsHaltedBy()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    lsb => { lsb[1] = _lsb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalseFailsToGetSecondValueFromProgramMemory_RaisesStatusChangedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    lsb => { lsb[1] = _lsb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalseFailsToGetSecondValueFromProgramMemory_RaisesVirtualMachineHaltedEvent()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    lsb => { lsb[1] = _lsb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalseFailsToGetSecondValueFromProgramMemory_DoesNotUpdateProgramCounter()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    lsb => { lsb[1] = _lsb; return true; },
                    _ => false);

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        [DataRow((byte)255, (byte)255)]
        [DataRow((byte)255, (byte)125)]
        public void ExecuteNextInstruction_JumpIfFalseAddressIsNotValid_SetsStatus(byte lsb, byte msb)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    l => { l[1] = lsb; return true; },
                    m => { m[1] = msb; return true; });

            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        [DataRow((byte)255, (byte)255)]
        [DataRow((byte)255, (byte)125)]
        public void ExecuteNextInstruction_JumpIfFalseAddressIsNotValid_SetsHaltedBy(byte lsb, byte msb)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    l => { l[1] = lsb; return true; },
                    m => { m[1] = msb; return true; });

            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.MemoryAddressViolation, programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        [DataRow((byte)255, (byte)255)]
        [DataRow((byte)255, (byte)125)]
        public void ExecuteNextInstruction_JumpIfFalseAddressIsNotValid_RaisesStatusChangedEvent(byte lsb, byte msb)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    l => { l[1] = lsb; return true; },
                    m => { m[1] = msb; return true; });

            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        [DataRow((byte)255, (byte)255)]
        [DataRow((byte)255, (byte)125)]
        public void ExecuteNextInstruction_JumpIfFalseAddressIsNotValid_RaisesVirtualMachineHaltedEvent(byte lsb, byte msb)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    l => { l[1] = lsb; return true; },
                    m => { m[1] = msb; return true; });

            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.MemoryAddressViolation, programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        [DataRow((byte)255, (byte)255)]
        [DataRow((byte)255, (byte)125)]
        public void ExecuteNextInstruction_JumpIfFalseAddressIsNotValid_DoesNotUpdateProgramCounter(byte lsb, byte msb)
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)0; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(
                    operation => { operation[1] = (byte)_operation; return true; },
                    l => { l[1] = lsb; return true; },
                    m => { m[1] = msb; return true; });

            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(programMemory: memory, stack: stack, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_JumpIfFalseValueIsNotZero_UpdatesPCToNextInstruction()
        {
            var stack = Substitute.For<IStack>();
            stack.TryPop(out Arg.Any<byte>())
                .Returns(value => { value[0] = (byte)1; return true; });
            var memory = Substitute.For<IVirtualMachineMemory>();
            memory.TryGet(Arg.Any<ushort>(), out Arg.Any<byte>())
                .Returns(operation => { operation[1] = (byte)_operation; return true; });

            VirtualMachine = new VirtualMachineBuilder(programMemory: memory, stack: stack, program: _program).Build();

            var pc = VirtualMachine.PC;

            VirtualMachine.ExecuteNextInstruction();

            Check.That(VirtualMachine.PC).Is((ushort)(pc + _operation.GetNumberOfMemoryCellsOccupied()));
        }
    }
}
