using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using TAlex.WPF.Mvvm;
using TAlex.WPF.Mvvm.Commands;

namespace TAlex.PowerCalc.ViewModels
{
    public class DefineVariableViewModel : ViewModelBase
    {
        #region Fields

        private string _variableName;
        private static readonly Regex _variableNameRegex = new Regex(@"^[^\W\d]\w*$", RegexOptions.Compiled);

        private bool _closeSignal;

        #endregion

        #region Properties

        public string VariableName
        {
            get
            {
                return _variableName;
            }

            set
            {
                Set(() => VariableName, ref _variableName, value);
            }
        }

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

        public ICommand DefineCommand { get; set; }

        #endregion

        #region Constructors

        public DefineVariableViewModel()
        {
            InitializeCommands();
        }

        #endregion

        #region Methods

        protected virtual void InitializeCommands()
        {
            DefineCommand = new RelayCommand(Define, CanDefine);
        }

        private void Define()
        {
            CloseSignal = true;
        }

        private bool CanDefine()
        {
            return _variableNameRegex.IsMatch(VariableName.Trim());
        }

        #endregion
    }
}
