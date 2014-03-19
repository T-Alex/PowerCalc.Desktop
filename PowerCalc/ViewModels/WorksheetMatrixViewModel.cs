using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;


namespace TAlex.PowerCalc.ViewModels
{
    public class WorksheetMatrixViewModel
    {
        #region Fields

        public readonly IExpressionTreeBuilder<Object> ExpressionTreeBuilder;

        #endregion

        #region Properties

        public string NameText { get; set; }

        public string FormulaBarText { get; set; }

        public object Worksheet { get; set; }

        #endregion

        #region Constructors

        public WorksheetMatrixViewModel(IExpressionTreeBuilder<Object> expressionTreeBuilder)
        {
            ExpressionTreeBuilder = expressionTreeBuilder;
        }

        #endregion
    }
}
