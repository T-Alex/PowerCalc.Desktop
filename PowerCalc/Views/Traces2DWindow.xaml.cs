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
            Model.Traces.CollectionChanged += Traces_CollectionChanged;
        }

        #endregion

        #region Event Handlers

        private void TracesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                try
                {
                    if (Model.Traces.Contains(e.RemovedItems[0]))
                    {
                        Model.UpdateTrace((Trace2D)e.RemovedItems[0]);
                    }
                }
                catch (Exception exc)
                {
                    TracesList.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        TracesList.SelectionChanged -= TracesList_SelectionChanged;
                        TracesList.SelectedItem = e.RemovedItems[0];
                        TracesList.SelectionChanged += TracesList_SelectionChanged;
                        MessageBox.Show(this, exc.Message, Properties.Resources.MessageBoxCaptionText, MessageBoxButton.OK, MessageBoxImage.Error);
                    }));
                }
            }
        }

        private void Traces_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                TracesList.SelectedItem = (Trace2D)e.NewItems[0];
                expressionTextBox.Focus();
            }
        }

        #endregion
    }
}
