using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.MathCore;
using TAlex.MathCore.LinearAlgebra;


namespace TAlex.PowerCalc.ViewModels.WorksheetMatrix
{
    public class DataCell : DataUnit
    {
        #region Fields

        private static readonly string CellErrorText = "#ERROR";

        private string _expression;
        private Object _cachedValue;

        #endregion

        #region Properties

        public DataArray Parent { get; set; }

        public override Object CachedValue
        {
            get
            {
                if (Parent == null)
                {
                    Object value = _cachedValue ?? EvaluateExpression();
                    if (value is CMatrix)
                    {
                        return ((CMatrix)value)[0, 0];
                    }
                    return value;
                }

                return Parent.FindValue(this, Parent.CachedValue);
            }
        }

        public override string Expression
        {
            get
            {
                return (Parent == null) ? _expression : Parent.Expression;
            }

            set
            {
                if (Parent == null)
                {
                    _expression = value;
                    _cachedValue = null;
                }
                else
                {
                    Parent.Expression = value;
                }

                RaisePropertyChanged(() => Expression);
                RefreshValue();
            }
        }

        #endregion

        #region Constructors

        public DataCell(DataTable dataTable)
        {
            DataTable = dataTable;
        }

        #endregion

        #region Methods

        public void Clear()
        {
            Expression = null;
        }

        public void RefreshValue()
        {
            RaisePropertyChanged(() => CachedValue);
        }

        #endregion
    }
}
