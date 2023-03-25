using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using abremir.MSP.Shared.Constants;
using abremir.MSP.Shared.Enums;
using abremir.MSP.Shared.Extensions;
using abremir.MSP.VirtualMachine;
using Terminal.Gui;

namespace abremir.MSP.IDE.Console.Views
{
    internal class ProgramMemoryFrame : FrameView
    {
        private readonly DataTable _programMemory;
        private readonly MemoryTable _programMemoryTable;
        private List<(int ProgramCounter, int TableRow)> _programCounterTableRowMap = new();

        private const string AddressColumnName = "address";
        private const string OperationCodeColumnName = "opcode";
        private const string InstructionColumnName = "instr";
        private const string Argument1ColumnName = "arg1";
        private const string Argument2ColumnName = "arg2";

        public ProgramMemoryFrame(IVirtualMachine virtualMachine)
        {
            Title = "program memory";

            _programMemory = new DataTable();
            _programMemory.Columns.Add(AddressColumnName);
            _programMemory.Columns.Add(OperationCodeColumnName);
            _programMemory.Columns.Add(InstructionColumnName);
            _programMemory.Columns.Add(Argument1ColumnName);
            _programMemory.Columns.Add(Argument2ColumnName);

            _programMemoryTable = new MemoryTable
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true,
                Table = _programMemory
            };

            ClearProgramMemory();

            Add(_programMemoryTable);

            var programMemoryScrollBar = new ScrollBarView(_programMemoryTable, true);

            programMemoryScrollBar.ChangedPosition += () =>
            {
                _programMemoryTable.RowOffset = programMemoryScrollBar.Position;
                if (_programMemoryTable.RowOffset != programMemoryScrollBar.Position)
                {
                    programMemoryScrollBar.Position = _programMemoryTable.RowOffset;
                }
                _programMemoryTable.SetNeedsDisplay();
            };

            _programMemoryTable.DrawContent += (_) =>
            {
                programMemoryScrollBar.Size = _programMemoryTable.Table?.Rows?.Count ?? 0;
                programMemoryScrollBar.Position = _programMemoryTable.RowOffset;
                programMemoryScrollBar.Refresh();
            };

            virtualMachine.VirtualMachineMemorySet += (_, _) =>
            {
                ClearProgramMemory();

                var tableRow = 0;
                for (var i = 0; i < virtualMachine.Program.Count; i++)
                {
                    _programCounterTableRowMap.Add(new(i, tableRow));
                    _programMemory.Rows[tableRow][AddressColumnName] = i.ToString().PadLeft(5, '0');

                    var operation = virtualMachine.Program.ElementAt(i);
                    _programMemory.Rows[tableRow][OperationCodeColumnName] = operation.ToString().PadLeft(3, '0');

                    var instruction = (Operation)operation;
                    _programMemory.Rows[tableRow][InstructionColumnName] = instruction.GetDescription();

                    var cellsOccupied = instruction.GetNumberOfMemoryCellsOccupied();

                    if (cellsOccupied > 1)
                    {
                        i++;
                        _programMemory.Rows[tableRow][Argument1ColumnName] = virtualMachine.Program.ElementAt(i);

                        if (cellsOccupied > 2)
                        {
                            i++;
                            _programMemory.Rows[tableRow][Argument2ColumnName] = virtualMachine.Program.ElementAt(i);

                            if (cellsOccupied > 3)
                            {
                                throw new ArgumentOutOfRangeException(nameof(cellsOccupied));
                            }
                        }
                    }

                    tableRow++;
                }
            };

            virtualMachine.ProgramCounterUpdated += (object? _, ushort programCounter) =>
            {
                var (ProgramCounter, TableRow) = _programCounterTableRowMap.Find(map => map.ProgramCounter == programCounter);

                _programMemoryTable.SelectTableRow(TableRow);
            };
        }

        private void ClearProgramMemory()
        {
            _programMemory.Clear();
            _programCounterTableRowMap = new();

            Enumerable
                .Range(0, Constants.MemoryCapacity)
                .Select(val => new[] { val.ToString().PadLeft(5, '0'), "0".PadLeft(3, '0'), string.Empty, string.Empty, string.Empty })
                .ToList()
                .ForEach(val => _programMemory.Rows.Add(val));

            _programMemoryTable.SelectTableRow(-1);
        }
    }
}
