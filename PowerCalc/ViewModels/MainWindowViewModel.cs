using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using TAlex.Common.Extensions;
using TAlex.Common.Models;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;
using TAlex.Mvvm.Commands;
using TAlex.Mvvm.ViewModels;
using TAlex.PowerCalc.ViewModels.Worksheet;
using TAlex.WPF.Theming;


namespace TAlex.PowerCalc.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Fields

        protected readonly AssemblyInfo AssemblyInfo;
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
                return AssemblyInfo.Title;
            }
        }

        public virtual string AboutMenuItemHeader
        {
            get
            {
                string productTitle = AssemblyInfo.Title;
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

        public MainWindowViewModel(AssemblyInfo assemblyInfo, IExpressionTreeBuilder<Object> treeBuilder, WorksheetModel worksheetModel, IThemeManager themeManager)
        {
            AssemblyInfo = assemblyInfo;
            ExpressionTreeBuilder = treeBuilder;

            Worksheet = worksheetModel;
            ThemeManager = themeManager;
        }

        #endregion

        #region Nested Types

        public class ColorScheme
        {
            private IThemeManager _themeManager;


            public string Name { get; private set; }

            public string DisplayName { get; private set; }

            public bool IsCurrent
            {
                get
                {
                    return Name == _themeManager.CurrentTheme;
                }
            }

            public ICommand ApplySchemeCommand { get; private set; }


            public ColorScheme(IThemeManager themeManager, string name, string displayName)
            {
                _themeManager = themeManager;

                Name = name;
                DisplayName = displayName;
                ApplySchemeCommand = new RelayCommand(() => { themeManager.ApplyTheme(name); });
            }
        }

        #endregion
    }
}
