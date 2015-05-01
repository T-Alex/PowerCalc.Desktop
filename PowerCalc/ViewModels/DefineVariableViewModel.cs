using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;
using TAlex.Mvvm.Commands;
using TAlex.Mvvm.ViewModels;
using TAlex.PowerCalc.Properties;


namespace TAlex.PowerCalc.ViewModels
{
    public class DefineVariableViewModel : ViewModelBase, IDataErrorInfo
    {
        #region Fields

        private string _variableName;
        private static readonly Regex VariableNameRegex = new Regex(@"^[^\W\d]\w*$", RegexOptions.Compiled);

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
                Set(ref _variableName, value);
                RaiseCanDefine();
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
                Set(ref _closeSignal, value);
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
            return VariableName != null && VariableNameRegex.IsMatch(VariableName.Trim());
        }

        private void RaiseCanDefine()
        {
            var command = DefineCommand as RelayCommand;
            if (command != null)
            {
                command.RaiseCanExecuteChanged();
            }
        }

        #endregion

        #region IDataErrorInfo Members

        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case "VariableName":
                        if (VariableName != null)
                        {
                            if (String.IsNullOrWhiteSpace(VariableName)) return Resources.VALID_VarCannotBeEmpty;
                            if (!VariableNameRegex.IsMatch(VariableName.Trim())) return Resources.VALID_VarInvalidFormat;
                        }
                        break;
                }

                return null;
            }
        }

        #endregion
    }
}
