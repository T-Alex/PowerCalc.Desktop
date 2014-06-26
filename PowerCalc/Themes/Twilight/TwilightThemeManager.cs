using System;
using System.Linq;
using System.Windows;
using System.IO;
using TAlex.WPF.Theming;
using System.Collections.ObjectModel;
using System.Collections.Generic;


namespace TAlex.WPFThemes.Twilight
{
    public class TwilightThemeManager : IThemeManager
    {
        #region Fields

        private static TwilightThemeManager _instance;
        private static object _syncObj = new object();

        private static string _assemblyName;

        private static readonly ReadOnlyCollection<ThemeInfo> _avaliableThemes;

        private static string _uriThemeDir;

        #endregion

        #region Properties

        public static TwilightThemeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock(_syncObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new TwilightThemeManager();
                        }
                    }
                }

                return _instance;
            }
        }


        public string CurrentTheme { get; private set; }

        public ReadOnlyCollection<ThemeInfo> AvailableThemes
        {
            get
            {
                return _avaliableThemes;
            }
        }

        #endregion

        #region Events

        public event ThemeChangedEventHandler ThemeChanged;

        #endregion

        #region Constructors

        static TwilightThemeManager()
        {
            _assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            _uriThemeDir = @"/" + _assemblyName + ";" + Path.Combine("component", "Themes", "Twilight");
            _avaliableThemes = new ReadOnlyCollection<ThemeInfo>(new List<ThemeInfo>
            {
                new ThemeInfo("Blue", "Blue"),
                new ThemeInfo("Silver", "Grayscale"),
                new ThemeInfo("Black", "Grayscale")
            });
        }

        protected TwilightThemeManager()
        {
        }

        #endregion

        #region Methods

        public bool ApplyTheme(string themeName)
        {
            if (!AvailableThemes.Any(x => x.Name == themeName))
            {
                return false;
            }

            ResourceDictionary resources = Application.Current.Resources.MergedDictionaries[0];
            resources.Clear();
            resources.MergedDictionaries.Add(Application.LoadComponent(new Uri(Path.Combine(_uriThemeDir, "ColorSchemes", themeName + ".xaml"), UriKind.Relative)) as ResourceDictionary);
            resources.MergedDictionaries.Add(Application.LoadComponent(new Uri(Path.Combine(_uriThemeDir, "Shared.xaml"), UriKind.Relative)) as ResourceDictionary);
            resources.MergedDictionaries.Add(Application.LoadComponent(new Uri(Path.Combine(_uriThemeDir, "NumericUpDown.xaml"), UriKind.Relative)) as ResourceDictionary);

            resources.MergedDictionaries.Add(Application.LoadComponent(new Uri(Path.Combine(_uriThemeDir, "DataGrid.xaml"), UriKind.Relative)) as ResourceDictionary);

            CurrentTheme = themeName;
            OnThemeChanged(themeName);
            return true;
        }

        protected virtual void OnThemeChanged(string themeName)
        {
            if (ThemeChanged != null)
            {
                ThemeChanged(this, new ThemeEventArgs(themeName));
            }
        }

        #endregion
    }
}
