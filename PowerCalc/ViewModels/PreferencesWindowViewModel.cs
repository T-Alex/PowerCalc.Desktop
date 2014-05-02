using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using TAlex.PowerCalc.Services;
using TAlex.WPF.Mvvm;
using TAlex.WPF.Mvvm.Commands;


namespace TAlex.PowerCalc.ViewModels
{
    public class PreferencesWindowViewModel : ViewModelBase
    {
        #region Fields

        protected readonly IAppSettings AppSettings;

        private bool _closeSignal;

        #endregion

        #region Properties

        public int ZeroThreshold { get; set; }

        public int ComplexThreshold { get; set; }

        public int WorksheetMaxMatrixRows { get; set; }

        public int WorksheetMaxMatrixCols { get; set; }


        public Color Plot2DBackground { get; set; }


        public bool CloseSignal
        {
            get
            {
                return _closeSignal;
            }

            set
            {
                Set(() => CloseSignal, ref _closeSignal, value);
            }
        }

        #endregion

        #region Commands

        public ICommand SaveCommand { get; set; }

        public ICommand CancelCommand { get; set; }

        #endregion

        #region Constructors

        public PreferencesWindowViewModel(IAppSettings appSettings)
        {
            AppSettings = appSettings;

            InitCommands();
            LoadSettings();
        }

        #endregion

        #region Methods

        private void InitCommands()
        {
            SaveCommand = new RelayCommand(SaveCommandExecute);
            CancelCommand = new RelayCommand(CancelCommandExecute);
        }


        private void LoadSettings()
        {
            ZeroThreshold = AppSettings.ZeroThreshold;
            ComplexThreshold = AppSettings.ComplexThreshold;

            WorksheetMaxMatrixRows = AppSettings.WorksheetMaxMatrixRows;
            WorksheetMaxMatrixCols = AppSettings.WorksheetMaxMatrixCols;

            Plot2DBackground = AppSettings.Plot2DBackground;
        }

        private void SaveCommandExecute()
        {
            AppSettings.ZeroThreshold = ZeroThreshold;
            AppSettings.ComplexThreshold = ComplexThreshold;

            AppSettings.WorksheetMaxMatrixRows = WorksheetMaxMatrixRows;
            AppSettings.WorksheetMaxMatrixCols = WorksheetMaxMatrixCols;

            AppSettings.Plot2DBackground = Plot2DBackground;

            AppSettings.Save();
            CloseSignal = true;
        }

        public void CancelCommandExecute()
        {
            CloseSignal = true;
        }

        #endregion
    }
}
