using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;

using TAlex.PowerCalc.Licensing;

namespace TAlex.PowerCalc
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        #region Constructors

        public App()
        {
            DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(App_DispatcherUnhandledException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
        }

        #endregion

        #region Methods

        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            ProcessingUnhandledException(e.Exception);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ProcessingUnhandledException(e.ExceptionObject as Exception);
        }

        private void ProcessingUnhandledException(Exception e)
        {
            Trace.TraceError(e.ToString());
        }

        protected override void OnNavigating(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigating(e);
            
            TAlex.WPFThemes.Twilight.TwilightThemeManager.ApplyTheme(PowerCalc.Properties.Settings.Default.ColorScheme);

            if (License.IsTrial && License.TrialHasExpired)
            {
                if (MessageBox.Show("Evaluation period has expired, please register.", "Information",
                    MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    RegistrationWindow window = new RegistrationWindow();
                    window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    window.ShowDialog();
                }
                else
                {
                    Shutdown();
                }
            }
        }

        #endregion
    }
}
