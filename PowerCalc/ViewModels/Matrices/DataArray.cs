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
                return _cachedValue ?? EvaluateExpression();
            }
        }

        public DataCell[,] Array { get; set; }

        #endregion


        public DataArray(DataCell currentCell, int x, int y, int rows, int cols)
        {
            Expression = currentCell.Expression;
            DataTable = currentCell.DataTable;
            Array = new DataCell[rows, cols];

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    Array[row, col] = currentCell.DataTable[y + row, x + col];
                    Array[row, col].Parent = this;
                    Array[row, col].RefreshValue();
                }
            }
        }


        public object FindValue(DataCell cell, Object cachedValue)
        {
            if (cachedValue is CMatrix)
            {
                CMatrix matrix = cachedValue as CMatrix;
                for (int i = 0; i < Array.GetLength(0); i++)
                {
                    for (int j = 0; j < Array.GetLength(1); j++)
                    {
                        if (this.Array[i, j] == cell)
                        {
                            return matrix[i, j];
                        }
                    }
                }
            }
            else if (cachedValue is Complex)
            {
                return cachedValue;
            }

            throw new ArgumentException();
        }

        public virtual void Expand(int x, int y, int rows, int cols)
        {

        }
    }
}
