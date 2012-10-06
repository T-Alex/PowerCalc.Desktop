using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace TAlex.WPF3DToolkit
{
    public class Sphere3D
    {
        #region Fields

        protected int n = 10;

        protected double r = 20;

        #endregion

        #region Properties

        public virtual double Radius
        {
            get
            {
                return r;
            }

            set
            {
                r = value;
            }
        }

        public virtual int Separators
        {
            get
            {
                return n;
            }

            set
            {
                n = value;
            }
        }

        #endregion

        #region Constructors

        public Sphere3D(double radius)
        {
            r = radius;
        }

        #endregion

        #region Methods

        public Geometry3D BuildGeometry()
        {
            int e;
            double segmentRad = Math.PI / 2 / (n + 1);
            int numberOfSeparators = 4 * n + 4;

            MeshGeometry3D mesh = new MeshGeometry3D();

            for (e = -n; e <= n; e++)
            {
                double r_e = r * Math.Cos(segmentRad * e);
                double y_e = r * Math.Sin(segmentRad * e);

                for (int s = 0; s <= (numberOfSeparators - 1); s++)
                {
                    double z_s = r_e * Math.Sin(segmentRad * s) * (-1);
                    double x_s = r_e * Math.Cos(segmentRad * s);
                    mesh.Positions.Add(new Point3D(x_s, y_e, z_s));
                }
            }

            mesh.Positions.Add(new Point3D(0, r, 0));
            mesh.Positions.Add(new Point3D(0, -1 * r, 0));

            for (e = 0; e < 2 * n; e++)
            {
                for (int i = 0; i < numberOfSeparators; i++)
                {
                    mesh.TriangleIndices.Add(e * numberOfSeparators + i);
                    mesh.TriangleIndices.Add(e * numberOfSeparators + i + numberOfSeparators);
                    mesh.TriangleIndices.Add(e * numberOfSeparators + (i + 1) % numberOfSeparators + numberOfSeparators);

                    mesh.TriangleIndices.Add(e * numberOfSeparators + (i + 1) % numberOfSeparators + numberOfSeparators);
                    mesh.TriangleIndices.Add(e * numberOfSeparators + (i + 1) % numberOfSeparators);
                    mesh.TriangleIndices.Add(e * numberOfSeparators + i);
                }
            }

            for (int i = 0; i < numberOfSeparators; i++)
            {
                mesh.TriangleIndices.Add(e * numberOfSeparators + i);
                mesh.TriangleIndices.Add(e * numberOfSeparators + (i + 1) % numberOfSeparators);
                mesh.TriangleIndices.Add(numberOfSeparators * (2 * n + 1));
            }

            for (int i = 0; i < numberOfSeparators; i++)
            {
                mesh.TriangleIndices.Add(i);
                mesh.TriangleIndices.Add((i + 1) % numberOfSeparators);
                mesh.TriangleIndices.Add(numberOfSeparators * (2 * n + 1) + 1);
            }

            return mesh;
        }

        #endregion
    }
}
