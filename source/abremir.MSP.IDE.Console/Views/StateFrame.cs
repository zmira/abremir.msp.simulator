using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine;
using abremir.MSP.VirtualMachine.Models;
using Terminal.Gui;

namespace abremir.MSP.IDE.Console.Views
{
    internal class StateFrame : FrameView
    {
        public StateFrame(IVirtualMachine virtualMachine)
        {
            Title = "state";

            var statusLabel = new Label("status:")
            {
                X = 1
            };
            var status = new Label("           ")
            {
                X = Pos.Right(statusLabel) + 1
            };

            Add(statusLabel);
            Add(status);

            var modeLabel = new Label("mode:")
            {
                X = Pos.Right(status) + 2
            };
            var mode = new Label("    ")
            {
                X = Pos.Right(modeLabel) + 1
            };

            Add(modeLabel);
            Add(mode);

            var instructionLabel = new Label("instruction:")
            {
                X = Pos.Right(mode) + 2
            };
            var instruction = new Label("        ")
            {
                X = Pos.Right(instructionLabel) + 1
            };

            Add(instructionLabel);
            Add(instruction);

            var programCounterLabel = new Label("pc:")
            {
                X = Pos.Right(instruction) + 2
            };
            var programCounter = new Label("00000")
            {
                X = Pos.Right(programCounterLabel) + 1
            };

            Add(programCounterLabel);
            Add(programCounter);

            var stackPointerLabel = new Label("sp:")
            {
                X = Pos.Right(programCounter) + 2
            };
            var stackPointer = new Label((Constants.MemoryCapacity - 1).ToString())
            {
                X = Pos.Right(stackPointerLabel) + 1
            };

            Add(stackPointerLabel);
            Add(stackPointer);

            virtualMachine.InstructionExecuting += (object? _, InstructionExecutingEventArgs instructionExecuting)
                => instruction.Text = instructionExecuting.Operation?.GetDescription();
            virtualMachine.VirtualMachineReset += (_, _)
                => instruction.Text = string.Empty;
            virtualMachine.StackPointerUpdated += (object? _, ushort stackPointerUpdated)
                => stackPointer.Text = stackPointerUpdated.ToString().PadLeft(5, '0');
            virtualMachine.ProgramCounterUpdated += (object? _, ushort programCounterUpdated)
                => programCounter.Text = programCounterUpdated.ToString().PadLeft(5, '0');
            virtualMachine.ModeChanged += (object? _, ModeChangedEventArgs modeChanged)
                => mode.Text = modeChanged.NewMode.GetDescription();
            virtualMachine.StatusChanged += (object? _, StatusChangedEventArgs statusChanged)
                => status.Text = statusChanged.NewStatus.GetDescription();
        }
    }
}
