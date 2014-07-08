using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TAlex.PowerCalc.Services;
using TAlex.PowerCalc.ViewModels;
using TAlex.PowerCalc.Views;
using TAlex.WPF.Mvvm.Extensions;


namespace TAlex.PowerCalc.Commands
{
    public class InsertMatrixCommand : ICommand
    {
        #region Fields

        protected readonly IClipboardService ClipboardService;
        protected readonly IAppSettings AppSettings;

        #endregion

        #region Constructors

        public InsertMatrixCommand()
        {
            ClipboardService = new ClipboardService();
            AppSettings = PowerCalc.Properties.Settings.Default;
        }

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
            // Get selection text
            string tempText = ClipboardService.GetText();
            ClipboardService.SetText(String.Empty);
            ApplicationCommands.Copy.Execute(null, null);
            string selectedText = ClipboardService.GetText();
            ClipboardService.SetText(tempText);

            List<List<string>> matrix = null;
            if (!String.IsNullOrEmpty(selectedText))
            {
                try
                {
                    matrix = ParseMatrix(selectedText);
                }
                catch (FormatException)
                {
                    MessageBox.Show(Application.Current.GetActiveWindow(),
                        "The format of matrix is invalid.", Properties.Resources.MessageBoxCaptionText, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            var dataContext = new InsertMatrixViewModel();
            if (matrix == null)
                dataContext.Matrix = new MatrixViewModel(AppSettings.WorksheetDefaultInsertMatrixRows, AppSettings.WorksheetDefaultInsertMatrixColumns);
            else
                dataContext.Matrix = new MatrixViewModel(matrix);

            var dialog = new InsertMatrixWindow
            {
                Owner = Application.Current.GetActiveWindow(),
                DataContext = dataContext
            };

            if (dialog.ShowDialog() == true)
            {
                ClipboardService.SetText(dataContext.Matrix.ToString());
                ApplicationCommands.Paste.Execute(null, null);
            }
        }

        #endregion

        #region Helpers

        private List<List<string>> ParseMatrix(string text)
        {
            string s = text.Trim();

            var result = new List<List<string>>();
            result.Add(new List<string>());

            CharEnumerator enumerator = s.GetEnumerator();
            enumerator.MoveNext();
            
            if (enumerator.Current != '{')
            {
                return result;
            }

            int index = 0;
            int parenthesesCount = 0, curlyBracesCount = 0;
            string currentItem = String.Empty;

            while (enumerator.MoveNext())
            {
                index++;
                if (Char.IsWhiteSpace(enumerator.Current)) continue;

                if (enumerator.Current == '{') curlyBracesCount++;
                else if (enumerator.Current == '(') parenthesesCount++;
                else if (enumerator.Current == '}') curlyBracesCount--;
                else if (enumerator.Current == ')') parenthesesCount--;

                if (parenthesesCount == 0 && curlyBracesCount == 0 && (enumerator.Current == ',' || enumerator.Current == ';'))
                {
                    result.Last().Add(currentItem);
                    currentItem = String.Empty;

                    if (enumerator.Current == ';')
                        result.Add(new List<string>());
                }
                else if (parenthesesCount >= 0 && curlyBracesCount >= 0)
                {
                    currentItem += enumerator.Current;
                }
                else if (index == s.Length - 1 && enumerator.Current == '}')
                {
                    result.Last().Add(currentItem);
                }
                else
                {
                    throw new FormatException();
                }
            }
            
            return result;
        }

        #endregion
    }
}
