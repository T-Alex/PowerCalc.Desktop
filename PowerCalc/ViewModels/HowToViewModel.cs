using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TAlex.PowerCalc.Services;
using TAlex.WPF.Mvvm.Commands;


namespace TAlex.PowerCalc.ViewModels
{
    public class HowToViewModel
    {
        #region Fields

        protected readonly IAppSettings AppSettings;

        #endregion

        #region Properties

        public bool ShowOnStartup
        {
            get
            {
                return AppSettings.ShowHowToOnStartup;
            }

            set
            {
                AppSettings.ShowHowToOnStartup = value;
                AppSettings.Save();
            }
        }

        #endregion

        #region Commands

        public ICommand PreviousCommand { get; set; }

        public ICommand NextCommand { get; set; }

        #endregion

        #region Constructors

        public HowToViewModel(IAppSettings appSettings)
        {
            InitializeCommands();

            AppSettings = appSettings;
            ShowOnStartup = appSettings.ShowHowToOnStartup;
        }

        #endregion

        #region Methods

        protected virtual void InitializeCommands()
        {
            PreviousCommand = new RelayCommand(Previous);
            NextCommand = new RelayCommand(Next);
        }


        private void Previous()
        {

        }

        private void Next()
        {

        }

        #endregion
    }
}
