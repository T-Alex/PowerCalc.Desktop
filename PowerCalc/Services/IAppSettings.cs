using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace TAlex.PowerCalc.Services
{
    public interface IAppSettings
    {
        WindowState WindowState { get; set; }
        Rect WindowBounds { get; set; }
        string ExtensionsPath { get; }

        int ZeroThreshold { get; set; }
        int ComplexThreshold { get; set; }
        string NumericFormat { get; set; }
        string ColorScheme { get; set; }

        int WorksheetMaxMatrixRows { get; set; }
        int WorksheetMaxMatrixCols { get; set; }
        double WorksheetFontSize { get; set; } // !!!!!!!!!!!!!!
        
        Color Plot2DBackground { get; set; }


        void Save();
    }
}
