using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;


namespace TAlex.PowerCalc.Commands
{
    public class InsertFunctionCommand : ICommand
    {
        private const string pattern = @"\A(?<name>[a-zA-Z_][a-zA-Z_0-9]*)\((?<separates>[,]*)\)\Z";
        private static readonly Regex regex = new Regex(pattern, RegexOptions.Compiled);


        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            //return (parameter is String && !String.IsNullOrEmpty((string)parameter)) || parameter is InsertFunctionParameter;
            return true;
        }

        public virtual event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, new EventArgs());
            }
        }

        public void Execute(object parameter)
        {
            IInputElement target = null;
            string functionHeader = null;

            if (parameter is String)
            {
                functionHeader = parameter as String;
            }
            else if (parameter is InsertFunctionParameter)
            {
                InsertFunctionParameter insertParameter = parameter as InsertFunctionParameter;
                functionHeader = insertParameter.FunctionHeader;
                target = insertParameter.Target;
            }

            InsertFunction(functionHeader, target);
        }

        private void InsertFunction(string functionHeader, IInputElement target)
        {
            string tempText = Clipboard.GetText();
            Clipboard.SetText(String.Empty);
            ApplicationCommands.Copy.Execute(null, target);
            string selectedText = Clipboard.GetText();

            Match match = regex.Match(functionHeader);

            if (match.Success)
            {
                if (!String.IsNullOrEmpty(selectedText))
                {
                    functionHeader = functionHeader.Replace("(", String.Format("({0}", selectedText));

                    Clipboard.SetText(functionHeader);
                    ApplicationCommands.Paste.Execute(null, target);

                    for (int i = 0; i < functionHeader.Length; i++)
                        EditingCommands.SelectLeftByCharacter.Execute(null, target);
                }
                else
                {
                    Clipboard.SetText(functionHeader);
                    ApplicationCommands.Paste.Execute(null, target);

                    int args = match.Groups["separates"].Value.Length + 1;

                    for (int i = 0; i < args; i++)
                        EditingCommands.MoveLeftByCharacter.Execute(null, target);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(selectedText))
                    EditingCommands.MoveLeftByCharacter.Execute(null, target);

                Clipboard.SetText(functionHeader);
                ApplicationCommands.Paste.Execute(null, target);
            }

            Clipboard.SetText(tempText);
            if (target != null) target.Focus();
        }

        #endregion
    }

    public class InsertFunctionParameter : DependencyObject
    {
        public string FunctionHeader { get; set; }

        public static readonly DependencyProperty TargetProperty;

        public IInputElement Target
        {
            get
            {
                return (IInputElement)GetValue(TargetProperty);
            }

            set
            {
                SetValue(TargetProperty, value);
            }
        }


        static InsertFunctionParameter()
        {
            TargetProperty = DependencyProperty.Register("Target", typeof(IInputElement), typeof(InsertFunctionParameter));
        }
    }
}
