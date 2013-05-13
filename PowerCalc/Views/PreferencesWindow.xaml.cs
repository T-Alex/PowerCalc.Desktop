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
using TAlex.WPF.CommonDialogs;

namespace TAlex.PowerCalc
{
    /// <summary>
    /// Interaction logic for PreferencesWindow.xaml
    /// </summary>
    public partial class PreferencesWindow : Window
    {
        #region Fields

        private FontFamily _worksheetFontFamily;
        private FontWeight _worksheetFontWeight;
        private FontStyle _worksheetFontStyle;
        private FontStretch _worksheetFontStretch;
        private double _worksheetFontSize;
        private Color _worksheetForeground;

        #endregion

        #region Properties

        #endregion

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
            zeroThresholdNumericUpDown.Value = settings.ZeroThreshold;
            complexThresholdNumericUpDown.Value = settings.ComplexThreshold;

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

            // Worksheet
            _worksheetFontFamily = settings.WorksheetFontFamily;
            _worksheetFontWeight = settings.WorksheetFontWeight;
            _worksheetFontStyle = settings.WorksheetFontStyle;
            _worksheetFontStretch = settings.WorksheetFontStretch;
            _worksheetFontSize = settings.WorksheetFontSize;
            _worksheetForeground = settings.WorksheetForeground;

            // 2D Plot
            plot2DBackgroundBrush.SelectedColor = settings.Plot2DBackground;
        }

        private void SaveSettings()
        {
            Properties.Settings settings = Properties.Settings.Default;

            // Result format
            settings.ZeroThreshold = (int)zeroThresholdNumericUpDown.Value;
            settings.ComplexThreshold = (int)complexThresholdNumericUpDown.Value;

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

            // Worksheet
            settings.WorksheetFontFamily = _worksheetFontFamily;
            settings.WorksheetFontWeight = _worksheetFontWeight;
            settings.WorksheetFontStyle = _worksheetFontStyle;
            settings.WorksheetFontStretch = _worksheetFontStretch;
            settings.WorksheetFontSize = _worksheetFontSize;
            settings.WorksheetForeground = _worksheetForeground;

            // 2D Plot
            settings.Plot2DBackground = plot2DBackgroundBrush.SelectedColor;

            settings.Save();
        }

        private void ResetSettings()
        {
        }

        #region Event Handlers

        private void fontButton_Click(object sender, RoutedEventArgs e)
        {
            FontChooserDialog fcd = new FontChooserDialog();
            fcd.Background = Brushes.WhiteSmoke;
            fcd.Owner = this;

            fcd.Width = 500;
            fcd.Height = 400;
            fcd.ShowTextDecorations = false;
            fcd.ShowColor = true;

            fcd.SelectedFontFamily = _worksheetFontFamily;
            fcd.SelectedFontWeight = _worksheetFontWeight;
            fcd.SelectedFontStyle = _worksheetFontStyle;
            fcd.SelectedFontStretch = _worksheetFontStretch;
            fcd.SelectedFontSize = _worksheetFontSize;
            fcd.SelectedFontColor = _worksheetForeground;

            if (fcd.ShowDialog() == true)
            {
                _worksheetFontFamily = fcd.SelectedFontFamily;
                _worksheetFontWeight = fcd.SelectedFontWeight;
                _worksheetFontStyle = fcd.SelectedFontStyle;
                _worksheetFontStretch = fcd.SelectedFontStretch;
                _worksheetFontSize = fcd.SelectedFontSize;
                _worksheetForeground = fcd.SelectedFontColor;
            }
        }


        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveSettings();
            }
            catch (Exception)
            {
                MessageBox.Show(this, "Incorrect settings.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion

        #endregion
    }
}
