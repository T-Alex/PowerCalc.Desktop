using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Data;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Media.Animation;
using TAlex.Common.Environment;


namespace TAlex.PowerCalc
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        #region Constructors

        protected AboutWindow()
        {
            InitializeComponent();
        }

        public AboutWindow(Window parent) : this()
        {
            this.Owner = parent;
        }

        #endregion

        #region Methods

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Close();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // Easter egg.
            if (e.Key == Key.L && licTextLabel.Opacity == 1.0)
            {
                DoubleAnimationUsingKeyFrames licenseInfoOpacityAnim = new DoubleAnimationUsingKeyFrames();
                licenseInfoOpacityAnim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
                licenseInfoOpacityAnim.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(5000))));
                licenseInfoOpacityAnim.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(42000))));
                licenseInfoOpacityAnim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(47000))));

                licTextLabel.BeginAnimation(UIElement.OpacityProperty, licenseInfoOpacityAnim);
                licNameLabel.BeginAnimation(UIElement.OpacityProperty, licenseInfoOpacityAnim);
                unregTextLabel.BeginAnimation(UIElement.OpacityProperty, licenseInfoOpacityAnim);


                DoubleAnimationUsingKeyFrames dedicatedOpacityAnim = new DoubleAnimationUsingKeyFrames();
                dedicatedOpacityAnim.BeginTime = TimeSpan.FromMilliseconds(7000);
                dedicatedOpacityAnim.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
                dedicatedOpacityAnim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(3000))));
                dedicatedOpacityAnim.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(6000))));
                dedicatedLabel.BeginAnimation(UIElement.OpacityProperty, dedicatedOpacityAnim);


                DoubleAnimationUsingKeyFrames LoveOpacityAnim = new DoubleAnimationUsingKeyFrames();
                LoveOpacityAnim.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(0))));
                LoveOpacityAnim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(3000))));
                LoveOpacityAnim.KeyFrames.Add(new LinearDoubleKeyFrame(1.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(14000))));
                LoveOpacityAnim.KeyFrames.Add(new LinearDoubleKeyFrame(0.0, KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(17000))));

                // Show "I" word
                LoveOpacityAnim.BeginTime = TimeSpan.FromMilliseconds(14000);
                ILabel.BeginAnimation(UIElement.OpacityProperty, LoveOpacityAnim);

                // Show "Love" word
                LoveOpacityAnim.BeginTime = TimeSpan.FromMilliseconds(16000);
                LoveLabel.BeginAnimation(UIElement.OpacityProperty, LoveOpacityAnim);

                // Show "You" word
                LoveOpacityAnim.BeginTime = TimeSpan.FromMilliseconds(18000);
                YouLabel.BeginAnimation(UIElement.OpacityProperty, LoveOpacityAnim);

                // Show "Ksenia" word
                LoveOpacityAnim.BeginTime = TimeSpan.FromMilliseconds(21000);
                KseniaLabel.BeginAnimation(UIElement.OpacityProperty, LoveOpacityAnim);

                // Show "Savitskaya" word
                LoveOpacityAnim.BeginTime = TimeSpan.FromMilliseconds(23000);
                SavitskayaLabel.BeginAnimation(UIElement.OpacityProperty, LoveOpacityAnim);
            }
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            if (e.Uri != null && !String.IsNullOrEmpty(e.Uri.OriginalString))
            {
                string uri = e.Uri.AbsoluteUri;
                Process.Start(new ProcessStartInfo(uri));
                e.Handled = true;
            }
        }

        #endregion
    }
}
