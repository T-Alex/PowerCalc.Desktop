using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using TAlex.PowerCalc.ViewModels;
using TAlex.PowerCalc.Views;
using TAlex.WPF.Mvvm.Extensions;


namespace TAlex.PowerCalc.Commands
{
    public class DefineVariableCommand : ICommand
    {
        #region Fields

        private Regex _variableRegex = new Regex(@"^(?<var>[^\W\d]\w*)\s*:\s*(?<expr>.*)$", RegexOptions.Compiled);

        #endregion

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        public void Execute(object parameter)
        {
            var activeWindow = App.Current.GetActiveWindow();
            var textBox = FocusManager.GetFocusedElement(activeWindow) as TextBox;
            if (textBox == null) return;

            string varName = null;
            string expression = textBox.Text.Trim();
            var varMatch = _variableRegex.Match(expression);

            if (varMatch.Success)
                varName = varMatch.Groups["var"].Value;

            var dataContext = new DefineVariableViewModel { VariableName = varName };
            DefineVariableWindow window = new DefineVariableWindow
            {
                Owner = activeWindow,
                DataContext = dataContext
            };

            if (window.ShowDialog() == true)
            {
                if (!varMatch.Success)
                    textBox.Text = expression.Insert(0, String.Format("{0}: ", dataContext.VariableName));
                else
                    textBox.Text = _variableRegex.Replace(expression, String.Format("{0}: ${{expr}}", dataContext.VariableName));
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        #endregion
    }
}
