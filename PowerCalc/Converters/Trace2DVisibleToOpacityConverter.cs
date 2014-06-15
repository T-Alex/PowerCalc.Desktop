using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.WPF.Converters;


namespace TAlex.PowerCalc.Converters
{
    public class Trace2DVisibleToOpacityConverter: ConverterBase<Trace2DVisibleToOpacityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)value) ? 1 : 0.4;
        }
    }
}
