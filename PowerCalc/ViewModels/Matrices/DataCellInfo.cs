using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAlex.PowerCalc.ViewModels.Matrices
{
    [Serializable]
    public struct DataCellInfo
    {
        public int RowIndex { get; set; }

        public int ColumnIndex { get; set; }


        public DataCellInfo(int rowIndex, int columnIndex)
            : this()
        {
            RowIndex = rowIndex;
            ColumnIndex = columnIndex;
        }


        public DataCellInfo Offset(int rowIndex, int columnIndex)
        {
            return new DataCellInfo(RowIndex + rowIndex, ColumnIndex + columnIndex);
        }

        public DataCellInfo Offset(DataCellInfo cellInfo)
        {
            return new DataCellInfo(RowIndex + cellInfo.RowIndex, ColumnIndex + cellInfo.ColumnIndex);
        }


        public static DataCellInfo operator -(DataCellInfo cellInfo)
        {
            return new DataCellInfo(-cellInfo.RowIndex, -cellInfo.ColumnIndex);
        }
    }
}
