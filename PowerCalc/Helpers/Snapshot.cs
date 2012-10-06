using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.IO;

namespace TAlex.PowerCalc.Helpers
{
    /// <summary>
    /// Capture and render content as an image.
    /// </summary>
    public static class Snapshot
    {
        public static void ToClipboard(Visual visual)
        {
            Clipboard.SetImage(ToBitmap(visual));
        }

        public static void ToFile(Visual visual, Window owner)
        {
            Microsoft.Win32.SaveFileDialog sfd = new Microsoft.Win32.SaveFileDialog();
            sfd.DefaultExt = "png";
            sfd.FileName = "Snapshot";
            sfd.Filter = "PNG (*.png)|*.png";

            if (sfd.ShowDialog(owner) == true)
            {
                PngBitmapEncoder png = new PngBitmapEncoder();
                png.Frames.Add(BitmapFrame.Create(ToBitmap(visual)));

                using (Stream file = sfd.OpenFile())
                {
                    png.Save(file);
                }
            }
        }

        public static BitmapSource ToBitmap(Visual visual)
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(visual);
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                (int)bounds.Width,
                (int)bounds.Height,
                96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);

            return rtb;
        }
    }
}
