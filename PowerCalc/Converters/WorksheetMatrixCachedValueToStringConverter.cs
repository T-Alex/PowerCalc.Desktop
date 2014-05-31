using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.MathCore;
using TAlex.PowerCalc.Services;
using TAlex.PowerCalc.ViewModels.Matrices;
using TAlex.WPF.Converters;


namespace TAlex.PowerCalc.Converters
{
    public class WorksheetMatrixCachedValueToStringConverter : ConverterBase<WorksheetMatrixCachedValueToStringConverter>
    {
        private WorksheetMatrixCachedValueConverter _converter;


        public WorksheetMatrixCachedValueToStringConverter()
        {
            _converter = new WorksheetMatrixCachedValueConverter(Properties.Settings.Default);
        }


        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _converter.ToString(value);
        }
    }

    public class WorksheetMatrixCachedValueConverter
    {
        protected readonly IAppSettings AppSettings;


        public WorksheetMatrixCachedValueConverter(IAppSettings settings)
        {
            AppSettings = settings;
        }


        public virtual string ToString(object value)
        {
            if (value is Complex)
            {
                Complex normilizedValue = NumericUtil.ComplexZeroThreshold((Complex)value, AppSettings.ComplexThreshold, AppSettings.ZeroThreshold);
                return normilizedValue.ToString(AppSettings.NumericFormat, CultureInfo.InvariantCulture);
            }
            else if (value is MatrixIndexOutOfRangeException)
            {
                return "#N/A";
            }
            else if (value is Exception)
            {
                return "#ERROR";
            }

            return null;
        }
    }
}
