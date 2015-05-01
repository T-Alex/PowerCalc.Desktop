using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using System.Windows.Threading;
using System.Diagnostics;
using TAlex.Mvvm.Extensions;
using TAlex.PowerCalc.Licensing;
using TAlex.PowerCalc.Locators;
using TAlex.PowerCalc.Views;
using TAlex.Common.Diagnostics.ErrorReporting;
using TAlex.WPF.Theming;
using TAlex.Common.Models;
using System.Reflection;


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

            ErrorReportingWindow reportWindow =
                new ErrorReportingWindow(new ErrorReport(e), new AssemblyInfo(Assembly.GetExecutingAssembly()));

            reportWindow.Owner = this.GetActiveWindow();
            reportWindow.ShowDialog();
        }

        protected override void OnNavigating(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigating(e);
            
            // apply theme
            ThemeLocator.Manager.ApplyTheme(PowerCalc.Properties.Settings.Default.ColorScheme);

            // check license
            CheckTrialExpiration();
        }

        private void CheckTrialExpiration()
        {
            ViewModelLocator locator = Resources["viewModelLocator"] as ViewModelLocator;
            AppLicense license = locator.Get<AppLicense>();

            if (license.IsTrial && license.TrialHasExpired)
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
