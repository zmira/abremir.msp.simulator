using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Memory;
using abremir.MSP.VirtualMachine.Models;

namespace abremir.MSP.VirtualMachine.Test.Helpers
{
    public class VirtualMachineTestsBase
    {
        protected static readonly Random Random = new();

        protected VirtualMachine VirtualMachine;

        public VirtualMachineTestsBase()
        {
            VirtualMachine = new VirtualMachineBuilder().Build();
        }

        public void ExecuteNextInstruction_Verify_RaisesOperationExecutingEvent(Operation operation)
        {
            var hook = EventHook.For(VirtualMachine)
                .Hook<InstructionExecutingEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuting += handler)
                .Verify(eventArgs => Check.That(eventArgs.Operation).Is(operation))
                .Build();

            VirtualMachine.ExecuteNextInstruction();

            hook.Verify(Called.Once());
        }

        public void ExecuteNextInstruction_Verify_RaisesInstructionArgumentsEvent(Operation operation, int value)
        {
            var hook = EventHook.For(VirtualMachine)
                .Hook<InstructionArgumentsEventArgs>((virtualMachine, handler) => virtualMachine.InstructionArguments += handler)
                .Verify(eventArgs => Check.That(eventArgs.Operation).Is(operation))
                .Verify(eventArgs => Check.That(eventArgs.Value).Is(value))
                .Build();

            VirtualMachine.ExecuteNextInstruction();

            hook.Verify(Called.Once());
        }

        public void ExecuteNextInstruction_Verify_RaisesOperationExecutedEvent(Operation operation)
        {
            var hook = EventHook.For(VirtualMachine)
                .Hook<InstructionExecutedEventArgs>((virtualMachine, handler) => virtualMachine.InstructionExecuted += handler)
                .Verify(eventArgs => Check.That(eventArgs.Operation).Is(operation))
                .Build();

            VirtualMachine.ExecuteNextInstruction();

            hook.Verify(Called.Once());
        }

        public void ExecuteNextInstruction_Verify_UpdatesProgramCounter(Operation operation)
        {
            var pc = VirtualMachine.PC;

            ExecuteNextInstruction_Verify_UpdatesProgramCounterToNewAddress((ushort)(pc + operation.GetNumberOfMemoryCellsOccupied()));
        }

        public void ExecuteNextInstruction_Verify_UpdatesProgramCounterToNewAddress(ushort address)
        {
            VirtualMachine.ExecuteNextInstruction();

            Check.That(VirtualMachine.PC).Is(address);
        }

        public void ExecuteNextInstruction_Verify_DoesNotChangeDataMemory()
        {
            var data = VirtualMachine.Data;

            VirtualMachine.ExecuteNextInstruction();

            Check.That(VirtualMachine.Data).Is(data);
        }

        public void ExecuteNextInstruction_Verify_DoesNotChangeStack()
        {
            var stack = VirtualMachine.Stack;
            var sp = VirtualMachine.SP;

            VirtualMachine.ExecuteNextInstruction();

            Check.That(VirtualMachine.Stack).Is(stack);
            Check.That(VirtualMachine.SP).Is(sp);
        }

        public void ExecuteNextInstruction_Verify_DoesNotUpdateProgramCounter(
            IVirtualMachineMemory? dataMemory = null,
            IVirtualMachineMemory? programMemory = null,
            IStack? stack = null,
            byte[]? data = null,
            byte[]? program = null,
            VirtualMachine? virtualMachine = null)
        {
            VirtualMachine = virtualMachine ?? new VirtualMachineBuilder(dataMemory, programMemory, stack, data, program).Build();

            var pc = VirtualMachine.PC;

            VirtualMachine.ExecuteNextInstruction();

            Check.That(VirtualMachine.PC).Is(pc);
        }

        public void ExecuteNextInstruction_Verify_RaisesVirtualMachineHaltedEvent(
            HaltReason reason,
            IVirtualMachineMemory? dataMemory = null,
            IVirtualMachineMemory? programMemory = null,
            IStack? stack = null,
            byte[]? data = null,
            byte[]? program = null,
            VirtualMachine? virtualMachine = null)
        {
            VirtualMachine = virtualMachine ?? new VirtualMachineBuilder(dataMemory, programMemory, stack, data, program).Build();

            var hook = EventHook.For(VirtualMachine)
                .Hook<VirtualMachineHaltedEventArgs>((virtualMachine, handler) => virtualMachine.VirtualMachineHalted += handler)
                .Verify(eventArgs => Check.That(eventArgs.Reason).Is(reason))
                .Build();

            VirtualMachine.ExecuteNextInstruction();

            hook.Verify(Called.Once());
        }

        public void ExecuteNextInstruction_Verify_RaisesStatusChangedEvent(
            Status newStatus,
            IVirtualMachineMemory? dataMemory = null,
            IVirtualMachineMemory? programMemory = null,
            IStack? stack = null,
            byte[]? data = null,
            byte[]? program = null,
            VirtualMachine? virtualMachine = null)
        {
            VirtualMachine = virtualMachine ?? new VirtualMachineBuilder(dataMemory, programMemory, stack, data, program).Build();

            var hook = EventHook.For(VirtualMachine)
                .Hook<StatusChangedEventArgs>((virtualMachine, handler) => virtualMachine.StatusChanged += handler)
                .Verify(eventArgs => Check.That(eventArgs.NewStatus).Is(newStatus))
                .Build();

            VirtualMachine.ExecuteNextInstruction();

            hook.Verify(Called.Once());
        }

        public void ExecuteNextInstruction_Verify_SetsStatus(
            Status status,
            IVirtualMachineMemory? dataMemory = null,
            IVirtualMachineMemory? programMemory = null,
            IStack? stack = null,
            byte[]? data = null,
            byte[]? program = null,
            VirtualMachine? virtualMachine = null)
        {
            VirtualMachine = virtualMachine ?? new VirtualMachineBuilder(dataMemory, programMemory, stack, data, program).Build();

            VirtualMachine.ExecuteNextInstruction();

            Check.That(VirtualMachine.Status).Is(status);
        }

        public void ExecuteNextInstruction_Verify_SetsHaltedBy(
            HaltReason reason,
            IVirtualMachineMemory? dataMemory = null,
            IVirtualMachineMemory? programMemory = null,
            IStack? stack = null,
            byte[]? data = null,
            byte[]? program = null,
            VirtualMachine? virtualMachine = null)
        {
            VirtualMachine = virtualMachine ?? new VirtualMachineBuilder(dataMemory, programMemory, stack, data, program).Build();

            VirtualMachine.ExecuteNextInstruction();

            Check.That(VirtualMachine.HaltedBy).Is(reason);
        }

        public void ExecuteNextInstruction_Verify_SetsInterruptedBy(
            InterruptReason reason,
            IVirtualMachineMemory? dataMemory = null,
            IVirtualMachineMemory? programMemory = null,
            IStack? stack = null,
            byte[]? data = null,
            byte[]? program = null,
            VirtualMachine? virtualMachine = null)
        {
            VirtualMachine = virtualMachine ?? new VirtualMachineBuilder(dataMemory, programMemory, stack, data, program).Build();

            VirtualMachine.ExecuteNextInstruction();

            Check.That(VirtualMachine.InterruptedBy).Is(reason);
        }
    }
}
