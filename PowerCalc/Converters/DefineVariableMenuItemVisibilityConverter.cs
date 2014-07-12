using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TAlex.WPF.Converters;
using TAlex.WPF.Mvvm.Extensions;


namespace TAlex.PowerCalc.Converters
{
    public class DefineVariableMenuItemVisibilityConverter : ConverterBase<DefineVariableMenuItemVisibilityConverter>
    {
        private static readonly string WorksheetTextBoxName = "worksheetTextBox";


        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var activeWindow = App.Current.GetActiveWindow();
            var textBox = FocusManager.GetFocusedElement(activeWindow) as TextBox;
            if (textBox == null) return Visibility.Visible;

            return (textBox.Name == WorksheetTextBoxName) ? Visibility.Visible : Visibility.Collapsed;
        }
    }
}
