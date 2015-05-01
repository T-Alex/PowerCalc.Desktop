using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TAlex.MathCore;
using TAlex.MathCore.ExpressionEvaluation.Trees;
using TAlex.MathCore.LinearAlgebra;
using TAlex.PowerCalc.Helpers;
using TAlex.MathCore.ExpressionEvaluation;
using TAlex.Mvvm.ViewModels;


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
            string expression = Expression;
            UnsubscribeReferences();

            try
            {
                DataTable.CheckCircularReferences(Address, this);
            }
            catch (CircularReferenceException exc)
            {
                AddReference(exc.DataUnit);
                throw;
            }
            
            IDictionary<string, Object> vars = FindAllVariables(ref expression);

            // Evaluation the expression
            Expression<Object> expr = DataTable.BuildTree(expression);
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

        protected void UnsubscribeReferences()
        {
            foreach (DataUnit unit in References)
            {
                unit.CachedValueChanged -= CachedValueChangedHandler;
            }
            References.Clear();
        }

        protected abstract void CachedValueChangedHandler(object sender, EventArgs e);

        #region Helpers

        private IDictionary<string, Object> FindAllVariables(ref string expression)
        {
            IDictionary<string, Object> vars = new Dictionary<string, object>();
            List<string> cellRangeReferences = A1ReferenceHelper.GetUniqueCellRangeReferences(expression);

            foreach (string reference in cellRangeReferences)
            {
                string varName = GetRandomVariableName();
                vars.Add(varName, GetCellRangeValue(reference));
                expression = expression.Replace(reference, varName);
            }

            List<string> singleCellReferences = A1ReferenceHelper.GetUniqueSingleCellReferences(expression);

            foreach (string reference in singleCellReferences)
            {
                string varName = GetRandomVariableName();
                vars.Add(varName, GetSingleCellValue(reference));
                expression = expression.Replace(reference, varName);
            }

            return vars;
        }

        private void AddReference(DataUnit unit)
        {
            if (this != unit)
            {
                unit.CachedValueChanged += CachedValueChangedHandler;
                References.Add(unit);
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

        private object GetSingleCellValue(string a1Reference)
        {
            int row, column;
            A1ReferenceHelper.Parse(a1Reference, out column, out row);

            DataCell cell = DataTable[row, column];
            AddReference(cell);
            object varValue = cell.CachedValue;
            EnsureValidReferenceValue(varValue, a1Reference);

            return varValue;
        }

        private CMatrix GetCellRangeValue(string a1Reference)
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
                    EnsureValidReferenceValue(cellValue, a1Reference);

                    matrix[i, j] = (Complex)cellValue;
                }
            }

            return matrix;
        }

        private void EnsureValidReferenceValue(object cellValue, string reference)
        {
            if (cellValue is Exception || cellValue.GetType() == typeof(object))
            {
                throw new UnassignedVariableException(reference);
            }
        }

        #endregion

        #endregion
    }
}
