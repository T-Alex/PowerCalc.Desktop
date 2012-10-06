using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace TAlex.WPF3DToolkit
{
    public class Polyline3D
    {
        #region Fields

        private double _thickness;

        private Point3DCollection _points;

        private Color _color;

        #endregion

        #region Properties

        public Point3DCollection Points
        {
            get
            {
                return _points;
            }

            set
            {
                _points = value;
            }
        }

        public double Thickness
        {
            get
            {
                return _thickness;
            }

            set
            {
                _thickness = value;
            }
        }

        public Color Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
            }
        }

        #endregion

        #region Methods



        #endregion
    }
}
