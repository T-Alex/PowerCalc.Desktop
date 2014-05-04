using System;
using System.Collections.Generic;
using System.Globalization;
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

        public int DecimalPlaces { get; set; }

        public string NumericFormat { get; set; }

        public int ZeroThreshold { get; set; }

        public int ComplexThreshold { get; set; }

        public int WorksheetMaxMatrixRows { get; set; }

        public int WorksheetMaxMatrixColumns { get; set; }


        public int MatricesWorksheetRows { get; set; }

        public int MatricesWorksheetColumns { get; set; }


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
            DecimalPlaces = int.Parse(AppSettings.NumericFormat.Substring(1));
            NumericFormat = AppSettings.NumericFormat[0].ToString();

            ZeroThreshold = AppSettings.ZeroThreshold;
            ComplexThreshold = AppSettings.ComplexThreshold;

            WorksheetMaxMatrixRows = AppSettings.WorksheetMaxMatrixRows;
            WorksheetMaxMatrixColumns = AppSettings.WorksheetMaxMatrixColumns;

            MatricesWorksheetRows = AppSettings.MatricesWorksheetRows;
            MatricesWorksheetColumns = AppSettings.MatricesWorksheetColumns;

            Plot2DBackground = AppSettings.Plot2DBackground;
        }

        private void SaveCommandExecute()
        {
            AppSettings.NumericFormat = NumericFormat + DecimalPlaces.ToString(CultureInfo.InvariantCulture);
            AppSettings.ZeroThreshold = ZeroThreshold;
            AppSettings.ComplexThreshold = ComplexThreshold;

            AppSettings.WorksheetMaxMatrixRows = WorksheetMaxMatrixRows;
            AppSettings.WorksheetMaxMatrixColumns = WorksheetMaxMatrixColumns;

            AppSettings.MatricesWorksheetRows = MatricesWorksheetRows;
            AppSettings.MatricesWorksheetColumns = MatricesWorksheetColumns;

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
