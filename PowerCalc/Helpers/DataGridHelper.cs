using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace TAlex.PowerCalc.Helpers
{
    public static class DataGridHelper
    {
        public static bool HasNonEscapeCharacters(TextCompositionEventArgs textArgs)
        {
            if (textArgs != null)
            {
                string text = textArgs.Text;
                int num = 0;
                int length = text.Length;
                while (num < length)
                {
                    if (text[num] != '\u001B')
                    {
                        return true;
                    }
                    num++;
                }
            }
            return false;
        }


        public static DataGridCell TryToFindGridCell(this DataGrid grid, DataGridCellInfo cellInfo)
        {
            DataGridCell result = null;
            DataGridRow row = (DataGridRow)grid.ItemContainerGenerator.ContainerFromItem(cellInfo.Item);
            if (row != null)
            {
                int columnIndex = grid.Columns.IndexOf(cellInfo.Column);
                if (columnIndex > -1)
                {
                    DataGridCellsPresenter presenter = GetVisualChild<DataGridCellsPresenter>(row);
                    result = presenter.ItemContainerGenerator.ContainerFromIndex(columnIndex) as DataGridCell;
                }
            }
            return result;
        }

        public static DataGridCell TryToFindGridCell(this DataGrid grid, object item, DataGridColumn column)
        {
            return TryToFindGridCell(grid, new DataGridCellInfo(item, column));
        }

        public static DataGridCell GetCurrentDataGridCell(this DataGrid grid)
        {
            return TryToFindGridCell(grid, grid.CurrentCell);
        }


        private static T GetVisualChild<T>(Visual parent) where T : Visual
        {
            T child = default(T);
            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }
    }
}
