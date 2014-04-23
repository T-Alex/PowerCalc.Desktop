using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.MathCore;
using TAlex.MathCore.LinearAlgebra;


namespace TAlex.PowerCalc.ViewModels.Matrices
{
    public class DataCell : DataUnit
    {
        #region Fields

        private string _expression;
        private Object _cachedValue;
        private DataArray _parent;

        #endregion

        #region Properties

        public DataArray Parent
        {
            get
            {
                return _parent;
            }

            set
            {
                Set(() => Parent, ref _parent, value);
            }
        }

        public override Object CachedValue
        {
            get
            {
                if (Parent == null)
                {
                    if (_cachedValue == null)
                    {
                        try
                        {
                            _cachedValue = !String.IsNullOrEmpty(Expression) ? EvaluateExpression() : new Object();
                        }
                        catch(Exception exc)
                        {
                            _cachedValue = exc;
                        }
                        OnCachedValueChanged();
                    }

                    if (_cachedValue is CMatrix)
                    {
                        return ((CMatrix)_cachedValue)[0, 0];
                    }
                    return _cachedValue;
                }

                return Parent.FindValue(this);
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
            Parent = null;
            Expression = null;
            UnsubscripeReferences();
        }

        public void RefreshValue()
        {
            RaisePropertyChanged(() => CachedValue);
        }

        public void RaiseCachedValueChanged()
        {
            OnCachedValueChanged();
        }

        protected override void CachedValueChangedHandler(object sender, EventArgs e)
        {
            if (Parent == null)
            {
                _cachedValue = null;
                RefreshValue();
            }
        }

        #endregion
    }
}
