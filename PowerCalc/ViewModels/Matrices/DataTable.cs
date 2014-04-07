using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;


namespace TAlex.PowerCalc.ViewModels.WorksheetMatrix
{
    public class DataTable
    {
        public readonly IExpressionTreeBuilder<Object> ExpressionTreeBuilder;

        public List<DataRow> Rows { get; private set; }

        public DataCell this[int row, int col]
        {
            get
            {
                return Rows[row][col];
            }
        }


        public DataTable(IExpressionTreeBuilder<Object> expressionTreeBuilder)
        {
            Rows = new List<DataRow>();
            ExpressionTreeBuilder = expressionTreeBuilder;
        }
    }
}
