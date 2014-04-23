using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAlex.PowerCalc.ViewModels.Matrices
{
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
    }
}
