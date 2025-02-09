using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using abremir.MSP.VirtualMachine.Test.Helpers;

namespace abremir.MSP.VirtualMachine.Test.InternalMethods
{
    public class ExecuteNextInstructionTests : VirtualMachineTestsBase
    {
        [Fact]
        public void ExecuteNextInstruction_ExecutesInstruction()
        {
            var program = new byte[] { (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.ExecuteNextInstruction();

            hook.Verify(Called.Once());
        }

        [Fact]
        public void ExecuteNextInstruction_ExecuteInstructionCompletedIsNull_DoesNothing()
        {
            var program = new byte[] { (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).WithStatus(Status.Interrupted).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler);

            VirtualMachine.ExecuteNextInstruction();

            hook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [Fact]
        public void ExecuteNextInstruction_ExecuteInstructionCompletedIsNotNull_RaisesInstructionExecutedEvent()
        {
            var program = new byte[] { (byte)Operation.Halt };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InstructionExecutedEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuted += handler);

            VirtualMachine.ExecuteNextInstruction();

            hook.Verify(Called.Once());
        }

        [Fact]
        public void ExecuteNextInstruction_ExecuteInstructionCompletedIsNotNullAndOperationIsHalt_DoesNothing()
        {
            var program = new byte[] { (byte)Operation.Halt, (byte)Operation.InputValue };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var inputRequestedEventHook = EventHook.For(VirtualMachine)
                .HookOnly<InputRequestedEventArgs>((virtualMachine, handler) => virtualMachine.InputRequested += handler);
            var operationExecutedEventHook = EventHook.For(VirtualMachine)
                .Hook<InstructionExecutedEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuted += handler)
                .Verify(eventArgs => Check.That(eventArgs.Operation).Is(Operation.Halt))
                .Build();

            VirtualMachine.ExecuteNextInstruction();

            inputRequestedEventHook.Verify(Helpers.EventTestingHelper.Called.Never());
        }

        [Theory]
        [InlineData(Operation.InputValue)]
        [InlineData(Operation.InputCharacter)]
        public void ExecuteNextInstruction_ExecuteInstructionCompletedIsNotNullAndOperationIsInput_RaisesInputRequestedEvent(Operation inputType)
        {
            var program = new byte[] { (byte)inputType };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var hook = EventHook.For(VirtualMachine)
                .HookOnly<InputRequestedEventArgs>((virtualMachine, handler) => virtualMachine.InputRequested += handler);

            VirtualMachine.ExecuteNextInstruction();

            hook.Verify(Called.Once());
        }

        [Fact]
        public void ExecuteNextInstruction_ExecuteInstructionCompletedIsNotNullAndOperationIsOtherAndHasAddress_SetsProgramCounterToAddress()
        {
            const ushort address = 5555;
            var addressComponents = ((int)address).ToLeastAndMostSignificantBytes();
            var program = new byte[] { (byte)Operation.Jump, addressComponents[0], addressComponents[1] };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            VirtualMachine.ExecuteNextInstruction();

            Check.That(VirtualMachine.PC).Is(address);
        }

        [Fact]
        public void ExecuteNextInstruction_ExecuteInstructionCompletedIsNotNullAndOperationIsOtherAndDoesNotHaveAddress_SetsProgramCounterToAddressOfNextNextInstruction()
        {
            const Operation operation = Operation.PushValue;
            var program = new byte[] { (byte)operation, 55 };

            VirtualMachine = new VirtualMachineBuilder().WithProgram(program).Build();

            var pc = VirtualMachine.PC;

            VirtualMachine.ExecuteNextInstruction();

            Check.That(VirtualMachine.PC).Is((ushort)(pc + operation.GetNumberOfMemoryCellsOccupied()));
        }
    }
}
