using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TAlex.WPF.Theming;


namespace TAlex.PowerCalc.Controls
{
    public class ThemedImage : Image
    {
        #region Fields

        private static readonly Regex ThemeNameRegex = new Regex(".(?<theme>[0-9a-zA-z]{1,20})(?<ext>.[0-9a-zA-z]{1,5})$", RegexOptions.Compiled);

        private IThemeManager ThemeManager;

        #endregion

        #region Constructors

        public ThemedImage()
        {
            ThemeManager = ThemeLocator.Manager;
            ThemeManager.ThemeChanged += Instance_ThemeChanged;
        }

        #endregion

        #region Methods

        private void Instance_ThemeChanged(object sender, ThemeEventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            ImageSource source = Source;
            Size size = RenderSize;

            if (source == null)
            {
                return;
            }

            var uri = Source.ToString();

            Uri newUri;
            if (Uri.TryCreate(NewImageUri(uri, ThemeManager.CurrentTheme), UriKind.RelativeOrAbsolute, out newUri))
            {
                try
                {
                    BitmapImage bi = new BitmapImage(newUri);
                    source = bi;
                    size = new Size(bi.Width / 0.75, bi.Height / 0.75);
                }
                catch (IOException)
                {
                }
                
            }
            dc.DrawImage(source, new Rect(new Point(), size));
        }

        #endregion

        #region Helpers

        private string NewImageUri(string uri, string newTheme)
        {
            return ThemeNameRegex.Replace(uri, String.Format(".{0}${{ext}}", ThemeManager.AvailableThemes.Single(x => x.Name == newTheme).FamilyName));
        }

        #endregion
    }
}
