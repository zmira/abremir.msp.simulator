using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    [TestClass]
    public class ExecuteInstructionTests : VirtualMachineTestsBase
    {
        [TestMethod]
        [DataRow(Status.Halted)]
        [DataRow(Status.Interrupted)]
        [DataRow(Status.Suspended)]
        public void ExecuteInstruction_SuspendedExecution_DoesNotExecuteInstruction(Status status)
        {
            var program = new byte[] { (byte)Operation.PushValue, 1 };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.SetStatus(status);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.ExecuteInstruction(VirtualMachine.PC);

            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [TestMethod]
        [DataRow(Status.Halted)]
        [DataRow(Status.Interrupted)]
        [DataRow(Status.Suspended)]
        public void ExecuteInstruction_SuspendedExecution_ReturnsNull(Status status)
        {
            var program = new byte[] { (byte)Operation.PushValue, 1 };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.SetStatus(status);

            var result = VirtualMachine.ExecuteInstruction(VirtualMachine.PC);

            Check.That(result).IsNull();
        }

        [TestMethod]
        public void ExecuteInstruction_InvalidProgramCounterAddress_DoesNotExecuteInstruction()
        {
            var program = new byte[] { (byte)Operation.InputValue };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.ExecuteInstruction(Constants.MemoryCapacity);

            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [TestMethod]
        public void ExecuteInstruction_InvalidProgramCounterAddress_ReturnsNull()
        {
            var program = new byte[] { (byte)Operation.InputValue };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var result = VirtualMachine.ExecuteInstruction(Constants.MemoryCapacity);

            Check.That(result).IsNull();
        }

        [TestMethod]
        public void ExecuteInstruction_InvalidOperation_DoesNotExecuteInstruction()
        {
#pragma warning disable IDE0230 // Use UTF-8 string literal
            var program = new byte[] { 33 };
#pragma warning restore IDE0230 // Use UTF-8 string literal

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.ExecuteInstruction(VirtualMachine.PC);

            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [TestMethod]
        public void ExecuteInstruction_InvalidOperation_SetsStatusToHalted()
        {
#pragma warning disable IDE0230 // Use UTF-8 string literal
            var program = new byte[] { 33 };
#pragma warning restore IDE0230 // Use UTF-8 string literal

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            VirtualMachine.ExecuteInstruction(VirtualMachine.PC);

            Check.That(VirtualMachine.Status).Is(Status.Halted);
        }

        [TestMethod]
        public void ExecuteInstruction_InvalidOperation_SetsHaltReasonUnknownOperation()
        {
#pragma warning disable IDE0230 // Use UTF-8 string literal
            var program = new byte[] { 33 };
#pragma warning restore IDE0230 // Use UTF-8 string literal

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            VirtualMachine.ExecuteInstruction(VirtualMachine.PC);

            Check.That(VirtualMachine.HaltedBy).Is(HaltReason.UnknownOperation);
        }

        [TestMethod]
        public void ExecuteInstruction_InvalidOperation_ReturnsNull()
        {
#pragma warning disable IDE0230 // Use UTF-8 string literal
            var program = new byte[] { 33 };
#pragma warning restore IDE0230 // Use UTF-8 string literal

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var result = VirtualMachine.ExecuteInstruction(VirtualMachine.PC);

            Check.That(result).IsNull();
        }

        [TestMethod]
        public void ExecuteInstruction_ValidOperation_ExecutesInstruction()
        {
            var program = new byte[] { (byte)Operation.PushValue, 1 };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.ExecuteInstruction(VirtualMachine.PC);

            hook.Verify(Called.Once());
        }

        [TestMethod]
        public void ExecuteInstruction_ValidOperation_ReturnsExecuteInstructionCompleted()
        {
            const Operation operation = Operation.PushValue;
            var program = new byte[] { (byte)operation, 1 };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var result = VirtualMachine.ExecuteInstruction(VirtualMachine.PC);

            Check.That(result).IsNotNull();
            Check.That(result!.Operation).Is(operation);
        }
    }
}
