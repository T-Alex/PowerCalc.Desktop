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
    /// Interaction logic for InsertMatrixWindow.xaml
    /// </summary>
    public partial class InsertMatrixWindow : Window
    {
        #region Properties

        public string[,] Matrix { get; set; }

        #endregion

        #region Constructors

        public InsertMatrixWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Event Handlers

        private void closeButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion
    }
}
