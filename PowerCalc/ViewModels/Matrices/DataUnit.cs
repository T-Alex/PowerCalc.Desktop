using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TAlex.MathCore;
using TAlex.MathCore.ExpressionEvaluation.Trees;
using TAlex.MathCore.LinearAlgebra;
using TAlex.WPF.Mvvm;
using TAlex.PowerCalc.Helpers;


namespace TAlex.PowerCalc.ViewModels.WorksheetMatrix
{
    public abstract class DataUnit : ViewModelBase
    {
        #region Fields

        private static Random _rand = new Random();

        #endregion

        #region Properties

        public virtual DataTable DataTable { get; protected set; }

        public virtual string Expression { get; set; }

        public abstract Object CachedValue { get; }

        #endregion

        #region Events

        public event EventHandler CachedValueChanged;

        #endregion

        #region Methods

        protected Object EvaluateExpression()
        {
            string expression = Expression;

            // Preparation variables
            IDictionary<string, Object> vars = new Dictionary<string, object>();

            MatchCollection referenceMatches = Regex.Matches(expression, Helpers.A1ReferenceHelper.A1ReferenceRangeOfCellsPattern);
            List<string> references = new List<string>();

            foreach (Match match in referenceMatches)
            {
                string reference = match.Value;

                if (!references.Contains(reference))
                    references.Add(reference);
            }

            references.Sort(StringLengthComparison);

            foreach (string reference in references)
            {
                string varName = GetRandomVariableName();
                vars.Add(varName, GetRangeOfCellValues(reference));
                expression = expression.Replace(reference, varName);
            }

            referenceMatches = Regex.Matches(expression, Helpers.A1ReferenceHelper.A1ReferenceSingleCellPattern);
            references.Clear();

            foreach (Match match in referenceMatches)
            {
                string reference = match.Value;

                if (!references.Contains(reference))
                    references.Add(reference);
            }

            references.Sort(StringLengthComparison);

            foreach (string reference in references)
            {
                string varName = GetRandomVariableName();
                vars.Add(varName, GetSingleCellValue(reference));
                expression = expression.Replace(reference, varName);
            }

            // Evaluation the expression
            Expression<Object> expr = DataTable.ExpressionTreeBuilder.BuildTree(expression);
            expr.SetAllVariables(vars);
            return expr.Evaluate();
        }

        protected virtual void OnCachedValueChanged()
        {
            if (CachedValueChanged != null)
            {
                CachedValueChanged(this, new EventArgs());
            }
        }

        private static string GetRandomVariableName()
        {
            return GetRandomVariableName(15);
        }

        private static string GetRandomVariableName(int length)
        {
            string name = String.Empty;

            for (int i = 0; i < length; i++)
            {
                bool upperCase = !(_rand.Next(0, 2) == 0);
                name += (Char)((upperCase ? 'A' : 'a') + _rand.Next(0, 26));
            }

            return name;
        }

        private int StringLengthComparison(string str1, string str2)
        {
            int len1 = str1.Length;
            int len2 = str2.Length;

            if (len1 < len2) return 1;
            else if (len1 > len2) return -1;
            else return 0;
        }

        private Complex GetSingleCellValue(string a1Reference)
        {
            int row, column;
            Helpers.A1ReferenceHelper.Parse(a1Reference, out column, out row);

            DataCell cell = DataTable[row, column];
            cell.CachedValueChanged -= CachedValueChangedHandler;
            cell.CachedValueChanged += CachedValueChangedHandler;
            return (Complex)cell.CachedValue;
        }

        protected abstract void CachedValueChangedHandler(object sender, EventArgs e);

        private CMatrix GetRangeOfCellValues(string a1Reference)
        {
            int row1Idx, col1Idx, row2Idx, col2Idx;
            Helpers.A1ReferenceHelper.Parse(a1Reference, out col1Idx, out row1Idx, out col2Idx, out row2Idx);

            int n = row2Idx - row1Idx;
            int m = col2Idx - col1Idx;

            CMatrix matrix = new CMatrix(n, m);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    DataCell cell = DataTable[i + row1Idx, j + col1Idx];
                    cell.CachedValueChanged -= CachedValueChangedHandler;
                    cell.CachedValueChanged += CachedValueChangedHandler;
                    if (cell.CachedValue != null)
                        matrix[i, j] = (Complex)cell.CachedValue;
                    else
                        matrix[i, j] = Complex.Zero;
                }
            }

            return matrix;
        }

        #endregion
    }
}
