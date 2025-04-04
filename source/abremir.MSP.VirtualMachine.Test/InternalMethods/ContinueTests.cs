using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    [TestClass]
    public class ContinueTests : VirtualMachineTestsBase
    {
        [TestMethod]
        public void Continue_StatusNotInterrupted_DoesNotContinue()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Status.Running).Build();

            Check.That(VirtualMachine.Status).IsNotEqualTo(Status.Interrupted);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<StatusChangedEventArgs>((virtualMachine, handler) => virtualMachine.StatusChanged += handler);

            VirtualMachine.Continue(Operation.InputValue);

            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [TestMethod]
        [DataRow(Operation.InputValue, Operation.InputCharacter)]
        [DataRow(Operation.InputCharacter, Operation.InputValue)]
        public void Continue_StatusInterruptedButInterruptedByAndFromOperationDoNotMatch_DoesNotContinue(Operation interruptOperation, Operation continueFromOperation)
        {
            var program = new byte[] { (byte)interruptOperation };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<StatusChangedEventArgs>((virtualMachine, handler) => virtualMachine.StatusChanged += handler);

            VirtualMachine.Continue(continueFromOperation);

            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [TestMethod]
        public void Continue_StatusInterruptedAndDoesNotSetProgramCounterToAddressOfNextNextInstruction_DoesNotContinue()
        {
            const Operation operation = Operation.InputValue;
            const int lastMemoryPosition = Constants.MemoryCapacity - 1;
            var lastMemoryPositionComponents = lastMemoryPosition.ToLeastAndMostSignificantBytes();
            List<byte> program = new(VirtualMachine.Program)
            {
                [0] = (byte)Operation.Jump,
                [1] = lastMemoryPositionComponents[0],
                [2] = lastMemoryPositionComponents[1],
                [lastMemoryPosition] = (byte)operation
            };

            VirtualMachine = new VirtualMachineBuilder().WithProgram([.. program]).Build();
            VirtualMachine.Run();

            Check.That(VirtualMachine.InterruptedBy).Is(InterruptReason.InputValue);

            VirtualMachine.Continue(operation);

            Check.That(VirtualMachine.InterruptedBy).IsNotNull();
        }

        [TestMethod]
        public void Continue_StatusInterrupted_SetsProgramCounterToAddressOfNextNextInstruction()
        {
            const Operation operation = Operation.InputValue;
            var program = new byte[] { (byte)operation };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            var pc = VirtualMachine.PC;

            VirtualMachine.Continue(operation);

            Check.That(VirtualMachine.PC).Is((ushort)(pc + operation.GetNumberOfMemoryCellsOccupied()));
        }

        [TestMethod]
        public void Continue_StatusInterrupted_ResetsInterruptedBy()
        {
            const Operation operation = Operation.InputValue;
            var program = new byte[] { (byte)operation };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            Check.That(VirtualMachine.InterruptedBy).Is(InterruptReason.InputValue);

            VirtualMachine.Continue(operation);

            Check.That(VirtualMachine.InterruptedBy).IsNull();
        }

        [TestMethod]
        public void Continue_StatusInterrupted_SetsStatusRunning()
        {
            const Operation operation = Operation.InputValue;
            var program = new byte[] { (byte)operation };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            Check.That(VirtualMachine.Status).Is(Status.Interrupted);

            VirtualMachine.Continue(operation);

            Check.That(VirtualMachine.Status).Is(Status.Running);
        }

        [TestMethod]
        public void Continue_StatusInterruptedAndModeRun_ExecutesNextInstruction()
        {
            const Operation operation = Operation.InputValue;
            var program = new byte[] { (byte)operation, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            Check.That(VirtualMachine.Mode).Is(Mode.Run);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.Continue(operation);

            hook.Verify(Called.Once());
        }
    }
}
