using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TAlex.Common.Environment;
using TAlex.Common.Extensions;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;
using TAlex.WPF.Mvvm;


namespace TAlex.PowerCalc.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        public readonly IExpressionTreeBuilder<Object> ExpressionTreeBuilder;

        private bool _canShowXYCoords;
        private double? _xCoordStatusBar;
        private double? _yCoordStatusBar;

        #endregion

        #region Properties

        public virtual string WindowTitle
        {
            get
            {
                string productTitle = ApplicationInfo.Title;

                if (Licensing.License.IsTrial)
                    return String.Format("{0} - Evaluation version (days left: {1})", productTitle, Licensing.License.TrialDaysLeft);
                else
                    return productTitle;
            }
        }

        public virtual string AboutMenuItemHeader
        {
            get
            {
                string productTitle = ApplicationInfo.Title;
                return "_About " + productTitle;
            }
        }

        public bool CanShowXYCoords
        {
            get
            {
                return _canShowXYCoords;
            }

            set
            {
                Set<bool>(() => CanShowXYCoords, ref _canShowXYCoords, value);
            }
        }

        public double? XCoord2dPlot
        {
            get
            {
                return _xCoordStatusBar;
            }

            set
            {
                _xCoordStatusBar = value;
                RaisePropertyChanged(ExpressionExtensions.GetPropertyName<MainWindowViewModel>(x => x.XCoord2dPlot));
            }
        }

        public double? YCoord2dPlot
        {
            get
            {
                return _yCoordStatusBar;
            }

            set
            {
                _yCoordStatusBar = value;
                RaisePropertyChanged(ExpressionExtensions.GetPropertyName<MainWindowViewModel>(x => x.YCoord2dPlot));
            }
        }

        #endregion

        #region Constructors

        public MainWindowViewModel(IExpressionTreeBuilder<Object> treeBuilder)
        {
            ExpressionTreeBuilder = treeBuilder;
        }

        #endregion
    }
}
