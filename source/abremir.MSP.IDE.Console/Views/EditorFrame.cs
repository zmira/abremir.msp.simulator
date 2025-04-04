using System;
using abremir.MSP.Shared.Constants;
using Terminal.Gui;

namespace abremir.MSP.IDE.Console.Views
{
    internal class EditorFrame : FrameView
    {
        public string? SourceCode => _editorTextView?.Text.ToString();
        public int Lines => _editorTextView.Lines;
        public bool IsDirty => _editorTextView.IsDirty;

        private const string FrameTitle = "editor";
        private readonly TextView _editorTextView;

        public EditorFrame(SimulatorManager simulatorManager, StatusItem cursorPosition)
        {
            Title = FrameTitle;

            _editorTextView = new TextView
            {
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2,
                BottomOffset = 1,
                RightOffset = 1,
                TabWidth = 2
            };

            _editorTextView.UnwrappedCursorPosition += (e) => cursorPosition.Title = $"Ln {e.Y + 1}, Col {e.X + 1}";

            Add(_editorTextView);

            var compileButton = new Button("compile") { X = 1, Y = Pos.Bottom(_editorTextView) + 1 };

            Add(compileButton);

            var editorScrollBar = new ScrollBarView(_editorTextView, true);

            editorScrollBar.ChangedPosition += () =>
            {
                _editorTextView.TopRow = editorScrollBar.Position;
                if (_editorTextView.TopRow != editorScrollBar.Position)
                {
                    editorScrollBar.Position = _editorTextView.TopRow;
                }
                _editorTextView.SetNeedsDisplay();
            };

            editorScrollBar.OtherScrollBarView.ChangedPosition += () =>
            {
                _editorTextView.LeftColumn = editorScrollBar.OtherScrollBarView.Position;
                if (_editorTextView.LeftColumn != editorScrollBar.OtherScrollBarView.Position)
                {
                    editorScrollBar.OtherScrollBarView.Position = _editorTextView.LeftColumn;
                }
                _editorTextView.SetNeedsDisplay();
            };

            editorScrollBar.VisibleChanged += () =>
            {
                if (editorScrollBar.Visible && _editorTextView.RightOffset == 0)
                {
                    _editorTextView.RightOffset = 1;
                }
                else if (!editorScrollBar.Visible && _editorTextView.RightOffset == 1)
                {
                    _editorTextView.RightOffset = 0;
                }
            };

            editorScrollBar.OtherScrollBarView.VisibleChanged += () =>
            {
                if (editorScrollBar.OtherScrollBarView.Visible && _editorTextView.BottomOffset == 0)
                {
                    _editorTextView.BottomOffset = 1;
                }
                else if (!editorScrollBar.OtherScrollBarView.Visible && _editorTextView.BottomOffset == 1)
                {
                    _editorTextView.BottomOffset = 0;
                }
            };

            _editorTextView.DrawContent += (_) =>
            {
                editorScrollBar.Size = _editorTextView.Lines;
                editorScrollBar.Position = _editorTextView.TopRow;
                if (editorScrollBar.OtherScrollBarView != null)
                {
                    editorScrollBar.OtherScrollBarView.Size = _editorTextView.Maxlength;
                    editorScrollBar.OtherScrollBarView.Position = _editorTextView.LeftColumn;
                }
                editorScrollBar.LayoutSubviews();
                editorScrollBar.Refresh();

                Title = $"{FrameTitle}{(IsDirty ? " *" : string.Empty)}";
            };

            compileButton.Clicked += () => simulatorManager.Compile(SourceCode);

            _editorTextView.SetFocus();
        }

        private void ResetState()
        {
            _editorTextView.ClearHistoryChanges();
            Title = FrameTitle;
        }

        public void New(string? sourceCode = null)
        {
            sourceCode ??= $"{Constants.DataSegment}{Environment.NewLine}{Constants.CodeSegment}{Environment.NewLine}";

            _editorTextView.Text = sourceCode;

            ResetState();
        }

        public void SourceCodeSaved()
        {
            ResetState();
        }

        public void GoToLine(int line)
        {
            _editorTextView.CursorPosition = new Point(0, line - 1);
        }
    }
}
