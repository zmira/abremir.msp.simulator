﻿using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    [TestClass]
    public class HaltTests : VirtualMachineTestsBase
    {
        private readonly byte[] _program;
        private const Operation _operation = Operation.Halt;

        public HaltTests()
        {
            _program = [(byte)_operation];

            VirtualMachine.SetMemory([], _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_Halt_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [TestMethod]
        public void ExecuteNextInstruction_Halt_SetsStatus()
        {
            ExecuteNextInstruction_Verify_SetsStatus(Status.Halted, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_Halt_SetsHaltedBy()
        {
            ExecuteNextInstruction_Verify_SetsHaltedBy(HaltReason.HaltInstruction, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_Halt_RaisesStatusChangedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(Status.Halted, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_Halt_RaisesVirtualMachineHaltedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(HaltReason.HaltInstruction, program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_Halt_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [TestMethod]
        public void ExecuteNextInstruction_Halt_DoesNotUpdateProgramCounter()
        {
            ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(program: _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_Halt_DoesNotChangeStack()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeStack();
        }

        [TestMethod]
        public void ExecuteNextInstruction_Halt_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }
    }
}
