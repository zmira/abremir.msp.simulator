using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;
using EventTesting;
using EventTestingHelper = abremir.MSP.VirtualMachine.Test.Helpers.EventTestingHelper;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class ContinueTests : VirtualMachineTestsBase
    {
        [Fact]
        public void Continue_StatusNotInterrupted_DoesNotContinue()
        {
            VirtualMachine = new VirtualMachineBuilder().WithStatus(Status.Running).Build();

            VirtualMachine.Status.ShouldNotBe(Status.Interrupted);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<StatusChangedEventArgs>((virtualMachine, handler) => virtualMachine.StatusChanged += handler);

            VirtualMachine.Continue(Operation.InputValue);

            hook.Verify(EventTestingHelper.Called.Never());
        }

        [Theory]
        [InlineData(Operation.InputValue, Operation.InputCharacter)]
        [InlineData(Operation.InputCharacter, Operation.InputValue)]
        public void Continue_StatusInterruptedButInterruptedByAndFromOperationDoNotMatch_DoesNotContinue(Operation interruptOperation, Operation continueFromOperation)
        {
            var program = new byte[] { (byte)interruptOperation };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<StatusChangedEventArgs>((virtualMachine, handler) => virtualMachine.StatusChanged += handler);

            VirtualMachine.Continue(continueFromOperation);

            hook.Verify(EventTestingHelper.Called.Never());
        }

        [Fact]
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

            VirtualMachine.InterruptedBy.ShouldBe(InterruptReason.InputValue);

            VirtualMachine.Continue(operation);

            VirtualMachine.InterruptedBy.ShouldNotBeNull();
        }

        [Fact]
        public void Continue_StatusInterrupted_SetsProgramCounterToAddressOfNextNextInstruction()
        {
            const Operation operation = Operation.InputValue;
            var program = new byte[] { (byte)operation };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            var pc = VirtualMachine.PC;

            VirtualMachine.Continue(operation);

            VirtualMachine.PC.ShouldBe((ushort)(pc + operation.GetNumberOfMemoryCellsOccupied()));
        }

        [Fact]
        public void Continue_StatusInterrupted_ResetsInterruptedBy()
        {
            const Operation operation = Operation.InputValue;
            var program = new byte[] { (byte)operation };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            VirtualMachine.InterruptedBy.ShouldBe(InterruptReason.InputValue);

            VirtualMachine.Continue(operation);

            VirtualMachine.InterruptedBy.ShouldBeNull();
        }

        [Fact]
        public void Continue_StatusInterrupted_SetsStatusRunning()
        {
            const Operation operation = Operation.InputValue;
            var program = new byte[] { (byte)operation };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Step();

            VirtualMachine.Status.ShouldBe(Status.Interrupted);

            VirtualMachine.Continue(operation);

            VirtualMachine.Status.ShouldBe(Status.Running);
        }

        [Fact]
        public void Continue_StatusInterruptedAndModeRun_ExecutesNextInstruction()
        {
            const Operation operation = Operation.InputValue;
            var program = new byte[] { (byte)operation, (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();
            VirtualMachine.Run();

            VirtualMachine.Mode.ShouldBe(Mode.Run);

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.Continue(operation);

            hook.Verify(Called.Once());
        }
    }
}
