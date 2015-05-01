using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Media.Animation;


namespace TAlex.PowerCalc.Views
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        #region Constructors

        protected AboutWindow()
        {
            InitializeComponent();
        }

        public AboutWindow(Window parent) : this()
        {
            this.Owner = parent;
        }

        #endregion

        #region Methods

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Close();
        }

        #endregion
    }
}
