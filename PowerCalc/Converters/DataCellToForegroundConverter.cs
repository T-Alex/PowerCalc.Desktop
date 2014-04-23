using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using TAlex.WPF.Converters;


namespace TAlex.PowerCalc.Converters
{
    public class DataCellToForegroundConverter : IMultiValueConverter
    {
        #region IMultiValueConverter Members

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            object cachedValue = values[0];
            object parent = values[1];

            Color color = Colors.Black;

            if (cachedValue is Exception)
            {
                color = Colors.Red;
            }
            else if (parent != null)
            {
                color = Colors.Green;
            }
            return new SolidColorBrush(color);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
