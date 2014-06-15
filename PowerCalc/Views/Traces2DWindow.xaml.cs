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
using TAlex.PowerCalc.ViewModels.Plot2D;


namespace TAlex.PowerCalc.Views
{
    /// <summary>
    /// Interaction logic for Traces2DWindow.xaml
    /// </summary>
    public partial class Traces2DWindow : Window
    {
        #region Properties

        public Traces2DModel Model
        {
            get { return (Traces2DModel)DataContext; }
        }
        
        #endregion

        #region Constructors

        public Traces2DWindow()
        {
            InitializeComponent();
        }

        public Traces2DWindow(Traces2DModel.StateMode mode, Trace2DCollection traces)
            : this()
        {
            Model.SetState(mode, traces);
        }

        #endregion
    }
}
