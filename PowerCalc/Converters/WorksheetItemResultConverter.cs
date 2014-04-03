using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.MathCore;
using TAlex.MathCore.LinearAlgebra;
using TAlex.PowerCalc.Properties;
using TAlex.WPF.Converters;


namespace TAlex.PowerCalc.Converters
{
    public class WorksheetItemResultConverter : ConverterBase<WorksheetItemResultConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Exception)
            {
                return ((Exception)value).Message;
            }
            else if (value is KeyValuePair<string, Object>)
            {
                var pair = (KeyValuePair<string, Object>)value;
                return String.Format(Resources.VariableValueAssigned, pair.Key) + " " + FormatValue(pair.Value as IFormattable);
            }
            else if (value is IFormattable)
            {
                return FormatValue(value as IFormattable);
            }

            return value;
        }

        private object FormatValue(IFormattable value)
        {
            Properties.Settings settings = Properties.Settings.Default;

            object result = null;

            if (value is Complex) result = NumericUtil.ComplexZeroThreshold((Complex)value, settings.ComplexThreshold, settings.ZeroThreshold);
            if (value is CMatrix) result = NumericUtilExtensions.ComplexZeroThreshold((CMatrix)result, settings.ComplexThreshold, settings.ZeroThreshold);

            return ((IFormattable)result).ToString(settings.NumericFormat, CultureInfo.InvariantCulture); ;
        }
    }
}
