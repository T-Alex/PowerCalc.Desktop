using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TAlex.PowerCalc.Properties;
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
            if (!String.IsNullOrWhiteSpace(selectedText))
            {
                try
                {
                    matrix = ParseMatrix(selectedText);
                }
                catch (FormatException exc)
                {
                    MessageBox.Show(Application.Current.GetActiveWindow(),
                        exc.Message, Resources.MessageBoxCaptionText, MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (!s.StartsWith("{") || !s.EndsWith("}"))
            {
                throw new FormatException(Resources.EXC_InvalidFormatMatrix);
            }
            s = s.Substring(1, s.Length - 2).Trim();

            var result = new List<List<string>>();
            result.Add(new List<string>());

            CharEnumerator enumerator = s.GetEnumerator();

            int parenthesesCount = 0, curlyBracesCount = 0;
            string currentItem = String.Empty;

            while (enumerator.MoveNext())
            {
                if (enumerator.Current == '{') curlyBracesCount++;
                else if (enumerator.Current == '(') parenthesesCount++;
                else if (enumerator.Current == '}') curlyBracesCount--;
                else if (enumerator.Current == ')') parenthesesCount--;

                if (parenthesesCount == 0 && curlyBracesCount == 0 && (enumerator.Current == ',' || enumerator.Current == ';'))
                {
                    result.Last().Add(currentItem.Trim());
                    currentItem = String.Empty;

                    if (enumerator.Current == ';')
                        result.Add(new List<string>());
                }
                else if (parenthesesCount >= 0 && curlyBracesCount >= 0)
                {
                    currentItem += enumerator.Current;
                }
                else
                {
                    throw new FormatException(Resources.EXC_InvalidFormatMatrix);
                }
            }
            if (String.IsNullOrWhiteSpace(currentItem) || parenthesesCount != 0 || curlyBracesCount != 0)
                throw new FormatException(Resources.EXC_InvalidFormatMatrix);
            else
                result.Last().Add(currentItem.Trim());
            
            return result;
        }

        #endregion
    }
}
