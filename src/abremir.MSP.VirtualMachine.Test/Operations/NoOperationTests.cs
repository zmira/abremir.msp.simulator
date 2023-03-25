using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.Operations
{
    public class NoOperationTests : VirtualMachineTestsBase
    {
        private readonly byte[] _data;
        private readonly byte[] _program;
        private const Operation _operation = Operation.NoOperation;

        public NoOperationTests() : base()
        {
            _data = Array.Empty<byte>();
            _program = new byte[] { (byte)_operation };

            VirtualMachine.SetMemory(_data, _program);
        }

        [Fact]
        public void ExecuteNextInstruction_NoOperation_RaisesOperationExecutingEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_NoOperation_DoesNotChangeStack()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeStack();
        }

        [Fact]
        public void ExecuteNextInstruction_NoOperation_DoesNotChangeDataMemory()
        {
            ExecuteNextInstruction_Verify_DoesNotChangeDataMemory();
        }

        [Fact]
        public void ExecuteNextInstruction_NoOperation_RaisesOperationExecutedEvent()
        {
            ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(_operation);
        }

        [Fact]
        public void ExecuteNextInstruction_NoOperation_UpdatesProgramCounter()
        {
            ExecuteNextInstruction_Verify_UpdatesProgramCounter(_operation);
        }
    }
}
