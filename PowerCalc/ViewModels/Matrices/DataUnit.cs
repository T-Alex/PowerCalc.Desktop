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
using TAlex.MathCore.ExpressionEvaluation;


namespace TAlex.PowerCalc.ViewModels.Matrices
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

        public IList<DataUnit> References { get; private set; }

        public abstract string Address { get; }

        #endregion

        #region Events

        public event EventHandler CachedValueChanged;

        #endregion

        #region Constructors

        public DataUnit()
        {
            References = new List<DataUnit>();
        }

        #endregion

        #region Methods

        protected Object EvaluateExpression()
        {
            UnsubscripeReferences();
            string expression = Expression;

            // TODO: AddReference for dependent cells.
            DataTable.CheckCircularReferences(Address, expression);

            // Preparation variables
            IDictionary<string, Object> vars = new Dictionary<string, object>();

            List<string> cellsRangesReferences = A1ReferenceHelper.A1ReferenceRangeOfCellsRegex.Matches(expression)
                .Cast<Match>().Select(x => x.Value).Distinct().OrderByDescending(x => x.Length).ToList();

            foreach (string reference in cellsRangesReferences)
            {
                string varName = GetRandomVariableName();
                vars.Add(varName, GetRangeOfCellValues(reference));
                expression = expression.Replace(reference, varName);
            }

            List<string> singleCellReferences = A1ReferenceHelper.A1ReferenceSingleCellRegex.Matches(expression)
                .Cast<Match>().Select(x => x.Value).Distinct().OrderByDescending(x => x.Length).ToList();

            foreach (string reference in singleCellReferences)
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

        protected void UnsubscripeReferences()
        {
            foreach (DataUnit unit in References)
            {
                unit.CachedValueChanged -= CachedValueChangedHandler;
            }
            References.Clear();
        }

        private void AddReference(DataUnit unit)
        {
            unit.CachedValueChanged += CachedValueChangedHandler;
            References.Add(unit);
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

        private object GetSingleCellValue(string a1Reference)
        {
            int row, column;
            A1ReferenceHelper.Parse(a1Reference, out column, out row);

            DataCell cell = DataTable[row, column];
            AddReference(cell);

            object varValue = cell.CachedValue;
            if (varValue is Exception || varValue.GetType() == typeof(object))
            {
                throw new UnassignedVariableException(a1Reference);
            }
            return varValue;
        }

        protected abstract void CachedValueChangedHandler(object sender, EventArgs e);

        private CMatrix GetRangeOfCellValues(string a1Reference)
        {
            int row1Idx, col1Idx, row2Idx, col2Idx;
            A1ReferenceHelper.Parse(a1Reference, out col1Idx, out row1Idx, out col2Idx, out row2Idx);

            int n = row2Idx - row1Idx + 1;
            int m = col2Idx - col1Idx + 1;

            CMatrix matrix = new CMatrix(n, m);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    DataCell cell = DataTable[i + row1Idx, j + col1Idx];
                    AddReference(cell);
                    object cellValue = cell.CachedValue;
                    if (cellValue is Exception || cellValue.GetType() == typeof(object))
                    {
                        throw new UnassignedVariableException(a1Reference);
                    }

                    matrix[i, j] = (Complex)cellValue;
                }
            }

            return matrix;
        }

        #endregion
    }
}
