using System.Data;
using System.Linq;
using abremir.MSP.Shared.Constants;
using abremir.MSP.VirtualMachine;
using Terminal.Gui;

namespace abremir.MSP.IDE.Console.Views
{
    internal class StackFrame : FrameView
    {
        private readonly DataTable _stackMemory;
        private readonly MemoryTable _stackTable;

        public StackFrame(IVirtualMachine virtualMachine)
        {
            Title = "stack";

            _stackMemory = new DataTable();
            _stackMemory.Columns.Add("address");
            _stackMemory.Columns.Add("value");

            _stackTable = new MemoryTable
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                Table = _stackMemory
            };

            ClearStack();

            Add(_stackTable);

            var stackScrollBar = new ScrollBarView(_stackTable, true);

            stackScrollBar.ChangedPosition += () =>
            {
                _stackTable.RowOffset = stackScrollBar.Position;
                if (_stackTable.RowOffset != stackScrollBar.Position)
                {
                    stackScrollBar.Position = _stackTable.RowOffset;
                }
                _stackTable.SetNeedsDisplay();
            };

            _stackTable.DrawContent += (_) =>
            {
                stackScrollBar.Size = _stackTable.Table?.Rows?.Count ?? 0;
                stackScrollBar.Position = _stackTable.RowOffset;
                stackScrollBar.Refresh();
            };

            virtualMachine.VirtualMachineReset += (_, _) => ClearStack();

            virtualMachine.StackPointerUpdated += (object? _, ushort stackPointer) =>
            {
                if (virtualMachine.Stack.Count is 0)
                {
                    ClearStack();

                    return;
                }

                var stack = virtualMachine.Stack.ToList();
                for (var i = Constants.MemoryCapacity - 1; i > virtualMachine.SP; i--)
                {
                    _stackMemory.Rows[i]["value"] = stack[i - virtualMachine.SP - 1].ToString().PadLeft(3, '0');
                }
                _stackMemory.Rows[stackPointer]["value"] = "0".PadLeft(3, '0');

                _stackTable.SelectTableRow(stackPointer);
            };
        }

        private void ClearStack()
        {
            _stackMemory.Clear();

            Enumerable
                .Range(0, Constants.MemoryCapacity)
                .Select(val => new[] { val.ToString().PadLeft(5, '0'), "0".PadLeft(3, '0') })
                .ToList()
                .ForEach(val => _stackMemory.Rows.Add(val));

            _stackTable.SelectTableRow(Constants.MemoryCapacity - 1);
        }
    }
}
