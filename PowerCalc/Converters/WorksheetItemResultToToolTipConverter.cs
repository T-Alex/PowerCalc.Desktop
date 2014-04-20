using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.WPF.Converters;

namespace TAlex.PowerCalc.Converters
{
    public class WorksheetItemResultToToolTipConverter : ConverterBase<WorksheetItemResultToToolTipConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Exception)
            {
                return ((Exception)value).Message;
            }
            return null;
        }
    }
}
