using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.WPF.Converters;
using TAlex.WPF.Theming;


namespace TAlex.PowerCalc.Converters
{
    public class IsCurrentThemeConverter : ConverterBase<IsCurrentThemeConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return String.Equals(ThemeLocator.Manager.CurrentTheme, (string)value);
        }
    }
}
