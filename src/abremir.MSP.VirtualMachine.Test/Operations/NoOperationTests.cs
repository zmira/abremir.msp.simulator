using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    [TestClass]
    public class NoOperationTests : VirtualMachineTestsBase
    {
        private readonly byte[] _data;
        private readonly byte[] _program;
        private const Operation _operation = Operation.NoOperation;

        public NoOperationTests()
        {
            _data = [];
            _program = [(byte)_operation];

            VirtualMachine.SetMemory(_data, _program);
        }

        [TestMethod]
        public void ExecuteNextInstruction_NoOperation_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [TestMethod]
        public void ExecuteNextInstruction_NoOperation_DoesNotChangeStack()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeStack();
        }

        [TestMethod]
        public void ExecuteNextInstruction_NoOperation_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [TestMethod]
        public void ExecuteNextInstruction_NoOperation_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [TestMethod]
        public void ExecuteNextInstruction_NoOperation_UpdatesProgramCounter()
        {
            ExecuteNextInstruction_Verify_UpdatesProgramCounter(_operation);
        }
    }
}
