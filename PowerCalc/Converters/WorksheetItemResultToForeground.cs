using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using TAlex.WPF.Converters;

namespace TAlex.PowerCalc.Converters
{
    public class WorksheetItemResultToForeground : ConverterBase<WorksheetItemResultToForeground>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Color color = Colors.DarkGreen;

            if (value is Exception)
                color = Colors.DarkRed;
            else if (value is KeyValuePair<string, Object>)
                color = Colors.DarkBlue;

            return new SolidColorBrush(color);
        }
    }
}
