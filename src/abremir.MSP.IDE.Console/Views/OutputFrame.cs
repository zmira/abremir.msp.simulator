using System;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine.Models;
using Terminal.Gui;

namespace abremir.MSP.IDE.Console.Views
{
    internal class OutputFrame : FrameView
    {
        public OutputFrame(SimulatorManager simulatorManager)
        {
            Title = "output";

            var outputTextView = new TextView
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                BottomOffset = 1,
                RightOffset = 1,
                ReadOnly = true,
                Multiline = true
            };

            outputTextView.ContextMenu.MenuItems = new MenuBarItem();

            Add(outputTextView);

            var outputScrollBar = new ScrollBarView(outputTextView, true);

            outputScrollBar.ChangedPosition += () =>
            {
                outputTextView.TopRow = outputScrollBar.Position;
                if (outputTextView.TopRow != outputScrollBar.Position)
                {
                    outputScrollBar.Position = outputTextView.TopRow;
                }
                outputTextView.SetNeedsDisplay();
            };

            outputScrollBar.OtherScrollBarView.ChangedPosition += () =>
            {
                outputTextView.LeftColumn = outputScrollBar.OtherScrollBarView.Position;
                if (outputTextView.LeftColumn != outputScrollBar.OtherScrollBarView.Position)
                {
                    outputScrollBar.OtherScrollBarView.Position = outputTextView.LeftColumn;
                }
                outputTextView.SetNeedsDisplay();
            };

            outputScrollBar.VisibleChanged += () =>
            {
                if (outputScrollBar.Visible && outputTextView.RightOffset == 0)
                {
                    outputTextView.RightOffset = 1;
                }
                else if (!outputScrollBar.Visible && outputTextView.RightOffset == 1)
                {
                    outputTextView.RightOffset = 0;
                }
            };

            outputScrollBar.OtherScrollBarView.VisibleChanged += () =>
            {
                if (outputScrollBar.OtherScrollBarView.Visible && outputTextView.BottomOffset == 0)
                {
                    outputTextView.BottomOffset = 1;
                }
                else if (!outputScrollBar.OtherScrollBarView.Visible && outputTextView.BottomOffset == 1)
                {
                    outputTextView.BottomOffset = 0;
                }
            };

            outputTextView.DrawContent += (_) =>
            {
                outputScrollBar.Size = outputTextView.Lines;
                outputScrollBar.Position = outputTextView.TopRow;
                if (outputScrollBar.OtherScrollBarView != null)
                {
                    outputScrollBar.OtherScrollBarView.Size = outputTextView.Maxlength;
                    outputScrollBar.OtherScrollBarView.Position = outputTextView.LeftColumn;
                }
                outputScrollBar.LayoutSubviews();
                outputScrollBar.Refresh();
            };

            simulatorManager.VirtualMachine.OutputEmitted += (object? _, OutputEmittedEventArgs outputEmitted) =>
            {
                outputTextView.Text += FromVirtualMachineOutput(outputEmitted);
                outputTextView.MoveEnd();
            };
            simulatorManager.OutputEmitted += (string text) =>
            {
                outputTextView.Text += text;
                outputTextView.MoveEnd();
            };
            simulatorManager.VirtualMachine.VirtualMachineReset += (_, _) => outputTextView.Text = string.Empty;
            simulatorManager.VirtualMachine.VirtualMachineHalted += (object? _, VirtualMachineHaltedEventArgs virtualMachineHalted) =>
            {
                outputTextView.Text += $"{Environment.NewLine}Virtual machine halted - {virtualMachineHalted.Reason.GetDescription()}";
                outputTextView.MoveEnd();
            };
        }

        private static string FromVirtualMachineOutput(OutputEmittedEventArgs outputEmitted)
        {
            if (outputEmitted.IsCharacter && outputEmitted.Value == 10)
            {
                return Environment.NewLine;
            }

            return outputEmitted.IsCharacter
                ? Convert.ToChar(outputEmitted.Value).ToString()
                : outputEmitted.Value.ToString();
        }
    }
}
