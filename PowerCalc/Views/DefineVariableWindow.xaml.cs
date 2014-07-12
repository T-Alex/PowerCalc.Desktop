using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace TAlex.PowerCalc.Views
{
    /// <summary>
    /// Interaction logic for DefineVariableWindow.xaml
    /// </summary>
    public partial class DefineVariableWindow : Window
    {
        #region Constructors

        public DefineVariableWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            VariableNameTextBox.CaretIndex = VariableNameTextBox.Text.Length;
        }

        #endregion
    }
}
