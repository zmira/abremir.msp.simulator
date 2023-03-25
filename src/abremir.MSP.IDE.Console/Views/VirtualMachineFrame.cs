using System;
using abremir.MSP.VirtualMachine.Enums;
using abremir.MSP.VirtualMachine.Models;
using Terminal.Gui;

namespace abremir.MSP.IDE.Console.Views
{
    internal class VirtualMachineFrame : FrameView
    {
        public VirtualMachineFrame(SimulatorManager simulatorManager)
        {
            Title = "virtual machine";

            var runButton = new Button("run") { X = 1, Enabled = false };
            var pauseButton = new Button("pause") { X = Pos.Right(runButton) + 1, Enabled = false };
            var stepButton = new Button("step") { X = Pos.Right(pauseButton) + 1, Enabled = false };
            var resetButton = new Button("reset") { X = Pos.Right(stepButton) + 1, Enabled = false };
            var millisecondsPerInstruction = new NumericUpDown
            {
                Label = "ms/instruction:",
                MinValue = 0,
                MaxValue = 10000,
                Step = 100,
                X = Pos.Right(resetButton) + 1
            };

            Add(runButton);
            Add(pauseButton);
            Add(stepButton);
            Add(resetButton);
            Add(millisecondsPerInstruction);

            var stateFrame = new StateFrame(simulatorManager.VirtualMachine)
            {
                Width = Dim.Fill(),
                Height = Dim.Sized(3),
                Y = 1
            };

            Add(stateFrame);

            var programMemoryFrame = new ProgramMemoryFrame(simulatorManager.VirtualMachine)
            {
                Y = Pos.Bottom(stateFrame),
                Width = Dim.Sized(35),
                Height = Dim.Fill()
            };

            Add(programMemoryFrame);

            var dataMemoryFrame = new DataMemoryFrame(simulatorManager)
            {
                Y = Pos.Bottom(stateFrame),
                X = Pos.Right(programMemoryFrame),
                Width = Dim.Sized(27),
                Height = Dim.Fill()
            };

            Add(dataMemoryFrame);

            var stackFrame = new StackFrame(simulatorManager.VirtualMachine)
            {
                Y = Pos.Bottom(stateFrame),
                X = Pos.Right(dataMemoryFrame),
                Width = Dim.Sized(18),
                Height = Dim.Fill()
            };

            Add(stackFrame);

            DrawContentComplete += (_)
                => millisecondsPerInstruction.X = Pos.AnchorEnd(millisecondsPerInstruction.Frame.Width) - 1;

            runButton.Clicked += () => simulatorManager.VirtualMachine.Run();
            pauseButton.Clicked += () => simulatorManager.VirtualMachine.Suspend();
            stepButton.Clicked += () => simulatorManager.VirtualMachine.Step();
            resetButton.Clicked += () => simulatorManager.Reset();
            millisecondsPerInstruction.ValueChanged += (value)
                => simulatorManager.VirtualMachine.SetInstructionExecutionDuration(TimeSpan.FromMilliseconds(value));

            simulatorManager.VirtualMachine.StatusChanged += (object? _, StatusChangedEventArgs statusChanged)
                => pauseButton.Enabled = statusChanged.NewStatus is Status.Running && simulatorManager.VirtualMachine.Mode is Mode.Run;
            simulatorManager.VirtualMachine.ModeChanged += (object? _, ModeChangedEventArgs modeChanged)
                => pauseButton.Enabled = modeChanged.NewMode is Mode.Run && simulatorManager.VirtualMachine.Status is Status.Running;
            simulatorManager.VirtualMachine.ProgramCounterUpdated += (object? _, ushort programCounter)
                => resetButton.Enabled = programCounter != 0;
            simulatorManager.VirtualMachine.VirtualMachineMemorySet += (_, _) =>
            {
                runButton.Enabled = true;
                stepButton.Enabled = true;
            };
        }
    }
}
