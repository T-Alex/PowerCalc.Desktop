using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAlex.PowerCalc.ViewModels.WorksheetMatrix
{
    public class DataRow
    {
        private DataCell[] _cells;

        public int RowNumber { get; set; }

        public DataCell this[int i]
        {
            get
            {
                return _cells[i];
            }

            set
            {
                _cells[i] = value;
            }
        }


        public DataRow(DataTable dataTable, int cellCount)
        {
            _cells = new DataCell[cellCount];

            for (int i = 0; i < cellCount; i++)
                _cells[i] = new DataCell(dataTable);
        }
    }
}
