using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.MathCore;
using TAlex.WPF.Converters;


namespace TAlex.PowerCalc.Converters
{
    public class WorksheetMatrixCachedValueToStringConverter : ConverterBase<WorksheetMatrixCachedValueToStringConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Complex)
            {
                Properties.Settings settings = Properties.Settings.Default;

                Complex normilizedValue = NumericUtil.ComplexZeroThreshold((Complex)value, settings.ComplexThreshold, settings.ZeroThreshold);
                return normilizedValue.ToString(settings.NumericFormat, CultureInfo.InvariantCulture);
            }

            return null;
        }
    }
}
