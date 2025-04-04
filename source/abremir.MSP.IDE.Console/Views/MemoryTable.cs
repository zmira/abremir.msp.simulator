using Terminal.Gui;

namespace abremir.MSP.IDE.Console.Views
{
    internal class MemoryTable : TableView
    {
        public void SelectTableRow(int row)
        {
            SelectedRow = row;
            Update();
        }
    }
}
