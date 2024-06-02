using System;
using System.Linq;
using abremir.MSP.VirtualMachine.Models;
using Terminal.Gui;

namespace abremir.MSP.IDE.Console.Views
{
    internal class InputRequestDialog : Dialog
    {
        public bool Cancelled { get; private set; }
        public string? Value { get; private set; }

        private readonly Key[] AllowedKeys =
        [
            Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9,
            Key.Backspace, Key.Delete, Key.Home, Key.End, Key.CursorLeft, Key.CursorRight,
            Key.DeleteChar, (Key)45 /* minus */, Key.AltMask | Key.X, Key.Enter, Key.Esc
        ];

        public InputRequestDialog(InputRequestedEventArgs inputRequestedEventArgs)
        {
            Title = "Input Requested";
            Border.Effect3D = false;
            ButtonAlignment = ButtonAlignments.Center;
            Height = 5;
            Width = Dim.Percent(30);

            var okButton = new Button("ok");
            var cancelButton = new Button("cancel", true);

            var labelContext = inputRequestedEventArgs.IsCharacter
                ? "character"
                : $"number [{sbyte.MinValue}..{sbyte.MaxValue}]";

            var inputLabel = new Label($"Enter a {labelContext}:");
            var inputTextField = new TextField
            {
                X = Pos.Right(inputLabel) + 1
            };

            if (inputRequestedEventArgs.IsCharacter)
            {
                inputLabel.X = 6;
                inputTextField.Width = 2;

                inputTextField.KeyPress += (KeyEventEventArgs keyEventArgs) =>
                {
                    if (keyEventArgs.KeyEvent.Key is Key.Enter)
                    {
                        keyEventArgs.Handled = true;

                        okButton.OnClicked();
                    }
                };

                inputTextField.TextChanging += (TextChangingEventArgs textChangingEventArgs) =>
                {
                    var newText = textChangingEventArgs.NewText.ToString();

                    if (newText?.Length > 1
                        || (newText?.Length == 1 && !char.IsAscii(newText[0])))
                    {
                        textChangingEventArgs.Cancel = true;
                    }
                };
            }
            else
            {
                inputTextField.Width = 5;

                inputTextField.KeyPress += (KeyEventEventArgs keyEventArgs) =>
                {
                    var key = keyEventArgs.KeyEvent.Key;

                    if (!AllowedKeys.Contains(key))
                    {
                        keyEventArgs.Handled = true;
                    }

                    if (key is Key.Enter && int.TryParse(inputTextField.Text.ToString(), out _))
                    {
                        keyEventArgs.Handled = true;

                        okButton.OnClicked();
                    }

                    if (key is Key.Esc)
                    {
                        keyEventArgs.Handled = true;

                        cancelButton.OnClicked();
                    }
                };

                inputTextField.TextChanging += (TextChangingEventArgs textChangingEventArgs) =>
                {
                    var newText = textChangingEventArgs.NewText.ToString();

                    if (string.IsNullOrEmpty(newText) || newText is "-")
                    {
                        return;
                    }

                    _ = int.TryParse(newText, out int intValue);

                    if (intValue < sbyte.MinValue || intValue > sbyte.MaxValue)
                    {
                        textChangingEventArgs.Cancel = true;
                    }
                };
            }

            cancelButton.Clicked += () =>
            {
                Value = null;
                Cancelled = true;
                Application.RequestStop(this);
            };

            okButton.Clicked += () =>
            {
                if (inputTextField.Text.Length > 0)
                {
                    Value = inputTextField.Text.ToString();
                    Application.RequestStop(this);
                }
            };

            Add(inputLabel);
            Add(inputTextField);
            AddButton(cancelButton);
            AddButton(okButton);

            inputTextField.SetFocus();
        }
    }
}
