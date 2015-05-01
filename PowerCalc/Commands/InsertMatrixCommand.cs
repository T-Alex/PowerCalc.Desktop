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
using TAlex.Mvvm.Extensions;


namespace TAlex.PowerCalc.Commands
{
    public class InsertMatrixCommand : ICommand
    {
        #region Fields

        private const int MaxMatrixSize = 100;

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
                catch (Exception exc)
                {
                    MessageBox.Show(Application.Current.GetActiveWindow(),
                        exc.Message, Resources.MessageBoxCaptionText, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

            var dataContext = new InsertMatrixViewModel(String.IsNullOrWhiteSpace(selectedText) ? InsertMatrixViewModel.MatrixViewMode.Insert : InsertMatrixViewModel.MatrixViewMode.Edit);
            if (matrix == null)
                dataContext.Matrix = new MatrixViewModel(AppSettings.WorksheetInsertMatrixDefaultRowsCount, AppSettings.WorksheetInsertMatrixDefaultColumnsCount);
            else
                dataContext.Matrix = new MatrixViewModel(matrix);

            var dialog = new InsertMatrixWindow
            {
                Owner = Application.Current.GetActiveWindow(),
                DataContext = dataContext
            };

            if (dialog.ShowDialog() == true)
            {
                string tempText2 = ClipboardService.GetText();
                ClipboardService.SetText(dataContext.Matrix.ToString());
                ApplicationCommands.Paste.Execute(null, null);
                ClipboardService.SetText(tempText2);
            }
        }

        #endregion

        #region Helpers

        private List<List<string>> ParseMatrix(string text)
        {
            string s = text.Trim();
            if (!s.StartsWith("{") || !s.EndsWith("}"))
            {
                throw new FormatException(Resources.EXC_InvalidFormatMatrixCurlyBrackets);
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
                    throw new FormatException(Resources.EXC_InvalidFormatMatrixClosingBracketMatching);
                }
            }

            if (parenthesesCount != 0 || curlyBracesCount != 0)
                throw new FormatException(Resources.EXC_InvalidFormatMatrixBracketMatching);

            if (String.IsNullOrWhiteSpace(currentItem))
                throw new FormatException(Resources.EXC_InvalidFormatMatrixLastItem);
            
            result.Last().Add(currentItem.Trim());

            if (result.Any(r => r.Count != result.First().Count))
                throw new FormatException(Resources.EXC_InvalidFormatMatrixRowLengths);
            if (result.Count > MaxMatrixSize || result.Last().Count > MaxMatrixSize)
                throw new Exception(Resources.EXC_MatrixSizeIsTooBig);

            return result;
        }

        #endregion
    }
}
