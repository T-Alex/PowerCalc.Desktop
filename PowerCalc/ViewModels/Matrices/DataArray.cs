using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.MathCore;
using TAlex.MathCore.LinearAlgebra;


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

        public DataArray(DataCell currentCell, int x, int y, int rows, int cols)
        {
            Expression = currentCell.Expression;
            DataTable = currentCell.DataTable;
            Array = new DataCell[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    DataCell cell = currentCell.DataTable[y + row, x + col];
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

        public virtual void Expand(int x, int y, int rows, int cols)
        {

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
