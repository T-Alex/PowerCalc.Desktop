using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TAlex.PowerCalc.ViewModels;
using TAlex.PowerCalc.Views;
using TAlex.WPF.Mvvm.Extensions;


namespace TAlex.PowerCalc.Commands
{
    public class InsertMatrixCommand : ICommand
    {
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
            var dataContext = new InsertMatrixViewModel { Matrix = new MatrixViewModel(3, 3) };
            var dialog = new InsertMatrixWindow
            {
                Owner = Application.Current.GetActiveWindow(),
                DataContext = dataContext
            };

            if (dialog.ShowDialog() == true)
            {
                Clipboard.SetText(dataContext.Matrix.ToString());
                ApplicationCommands.Paste.Execute(null, null);
            }
        }

        #endregion

        #region Helpers

        #endregion
    }
}
