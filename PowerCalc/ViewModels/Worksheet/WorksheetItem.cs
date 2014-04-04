using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAlex.WPF.Mvvm;


namespace TAlex.PowerCalc.ViewModels.Worksheet
{
    public class WorksheetItem : ViewModelBase
    {
        #region Fields

        public string _expression;
        public Object _result;

        #endregion

        #region Properties

        public string Expression
        {
            get
            {
                return _expression;
            }

            set
            {
                Set(() => Expression, ref _expression, value);
            }
        }

        public object Result
        {
            get
            {
                return _result;
            }

            set
            {
                Set(() => Result, ref _result, value);
            }
        }

        #endregion

        #region Constructors

        public WorksheetItem()
        {
            Expression = String.Empty;
        }

        public WorksheetItem(string expression, Object result)
        {
            Expression = expression;
            Result = result;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return Expression;
        }

        #endregion
    }
}
