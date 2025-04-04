using System.Data;
using System.Linq;
using abremir.MSP.Shared.Constants;
using abremir.MSP.VirtualMachine.Models;
using Terminal.Gui;

namespace abremir.MSP.IDE.Console.Views
{
    internal class DataMemoryFrame : FrameView
    {
        private readonly DataTable _dataMemory;
        private readonly MemoryTable _dataMemoryTable;

        private const string ValueColumnName = "value";
        private const string VariableColumnName = "variable";

        public DataMemoryFrame(SimulatorManager simulatorManager)
        {
            Title = "data memory";

            _dataMemory = new DataTable();
            _dataMemory.Columns.Add("address");
            _dataMemory.Columns.Add(ValueColumnName);
            _dataMemory.Columns.Add(VariableColumnName);

            _dataMemoryTable = new MemoryTable
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                Table = _dataMemory
            };

            ClearDataMemory();

            Add(_dataMemoryTable);

            var dataMemoryScrollBar = new ScrollBarView(_dataMemoryTable, true);

            dataMemoryScrollBar.ChangedPosition += () =>
            {
                _dataMemoryTable.RowOffset = dataMemoryScrollBar.Position;
                if (_dataMemoryTable.RowOffset != dataMemoryScrollBar.Position)
                {
                    dataMemoryScrollBar.Position = _dataMemoryTable.RowOffset;
                }
                _dataMemoryTable.SetNeedsDisplay();
            };

            _dataMemoryTable.DrawContent += (_) =>
            {
                dataMemoryScrollBar.Size = _dataMemoryTable.Table?.Rows?.Count ?? 0;
                dataMemoryScrollBar.Position = _dataMemoryTable.RowOffset;
                dataMemoryScrollBar.Refresh();
            };

            simulatorManager.VirtualMachine.VirtualMachineMemorySet += (_, _) =>
            {
                ClearDataMemory();

                var index = 0;
                foreach (var value in simulatorManager.VirtualMachine.Data)
                {
                    _dataMemory.Rows[index][ValueColumnName] = value.ToString().PadLeft(3, '0');

                    index++;
                }

                foreach (var variableMap in simulatorManager.DataAddressVariableMap)
                {
                    _dataMemory.Rows[variableMap.Address][VariableColumnName] = variableMap.Variable;
                }
            };

            simulatorManager.VirtualMachine.DataMemoryUpdated += (object? _, DataMemoryUpdatedEventArgs dataMemoryUpdated) =>
            {
                _dataMemory.Rows[dataMemoryUpdated.Address][ValueColumnName] = dataMemoryUpdated.Value;

                _dataMemoryTable.SelectTableRow(dataMemoryUpdated.Address);
            };
        }

        private void ClearDataMemory()
        {
            _dataMemory.Clear();

            Enumerable
                .Range(0, Constants.MemoryCapacity)
                .Select(val => new[] { val.ToString().PadLeft(5, '0'), "0".PadLeft(3, '0'), string.Empty })
                .ToList()
                .ForEach(val => _dataMemory.Rows.Add(val));

            _dataMemoryTable.SelectTableRow(-1);
        }
    }
}
