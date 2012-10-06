using System;
using System.Windows.Media.Media3D;

using TAlex.MathCore;

namespace TAlex.WPF3DToolkit.Surfaces
{
    public class SimpleSurface
    {
        #region Fields

        private const double dx = 0.2;

        private const double dy = 0.2;

        private double _xmin = -5;

        private double _xmax = 5;

        private double _ymin = -5;

        private double _ymax = 5;

        private double _zmin = -5;

        private double _zmax = 5;

        private Function2Real _func;

        #endregion

        #region Properties

        public double Xmin
        {
            get { return _xmin; }
            set { _xmin = value; }
        }

        public double Xmax
        {
            get { return _xmax; }
            set { _xmax = value; }
        }

        public double Ymin
        {
            get { return _ymin; }
            set { _ymin = value; }
        }

        public double Ymax
        {
            get { return _ymax; }
            set { _ymax = value; }
        }

        public double Zmin
        {
            get { return _zmin; }
            set { _zmin = value; }
        }

        public double Zmax
        {
            get { return _zmax; }
            set { _zmax = value; }
        }

        public Function2Real Function
        {
            get
            {
                return _func;
            }

            set
            {
                _func = value;
            }
        }

        #endregion

        #region Constructors

        public SimpleSurface(Function2Real function)
        {
            _func = function;
        }

        #endregion

        #region Methods

        public Geometry3D BuildGeometry()
        {
            int x_count = (int)((_xmax - _xmin) / dx) + 1;
            int y_count = (int)((_ymax - _ymin) / dy) + 1;

            MeshGeometry3D geometry = new MeshGeometry3D();

            for (int i = 0; i < x_count; i++)
            {
                for (int j = 0; j < y_count; j++)
                {
                    double x = _xmin + i * dx;
                    double y = _ymin + j * dy;

                    geometry.Positions.Add(new Point3D(x, y, _func(x, y)));
                }
            }

            for (int i = 0; i < x_count; i++)
            {
                for (int j = 0; j < y_count - 1; j++)
                {
                    geometry.TriangleIndices.Add(x_count * i + j);
                    geometry.TriangleIndices.Add(x_count * i + j + 1);
                    geometry.TriangleIndices.Add(x_count * (i + 1) + j);
                    geometry.TriangleIndices.Add(x_count * (i + 1) + j);
                    geometry.TriangleIndices.Add(x_count * i + j + 1);
                    geometry.TriangleIndices.Add(x_count * (i + 1) + j + 1);
                }
            }

            return geometry;
        }

        #endregion
    }
}
