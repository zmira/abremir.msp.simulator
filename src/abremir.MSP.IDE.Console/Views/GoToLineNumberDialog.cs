using System.Linq;
using Terminal.Gui;

namespace abremir.MSP.IDE.Console.Views
{
    internal class GoToLineNumberDialog : Dialog
    {
        public bool Cancelled { get; private set; }
        public int? Value { get; private set; }

        private readonly Key[] AllowedKeys = new[]
{
            Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9,
            Key.Backspace, Key.Delete, Key.Home, Key.End, Key.CursorLeft, Key.CursorRight,
            Key.DeleteChar, Key.AltMask | Key.X, Key.Enter, Key.Esc
        };

        public GoToLineNumberDialog(int maxValue)
        {
            Title = "Input Requested";
            Border.Effect3D = false;
            ButtonAlignment = ButtonAlignments.Center;
            Height = 5;
            Width = Dim.Percent(30);

            var okButton = new Button("ok");
            var cancelButton = new Button("cancel", true);

            var inputLabel = new Label("Enter a new line number:");
            var inputTextField = new TextField
            {
                X = Pos.Right(inputLabel) + 1,
                Width = 5
            };

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

                if (string.IsNullOrEmpty(newText))
                {
                    return;
                }

                _ = int.TryParse(newText, out int intValue);

                if (intValue < 0 || intValue > maxValue)
                {
                    textChangingEventArgs.Cancel = true;
                }
            };

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
                    Value = int.Parse(inputTextField.Text.ToString()!);
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
