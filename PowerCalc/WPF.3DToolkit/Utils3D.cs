using System;
using System.Windows.Media.Media3D;
using TAlex.MathCore;
using _3DTools;
using System.Windows.Media;

namespace TAlex.WPF3DToolkit
{
    public static class Utils3D
    {
        public static Visual3D GetCubeRectCoordSystem(double side, double grid_step)
        {
            double halfOfSide = side / 2.0;

            const double axesThickness = 1;

            // Shafts
            ScreenSpaceLines3D lineAxisX = new ScreenSpaceLines3D();
            lineAxisX.Thickness = axesThickness;
            lineAxisX.Color = Colors.Red;
            lineAxisX.Points.Add(new Point3D(0, 0, 0));
            lineAxisX.Points.Add(new Point3D(halfOfSide / 2, 0, 0));

            ScreenSpaceLines3D lineAxisY = new ScreenSpaceLines3D();
            lineAxisY.Thickness = axesThickness;
            lineAxisY.Color = Colors.Green;
            lineAxisY.Points.Add(new Point3D(0, 0, 0));
            lineAxisY.Points.Add(new Point3D(0, halfOfSide / 2, 0));

            ScreenSpaceLines3D lineAxisZ = new ScreenSpaceLines3D();
            lineAxisZ.Thickness = axesThickness;
            lineAxisZ.Color = Colors.Blue;
            lineAxisZ.Points.Add(new Point3D(0, 0, 0));
            lineAxisZ.Points.Add(new Point3D(0, 0, halfOfSide / 2));


            // Mesh lines
            ScreenSpaceLines3D meshLines = new ScreenSpaceLines3D();
            meshLines.Color = Colors.Silver;
            meshLines.Thickness = 1;

            for (double i = -halfOfSide; i < halfOfSide; i += grid_step)
            {
                meshLines.Points.Add(new Point3D(i, -halfOfSide, -halfOfSide));
                meshLines.Points.Add(new Point3D(i, halfOfSide, -halfOfSide));
            }

            for (double i = -halfOfSide; i < halfOfSide; i += grid_step)
            {
                meshLines.Points.Add(new Point3D(i, -halfOfSide, -halfOfSide));
                meshLines.Points.Add(new Point3D(i, -halfOfSide, halfOfSide));
            }

            for (double i = -halfOfSide; i < halfOfSide; i += grid_step)
            {
                meshLines.Points.Add(new Point3D(-halfOfSide, i, -halfOfSide));
                meshLines.Points.Add(new Point3D(halfOfSide, i, -halfOfSide));
            }

            for (double i = -halfOfSide; i < halfOfSide; i += grid_step)
            {
                meshLines.Points.Add(new Point3D(-halfOfSide, i, -halfOfSide));
                meshLines.Points.Add(new Point3D(-halfOfSide, i, halfOfSide));
            }

            for (double i = -halfOfSide; i < halfOfSide; i += grid_step)
            {
                meshLines.Points.Add(new Point3D(-halfOfSide, -halfOfSide, i));
                meshLines.Points.Add(new Point3D(halfOfSide, -halfOfSide, i));
            }

            for (double i = -halfOfSide; i < halfOfSide; i += grid_step)
            {
                meshLines.Points.Add(new Point3D(-halfOfSide, -halfOfSide, i));
                meshLines.Points.Add(new Point3D(-halfOfSide, halfOfSide, i));
            }


            // Box lines
            ScreenSpaceLines3D boxLines = new ScreenSpaceLines3D();
            boxLines.Color = Colors.Black;
            boxLines.Thickness = 1;

            boxLines.Points.Add(new Point3D(-halfOfSide, halfOfSide, -halfOfSide));
            boxLines.Points.Add(new Point3D(-halfOfSide, halfOfSide, halfOfSide));

            boxLines.Points.Add(new Point3D(-halfOfSide, halfOfSide, halfOfSide));
            boxLines.Points.Add(new Point3D(-halfOfSide, -halfOfSide, halfOfSide));

            boxLines.Points.Add(new Point3D(halfOfSide, halfOfSide, halfOfSide));
            boxLines.Points.Add(new Point3D(-halfOfSide, halfOfSide, halfOfSide));

            boxLines.Points.Add(new Point3D(halfOfSide, halfOfSide, halfOfSide));
            boxLines.Points.Add(new Point3D(halfOfSide, -halfOfSide, halfOfSide));

            boxLines.Points.Add(new Point3D(halfOfSide, -halfOfSide, halfOfSide));
            boxLines.Points.Add(new Point3D(halfOfSide, -halfOfSide, -halfOfSide));

            boxLines.Points.Add(new Point3D(halfOfSide, -halfOfSide, halfOfSide));
            boxLines.Points.Add(new Point3D(-halfOfSide, -halfOfSide, halfOfSide));

            boxLines.Points.Add(new Point3D(halfOfSide, halfOfSide, halfOfSide));
            boxLines.Points.Add(new Point3D(halfOfSide, halfOfSide, -halfOfSide));

            boxLines.Points.Add(new Point3D(halfOfSide, halfOfSide, -halfOfSide));
            boxLines.Points.Add(new Point3D(-halfOfSide, halfOfSide, -halfOfSide));

            boxLines.Points.Add(new Point3D(halfOfSide, halfOfSide, -halfOfSide));
            boxLines.Points.Add(new Point3D(halfOfSide, -halfOfSide, -halfOfSide));


            boxLines.Points.Add(new Point3D(-halfOfSide, -halfOfSide, -halfOfSide));
            boxLines.Points.Add(new Point3D(-halfOfSide, -halfOfSide, halfOfSide));

            boxLines.Points.Add(new Point3D(-halfOfSide, -halfOfSide, -halfOfSide));
            boxLines.Points.Add(new Point3D(-halfOfSide, halfOfSide, -halfOfSide));

            boxLines.Points.Add(new Point3D(-halfOfSide, -halfOfSide, -halfOfSide));
            boxLines.Points.Add(new Point3D(halfOfSide, -halfOfSide, -halfOfSide));


            ModelVisual3D coordSys = new ModelVisual3D();
            coordSys.Children.Add(lineAxisX);
            coordSys.Children.Add(lineAxisY);
            coordSys.Children.Add(lineAxisZ);
    
            coordSys.Children.Add(meshLines);
            coordSys.Children.Add(boxLines);

            return coordSys;
        }

        public static Point3DCollection GetHodographPoints(
            Function1Real fn1, Function1Real fn2, Function1Real fn3,
            double dt, double t0, double t1)
        {
            Point3DCollection points = new Point3DCollection();

            int t_count = (int)(Math.Abs(t1 - t0) / dt);

            for (int i = 0; i < t_count; i++)
            {
                double t = t0 + i * dt;

                points.Add(new Point3D(fn1(t), fn2(t), fn3(t)));
                points.Add(new Point3D(fn1(t + dt), fn2(t + dt), fn3(t + dt)));
            }

            return points;
        }

        public static Point3DCollection GetTrackPoints(
            Function1Real fn1, Function1Real fn2, Function2Real fn3,
            double dt, double t0, double t1)
        {
            Point3DCollection points = new Point3DCollection();

            int t_count = (int)(Math.Abs(t1 - t0) / dt);

            for (int i = 0; i < t_count; i++)
            {
                double t = dt + i * dt;

                double xt = fn1(t);
                double yt = fn2(t);
                points.Add(new Point3D(xt, yt, fn3(xt, yt)));

                xt = fn1(t + dt);
                yt = fn2(t + dt);
                points.Add(new Point3D(xt, yt, fn3(xt, yt)));
            }

            return points;
        }
    }
}