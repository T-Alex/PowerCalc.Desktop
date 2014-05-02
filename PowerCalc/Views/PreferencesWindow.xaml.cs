using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Globalization;

using TAlex.WPF.Controls;


namespace TAlex.PowerCalc
{
    /// <summary>
    /// Interaction logic for PreferencesWindow.xaml
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        #region Constructors

        public PreferencesWindow()
        {
            InitializeComponent();
            LoadSettings();
        }

        public PreferencesWindow(int tabIndex) : this()
        {
            mainTabControl.SelectedIndex = tabIndex;
        }

        #endregion

        #region Methods

        private void LoadSettings()
        {
            Properties.Settings settings = Properties.Settings.Default;

            // Result format
            char format = settings.NumericFormat[0];

            if (settings.NumericFormat.Length > 1)
            {
                int precision = int.Parse(settings.NumericFormat.Substring(1));
                decimalPlacesNumericUpDown.Value = precision;
            }

            switch (Char.ToUpper(format))
            {
                case 'E':
                    scientificFormatRadioButton.IsChecked = true;
                    break;

                case 'F':
                    fixedPointFormatRadioButton.IsChecked = true;
                    break;

                case 'G':
                    generalFormatRadioButton.IsChecked = true;
                    break;

                default:
                    throw new FormatException();
            }
        }

        private void SaveSettings()
        {
            Properties.Settings settings = Properties.Settings.Default;

            // Result format
            string numFormat = String.Empty;

            if (scientificFormatRadioButton.IsChecked == true)
                numFormat = "E";
            else if (fixedPointFormatRadioButton.IsChecked == true)
                numFormat = "F";
            else if (generalFormatRadioButton.IsChecked == true)
                numFormat = "G";
            else
                throw new FormatException();

            int decimalPlaces = (int)decimalPlacesNumericUpDown.Value;
            numFormat += decimalPlaces.ToString(CultureInfo.InvariantCulture);

            settings.NumericFormat = numFormat;

            settings.Save();
        }

        #endregion
    }
}
