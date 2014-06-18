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
        int WorksheetMaxMatrixColumns { get; set; }

        int MatricesWorksheetRows { get; set; }
        int MatricesWorksheetColumns { get; set; }

        Color Plot2DBackground { get; set; }
        Color Plot2DForeground { get; set; }
        Color Plot2DGridlinesColor { get; set; }
        Color Plot2DAxisColor { get; set; }
        Color Plot2DSelectionRegionColor { get; set; }
        bool Plot2DVertGridlinesVisible { get; set; }
        bool Plot2DHorizGridlinesVisible { get; set; }
        bool Plot2DXAxisVisible { get; set; }
        bool Plot2DYAxisVisible { get; set; }

        void Save();
    }
}
