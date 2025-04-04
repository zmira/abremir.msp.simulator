using System;
using System.Linq;

namespace Terminal.Gui
{
    public class NumericUpDown : View
    {
        public byte ValueWidth { get; set; } = 1;
        public ushort Step { get; set; } = 1;
        public int MaxValue { get; set; } = 100;
        public int MinValue { get; set; }
        public int Value { get; set; }
        public string Label { get; set; } = string.Empty;

        public event Action<int>? ValueChanged;

        private readonly Key[] AllowedKeys =
        [
            Key.D0, Key.D1, Key.D2, Key.D3, Key.D4, Key.D5, Key.D6, Key.D7, Key.D8, Key.D9,
            Key.Backspace, Key.Delete, Key.Home, Key.End, Key.CursorLeft, Key.CursorRight,
            Key.CursorUp, Key.CursorDown, Key.CtrlMask | Key.Home, Key.CtrlMask | Key.End,
            Key.DeleteChar, (Key)45 /* minus */, Key.AltMask | Key.X
        ];

        public NumericUpDown()
        {
            Height = 1;

            Initialized += (_, _) =>
            {
                if (MaxValue.ToString().Length > ValueWidth || MinValue.ToString().Length > ValueWidth)
                {
                    ValueWidth = MinValue.ToString().Length > MaxValue.ToString().Length
                        ? (byte)MinValue.ToString().Length
                        : (byte)MaxValue.ToString().Length;
                }

                if (Value < MinValue)
                {
                    Value = MinValue;
                }

                if (Value > MaxValue)
                {
                    Value = MaxValue;
                }

                Label? label = null;

                if (!string.IsNullOrEmpty(Label))
                {
                    label = new Label(Label);
                }

                var downLabel = new Label("▼");

                if (label is not null)
                {
                    downLabel.X = Pos.Right(label) + 1;
                }

                var value = new TextField
                {
                    X = Pos.Right(downLabel) + 1,
                    Width = ValueWidth
                };

                var upLabel = new Label("▲")
                {
                    X = Pos.Right(value) + 1
                };

                if (label is not null)
                {
                    Add(label);
                }

                Add(downLabel);
                Add(value);
                Add(upLabel);

                Width = (label?.Frame.Width ?? 0)
                    + downLabel.Frame.Width
                    + value.Frame.Width
                    + upLabel.Frame.Width
                    + 2
                    + (label is not null ? 1 : 0);

                value.MouseClick += (MouseEventArgs mouseEventArgs) =>
                {
                    if ((mouseEventArgs.MouseEvent.Flags & MouseFlags.WheeledDown) is MouseFlags.WheeledDown)
                    {
                        DecrementValue();
                        mouseEventArgs.Handled = true;
                    }
                    else if ((mouseEventArgs.MouseEvent.Flags & MouseFlags.WheeledUp) is MouseFlags.WheeledUp)
                    {
                        IncrementValue();
                        mouseEventArgs.Handled = true;
                    }
                };

                value.KeyPress += (KeyEventEventArgs keyEventArgs) =>
                {
                    var pressedKey = keyEventArgs.KeyEvent.Key;

                    if (!AllowedKeys.Contains(pressedKey))
                    {
                        keyEventArgs.Handled = true;
                    }

                    switch (keyEventArgs.KeyEvent.Key)
                    {
                        case Key.CursorDown:
                            DecrementValue();
                            keyEventArgs.Handled = true;
                            break;
                        case Key.CursorUp:
                            IncrementValue();
                            keyEventArgs.Handled = true;
                            break;
                        case Key.CtrlMask | Key.Home:
                            SetValue(MinValue);
                            keyEventArgs.Handled = true;
                            break;
                        case Key.CtrlMask | Key.End:
                            SetValue(MaxValue);
                            keyEventArgs.Handled = true;
                            break;
                        case (Key)45:
                            if (MinValue >= 0)
                            {
                                keyEventArgs.Handled = true;
                            }
                            break;
                        case Key.CtrlMask | Key.Q:
                            InvokeKeybindings(keyEventArgs.KeyEvent);
                            break;
                    }
                };

                value.TextChanging += (TextChangingEventArgs textChangingEventArgs) =>
                {
                    var newText = textChangingEventArgs.NewText.ToString();

                    if (string.IsNullOrEmpty(newText) || newText is "-")
                    {
                        return;
                    }

                    _ = int.TryParse(newText, out int intValue);

                    if (intValue < MinValue || intValue > MaxValue)
                    {
                        textChangingEventArgs.Cancel = true;
                    }
                };

                value.TextChanged += (_) =>
                {
                    if (int.TryParse(value.Text.ToString(), out int intValue))
                    {
                        ValueChanged?.Invoke(intValue);
                    }
                };

                downLabel.MouseClick += (MouseEventArgs mouseEventArgs) =>
                {
                    if ((mouseEventArgs.MouseEvent.Flags & MouseFlags.Button1Clicked) is MouseFlags.Button1Clicked
                        || (mouseEventArgs.MouseEvent.Flags & MouseFlags.Button1DoubleClicked) is MouseFlags.Button1DoubleClicked
                        || (mouseEventArgs.MouseEvent.Flags & MouseFlags.Button1TripleClicked) is MouseFlags.Button1TripleClicked)
                    {
                        DecrementValue();
                        mouseEventArgs.Handled = true;
                    }
                };

                upLabel.MouseClick += (MouseEventArgs mouseEventArgs) =>
                {
                    if ((mouseEventArgs.MouseEvent.Flags & MouseFlags.Button1Clicked) is MouseFlags.Button1Clicked
                        || (mouseEventArgs.MouseEvent.Flags & MouseFlags.Button1DoubleClicked) is MouseFlags.Button1DoubleClicked
                        || (mouseEventArgs.MouseEvent.Flags & MouseFlags.Button1TripleClicked) is MouseFlags.Button1TripleClicked)
                    {
                        IncrementValue();
                        mouseEventArgs.Handled = true;
                    }
                };

                void IncrementValue()
                {
                    _ = int.TryParse(value.Text.ToString(), out int intValue);

                    intValue += Step;

                    if (intValue > MaxValue)
                    {
                        intValue = MaxValue;
                    }

                    SetValue(intValue);
                }

                void DecrementValue()
                {
                    _ = int.TryParse(value.Text.ToString(), out int intValue);

                    intValue -= Step;

                    if (intValue < MinValue)
                    {
                        intValue = MinValue;
                    }

                    SetValue(intValue);
                }

                void SetValue(int intValue)
                {
                    Value = intValue;

                    value!.Text = Value.ToString();
                }

                value.Text = Value.ToString();
            };
        }
    }
}
