using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.MathCore;
using TAlex.MathCore.LinearAlgebra;
using TAlex.PowerCalc.ViewModels.Matrices;


namespace TAlex.PowerCalc.ViewModels.WorksheetMatrix
{
    public class DataArray : DataUnit
    {
        #region Fields

        private string _expression;
        private Object _cachedValue;

        #endregion

        #region Properties

        public override string Expression
        {
            get
            {
                return _expression;
            }

            set
            {
                _expression = value;
            }
        }

        public override Object CachedValue
        {
            get
            {
                if (_cachedValue == null)
                {
                    try
                    {
                        _cachedValue = EvaluateExpression();
                    }
                    catch (Exception exc)
                    {
                        _cachedValue = exc;
                    }
                    
                    OnCachedValueChanged();
                }

                return _cachedValue;
            }
        }

        public DataCell[,] Array { get; set; }

        #endregion

        #region Constructors

        public DataArray(DataCell currentCell, IList<DataCellInfo> cells)
        {
            Expression = currentCell.Expression;
            DataTable = currentCell.DataTable;
            ValidateCellsForArray(currentCell.DataTable, cells);
            GetAllUniqueArrays(GetDataCells(DataTable, cells)).ForEach(arr => arr.Clear());

            // Define bounds of new array
            DataCellInfo firstCell = cells[0];
            DataCellInfo lastCell = cells[cells.Count - 1];

            int rows = Math.Abs(firstCell.RowIndex - lastCell.RowIndex) + 1;
            int cols = Math.Abs(firstCell.ColumnIndex - lastCell.ColumnIndex) + 1;
            int y = Math.Min(firstCell.RowIndex, lastCell.RowIndex);
            int x = Math.Min(firstCell.ColumnIndex, lastCell.ColumnIndex);

            Array = new DataCell[rows, cols];
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    DataCell cell = DataTable[y + row, x + col];
                    Array[row, col] = cell;
                    cell.Parent = this;
                    cell.RefreshValue();
                }
            }
        }

        #endregion

        #region Methods

        public object FindValue(DataCell cell)
        {
            object cachedValue = CachedValue;

            if (cachedValue is CMatrix)
            {
                CMatrix matrix = cachedValue as CMatrix;
                for (int i = 0; i < Array.GetLength(0); i++)
                {
                    for (int j = 0; j < Array.GetLength(1); j++)
                    {
                        if (this.Array[i, j] == cell)
                        {
                            if (i < matrix.RowCount && j < matrix.ColumnCount)
                                return matrix[i, j];
                            else
                                return new MatrixIndexOutOfRangeException(Properties.Resources.EXC_MatrixIndexOutOfRange);
                        }
                    }
                }
            }
            else if (cachedValue is Complex || cachedValue is Exception)
            {
                return cachedValue;
            }

            throw new ArgumentException();
        }

        protected override void CachedValueChangedHandler(object sender, EventArgs e)
        {
            _cachedValue = null;
            for (int i = 0; i < Array.GetLength(0); i++)
            {
                for (int j = 0; j < Array.GetLength(1); j++)
                {
                    DataCell cell = Array[i, j];
                    cell.RefreshValue();
                    cell.RaiseCachedValueChanged();
                }
            }
        }

        protected virtual void Clear()
        {
            UnsubscripeReferences();
            foreach (DataCell cell in Array)
            {
                cell.Parent = null;
            }
            Array = new DataCell[,] { };
            Expression = null;
        }

        #region Helpers

        private List<DataArray> GetAllUniqueArrays(IList<DataCell> dataCells)
        {
            return dataCells.Select(x => x.Parent).Where(x => x != null).Distinct().ToList();
        }

        private IList<DataCell> GetDataCells(DataTable dataTable, IList<DataCellInfo> cells)
        {
            return cells.Select(x => dataTable[x.RowIndex, x.ColumnIndex]).ToList();
        }

        private void ValidateCellsForArray(DataTable dataTable, IList<DataCellInfo> cells)
        {
            EnsureRectangularity(cells);
            EnsureAllArraysEnclosing(dataTable, cells);
        }

        private void EnsureRectangularity(IList<DataCellInfo> cells)
        {
            int minRow = cells.Min(x => x.RowIndex);
            int maxRow = cells.Max(x => x.RowIndex);
            int minCol = cells.Min(x => x.ColumnIndex);
            int maxCol = cells.Max(x => x.ColumnIndex);

            int cellsInRect = (maxRow - minRow + 1) * (maxCol - minCol + 1);

            if (cellsInRect != cells.Count)
            {
                throw new ArgumentException("The array must be rectangular.");
            }

            bool isAllCellInsideRect = cells.All(x =>
                x.RowIndex >= minRow
                && x.RowIndex <= maxRow
                && x.ColumnIndex >= minCol
                && x.ColumnIndex <= maxCol);

            if (!isAllCellInsideRect)
            {
                throw new ArgumentException("The array must be rectangular.");
            }
        }

        private void EnsureAllArraysEnclosing(DataTable dataTable, IList<DataCellInfo> cells)
        {
            IList<DataCell> dataCells = GetDataCells(dataTable, cells);
            IList<DataArray> allArrays = GetAllUniqueArrays(dataCells);

            bool isValid = allArrays.All(a => IsSequenceContainsArray(a, dataCells));

            if (!isValid)
            {
                throw new ArgumentException("You cannot change part of an array.");
            }
        }

        private bool IsSequenceContainsArray(DataArray array, IList<DataCell> dataCells)
        {
            foreach (var x in array.Array)
            {
                if (!dataCells.Contains(x)) return false;
            }
            return true;
        }

        #endregion

        #endregion
    }

    public class MatrixIndexOutOfRangeException : Exception
    {
        public MatrixIndexOutOfRangeException()
            : base()
        {
        }

        public MatrixIndexOutOfRangeException(string message)
            : base(message)
        {
        }
    }
}
