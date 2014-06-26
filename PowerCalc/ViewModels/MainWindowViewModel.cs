using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TAlex.Common.Environment;
using TAlex.Common.Extensions;
using TAlex.Common.Licensing;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;
using TAlex.PowerCalc.ViewModels.Worksheet;
using TAlex.WPF.Mvvm;
using TAlex.WPF.Mvvm.Commands;
using TAlex.WPF.Theming;


namespace TAlex.PowerCalc.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        protected readonly ApplicationInfo ApplicationInfo;
        protected readonly LicenseBase AppLicense;
        protected readonly IThemeManager ThemeManager;
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

                if (AppLicense.IsTrial)
                    return String.Format("{0} - Evaluation version (days left: {1})", productTitle, AppLicense.TrialDaysLeft);
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

        public virtual List<ColorScheme> ColorSchemes
        {
            get
            {
                return ThemeManager.AvailableThemes.Select(x => new ColorScheme(ThemeManager, x.Name, x.DisplayName)).ToList();
            }
        }


        public WorksheetModel Worksheet { get; set; }


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
                Set<double?>(() => XCoord2dPlot, ref _xCoordStatusBar, value);
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

        public MainWindowViewModel(ApplicationInfo applicationInfo, LicenseBase appLicense, IExpressionTreeBuilder<Object> treeBuilder, WorksheetModel worksheetModel, IThemeManager themeManager)
        {
            ApplicationInfo = applicationInfo;
            AppLicense = appLicense;
            ExpressionTreeBuilder = treeBuilder;

            Worksheet = worksheetModel;
            ThemeManager = themeManager;
        }

        #endregion

        #region Nested Types

        public class ColorScheme
        {
            public string Name { get; private set; }

            public string DisplayName { get; private set; }

            public ICommand ApplySchemeCommand { get; private set; }


            public ColorScheme(IThemeManager themeManager, string name, string displayName)
            {
                Name = name;
                DisplayName = displayName;
                ApplySchemeCommand = new RelayCommand(() => { themeManager.ApplyTheme(name); });
            }
        }

        #endregion
    }
}
