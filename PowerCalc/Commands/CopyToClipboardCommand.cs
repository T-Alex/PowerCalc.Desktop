using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TAlex.MathCore;
using TAlex.MathCore.LinearAlgebra;


namespace TAlex.PowerCalc.Commands
{
    public class CopyToClipboardCommand : ICommand
    {
        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        public void Execute(object parameter)
        {
            CopyToClipboardParameter copyParams = GetParams(parameter);
            object value = copyParams.Value;
            string text = null;

            if (copyParams.IsFormatted)
            {
                object result = null;
                Properties.Settings settings = Properties.Settings.Default;

                if (value is Complex) result = NumericUtil.ComplexZeroThreshold((Complex)value, settings.ComplexThreshold, settings.ZeroThreshold);
                else if (value is CMatrix) result = NumericUtilExtensions.ComplexZeroThreshold((CMatrix)value, settings.ComplexThreshold, settings.ZeroThreshold);
                else result = value;

                text = (result is String) ? (String)result : ((IFormattable)result).ToString(settings.NumericFormat, CultureInfo.InvariantCulture);
            }
            else
            {
                text = (value is IFormattable) ? ((IFormattable)value).ToString(null, CultureInfo.InvariantCulture) : value.ToString();
            }
            
            Clipboard.SetText(text);
        }

        private CopyToClipboardParameter GetParams(object obj)
        {
            CopyToClipboardParameter param = (obj is CopyToClipboardParameter) ?
                (CopyToClipboardParameter)obj :
                new CopyToClipboardParameter(obj, false);

            param.Value = (param.Value is KeyValuePair<string, Object>) ? ((KeyValuePair<string, Object>)param.Value).Value : param.Value;
            if (param.Value is Exception) param.Value = ((Exception)param.Value).Message;

            return param;
        }

        #endregion
    }

    public class CopyToClipboardParameter
    {
        public bool IsFormatted { get; set; }

        public object Value { get; set; }


        public CopyToClipboardParameter()
        {
        }

        public CopyToClipboardParameter(object value, bool isFormatted)
        {
            Value = value;
            IsFormatted = isFormatted;
        }
    }
}
