using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

using TAlex.PowerCalc.Helpers;


namespace TAlex.PowerCalc.Controls
{
    public class Plot2D : FrameworkElement
    {
        #region Fields

        private const double DefaultStepX = 40.0;

        private const double DefaultStepY = 40.0;

        private const double DefaultXAxisScaleInterval = 1.0;

        private const double DefaultYAxisScaleInterval = 1.0;


        private readonly List<SimpleTrace> _traces = new List<SimpleTrace>();

        private Point _lastMousePos = new Point();

        /// <summary>
        /// Horizontal displacement of the center of the Plot2D.
        /// </summary>
        private double _horizOffset = 0.0;

        /// <summary>
        /// Vertical displacement of the center of the Plot2D.
        /// </summary>
        private double _vertOffset = 0.0;


        private Typeface _typeface = new Typeface("Segoe UI");

        private double _fontSize = 8.0;

        private string _numericFormat = "G4";

        private Brush _splashBrush = new SolidColorBrush(Color.FromArgb(64, 128, 128, 200));

        private string _splashText = "Moving Plot";

        private Typeface _splashTypeface = new Typeface("Colibri");

        private double _splashEmSize = 40.0;


        private DrawingVisual _drawingVisual = new DrawingVisual();

        //----------------------------------------------------
        private DrawingVisual _selectedRegionVisual;

        private Point _selectedRegionPoint1 = new Point();

        //----------------------------------------------------

        private DragMode? _currentDragState = null;


        /// <summary>
        /// Size of the division on the x-axis.
        /// </summary>
        private double _dx;

        /// <summary>
        /// Size of the division on the y-axis.
        /// </summary>
        private double _dy;

        /// <summary>
        /// Scale interval on the x-axis.
        /// </summary>
        private double _xAxisScaleInterval;

        /// <summary>
        /// Scale interval on the y-axis.
        /// </summary>
        private double _yAxisScaleInterval;


        private const double borderSize = 25;

        private bool _fullScreen = false;

        private Stack<ViewportState> _undoStack = new Stack<ViewportState>();


        public static readonly DependencyProperty VertGridLinesVisibleProperty;

        public static readonly DependencyProperty HorizGridLinesVisibleProperty;

        public static readonly DependencyProperty XAxisVisibleProperty;

        public static readonly DependencyProperty YAxisVisibleProperty;

        public static readonly DependencyProperty BackgroundProperty;

        public static readonly DependencyProperty ForegroundProperty;

        public static readonly DependencyProperty BorderPenProperty;

        public static readonly DependencyProperty GridPenProperty;

        public static readonly DependencyProperty AxisPenProperty;

        public static readonly DependencyProperty SelectedRegionBrushProperty;

        public static readonly DependencyProperty SelectedRegionBorderPenProperty;

        #endregion

        #region Properties

        public bool VertGridLinesVisible
        {
            get
            {
                return (bool)base.GetValue(VertGridLinesVisibleProperty);
            }

            set
            {
                base.SetValue(VertGridLinesVisibleProperty, value);
            }
        }

        public bool HorizGridLinesVisible
        {
            get
            {
                return (bool)base.GetValue(HorizGridLinesVisibleProperty);
            }

            set
            {
                base.SetValue(HorizGridLinesVisibleProperty, value);
            }
        }

        public bool XAxisVisible
        {
            get
            {
                return (bool)base.GetValue(XAxisVisibleProperty);
            }

            set
            {
                base.SetValue(XAxisVisibleProperty, value);
            }
        }

        public bool YAxisVisible
        {
            get
            {
                return (bool)base.GetValue(YAxisVisibleProperty);
            }

            set
            {
                base.SetValue(YAxisVisibleProperty, value);
            }
        }

        public double XAxisScaleInterval
        {
            get
            {
                return _xAxisScaleInterval;
            }

            set
            {
                if (value <= 0.0)
                    throw new InvalidOperationException();

                _xAxisScaleInterval = value;
            }
        }

        public double YAxisScaleInterval
        {
            get
            {
                return _yAxisScaleInterval;
            }

            set
            {
                if (value <= 0.0)
                    throw new InvalidOperationException();

                _yAxisScaleInterval = value;
            }
        }

        public double StepX
        {
            get
            {
                return _dx;
            }
        }

        public double StepY
        {
            get
            {
                return _dy;
            }
        }

        public Brush Background
        {
            get
            {
                return (Brush)base.GetValue(BackgroundProperty);
            }

            set
            {
                base.SetValue(BackgroundProperty, value);
            }
        }

        public Brush Foreground
        {
            get
            {
                return (Brush)base.GetValue(ForegroundProperty);
            }

            set
            {
                base.SetValue(ForegroundProperty, value);
            }
        }

        public Pen BorderPen
        {
            get
            {
                return (Pen)base.GetValue(BorderPenProperty);
            }

            set
            {
                base.SetValue(BorderPenProperty, value);
            }
        }

        public Pen GridPen
        {
            get
            {
                return (Pen)base.GetValue(GridPenProperty);
            }

            set
            {
                base.SetValue(GridPenProperty, value);
            }
        }

        public Pen AxisPen
        {
            get
            {
                return (Pen)base.GetValue(AxisPenProperty);
            }

            set
            {
                base.SetValue(AxisPenProperty, value);
            }
        }

        public Brush SelectedRegionBrush
        {
            get
            {
                return (Brush)base.GetValue(SelectedRegionBrushProperty);
            }

            set
            {
                base.SetValue(SelectedRegionBrushProperty, value);
            }
        }

        public Pen SelectedRegionBorderPen
        {
            get
            {
                return (Pen)base.GetValue(SelectedRegionBorderPenProperty);
            }

            set
            {
                base.SetValue(SelectedRegionBorderPenProperty, value);
            }
        }

        public bool FullScreen
        {
            get
            {
                return _fullScreen;
            }

            set
            {
                if (_fullScreen != value)
                {
                    if (value == true)
                    {
                        Window window = new Window();
                        window.WindowState = WindowState.Maximized;
                        window.WindowStyle = WindowStyle.None;
                        window.ShowInTaskbar = false;
                        window.Topmost = true;
                        window.Tag = this;

                        Plot2D plot = new Plot2D();
                        Copy(this, plot, true);
                        plot.ContextMenu = CreateContextMenuForFullscreen(plot, window);
                        plot._fullScreen = true;

                        plot.KeyDown += new KeyEventHandler(delegate(object o, KeyEventArgs args)
                            { if (args.Key == Key.Escape) plot.FullScreen = false; }
                        );
                        plot.Focus();

                        window.Content = plot;
                        window.ShowDialog();
                    }
                    else
                    {
                        Window window = this.Parent as Window;
                        Plot2D plot = (Plot2D)window.Tag;
                        Copy(this, plot, false);

                        window.Close();
                    }
                }
            }
        }

        public List<SimpleTrace> Traces
        {
            get
            {
                return _traces;
            }
        }

        public bool CanUndo
        {
            get
            {
                if (_undoStack.Count == 0)
                    return (new ViewportState(this) != ViewportState.DefaultViewportState);
                else
                    return true;
            }
        }

        #endregion

        #region Constructors
        
        static Plot2D()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Plot2D), new FrameworkPropertyMetadata(typeof(Plot2D)));

            VertGridLinesVisibleProperty = DependencyProperty.Register("VertGridLinesVisible", typeof(bool), typeof(Plot2D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
            HorizGridLinesVisibleProperty = DependencyProperty.Register("HorizGridLinesVisible", typeof(bool), typeof(Plot2D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
            XAxisVisibleProperty = DependencyProperty.Register("XAxisVisible", typeof(bool), typeof(Plot2D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
            YAxisVisibleProperty = DependencyProperty.Register("YAxisVisible", typeof(bool), typeof(Plot2D), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
            BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(Plot2D), new FrameworkPropertyMetadata(Brushes.WhiteSmoke, FrameworkPropertyMetadataOptions.AffectsRender));
            ForegroundProperty = DependencyProperty.Register("Foreground", typeof(Brush), typeof(Plot2D), new FrameworkPropertyMetadata(Brushes.DimGray, FrameworkPropertyMetadataOptions.AffectsRender));
            BorderPenProperty = DependencyProperty.Register("BorderPen", typeof(Pen), typeof(Plot2D), new FrameworkPropertyMetadata(new Pen(Brushes.DimGray, 1), FrameworkPropertyMetadataOptions.AffectsRender));
            GridPenProperty = DependencyProperty.Register("GridPen", typeof(Pen), typeof(Plot2D), new FrameworkPropertyMetadata(new Pen(Brushes.Silver, 1), FrameworkPropertyMetadataOptions.AffectsRender));
            AxisPenProperty = DependencyProperty.Register("AxisPen", typeof(Pen), typeof(Plot2D), new FrameworkPropertyMetadata(new Pen(Brushes.DarkGray, 1.5), FrameworkPropertyMetadataOptions.AffectsRender));

            Brush selectedRegionBrush = new SolidColorBrush(Color.FromArgb(96, 51, 153, 255));
            SelectedRegionBrushProperty = DependencyProperty.Register("SelectedRegionBrush", typeof(Brush), typeof(Plot2D), new PropertyMetadata(selectedRegionBrush));

            Pen selectedRegionBorderPen = new Pen(Brushes.CornflowerBlue, 2);
            selectedRegionBorderPen.DashStyle = DashStyles.Dash;
            SelectedRegionBorderPenProperty = DependencyProperty.Register("SelectedRegionBorderPen", typeof(Pen), typeof(Plot2D), new PropertyMetadata(selectedRegionBorderPen));

            CommandManager.RegisterClassCommandBinding(typeof(Plot2D), new CommandBinding(ApplicationCommands.Undo, new ExecutedRoutedEventHandler(OnExecutedUndo), new CanExecuteRoutedEventHandler(OnCanExecuteUndo)));
        }

        public Plot2D()
        {
            _dx = DefaultStepX;
            _dy = DefaultStepY;
            _xAxisScaleInterval = DefaultXAxisScaleInterval;
            _yAxisScaleInterval = DefaultYAxisScaleInterval;

            ClipToBounds = true;
            Focusable = true;
            FocusVisualStyle = null;
            AddVisualChild(_drawingVisual);
        }

        #endregion

        #region Methods

        #region Event Handlers

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (!IsFocused) Focus();

            if (e.ChangedButton == MouseButton.Middle)
            {
                if (e.ClickCount == 2)
                {
                    ResetViewport();
                }
            }
            else if (e.ChangedButton == MouseButton.Left)
            {
                if (e.ClickCount == 2)
                {
                    FullScreen = !FullScreen;
                    return;
                }

                _undoStack.Push(new ViewportState(this));

                if (Keyboard.IsKeyDown(Key.LeftCtrl))
                    _currentDragState = DragMode.MarqueeZoom;
                else
                    _currentDragState = DragMode.HandTool;

                if (_currentDragState == DragMode.HandTool)
                {
                    _lastMousePos = e.GetPosition(this);
                }
                else if (_currentDragState == DragMode.MarqueeZoom)
                {
                    _selectedRegionVisual = new DrawingVisual();
                    _drawingVisual.Children.Add(_selectedRegionVisual);
                    _selectedRegionPoint1 = e.GetPosition(this);
                }

                // Make sure we get the MouseLeftButtonUp event even if the user
                // moves off the Canvas. Otherwise, two selection squares could be drawn at once.
                CaptureMouse();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_currentDragState == DragMode.MarqueeZoom)
                {
                    Point p2 = e.GetPosition(this);

                    Point lt = PointToPlot2DCoordinate(new Point(
                        Math.Min(_selectedRegionPoint1.X, p2.X),
                        Math.Min(_selectedRegionPoint1.Y, p2.Y))
                        );

                    Point rb = PointToPlot2DCoordinate(new Point(
                        Math.Max(_selectedRegionPoint1.X, p2.X),
                        Math.Max(_selectedRegionPoint1.Y, p2.Y))
                        );

                    Rect newViewport = new Rect(lt.X, lt.Y, Math.Abs(rb.X - lt.X), Math.Abs(lt.Y - rb.Y));
                    SetPlot2DViewport(newViewport);

                    _drawingVisual.Children.Remove(_selectedRegionVisual);
                }

                ReleaseMouseCapture();

                _currentDragState = null;
                RenderPlot();

                if (_undoStack.Count > 0 && (_undoStack.Peek() == new ViewportState(this)))
                    _undoStack.Pop();
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && IsMouseCaptured)
            {
                if (_currentDragState == DragMode.HandTool)
                {
                    Point p = e.GetPosition(this);

                    _horizOffset += p.X - _lastMousePos.X;
                    _vertOffset += p.Y - _lastMousePos.Y;

                    _lastMousePos = p;

                    RenderPlot();
                }
                else if (_currentDragState == DragMode.MarqueeZoom)
                {
                    Point pointDragged = e.GetPosition(this);

                    using (DrawingContext dc = _selectedRegionVisual.RenderOpen())
                    {
                        dc.DrawRectangle(SelectedRegionBrush, SelectedRegionBorderPen,
                            new Rect(_selectedRegionPoint1, pointDragged));
                    }
                }
            }
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            double deltaZoom = e.Delta / 100.0;
            Zooming(deltaZoom);
        }

        protected override void OnRender(DrawingContext dc)
        {
            RenderPlot();
        }

        private static void OnExecutedUndo(object target, ExecutedRoutedEventArgs args)
        {
            ((Plot2D)target).UndoZoomPan();
        }

        private static void OnCanExecuteUndo(object target, CanExecuteRoutedEventArgs args)
        {
            args.CanExecute = ((Plot2D)target).CanUndo;
        }

        #endregion

        #region Rendering

        protected void RenderPlot()
        {
            double w = Math.Round(ActualWidth);
            double h = Math.Round(ActualHeight);

            double mx = Math.Round(w / 2.0);
            double my = Math.Round(h / 2.0);

            if (w <= borderSize || h <= borderSize)
                return;

            using (DrawingContext dc = _drawingVisual.RenderOpen())
            {
                // Clear plot 2d
                dc.DrawRectangle(Background, null, new Rect(0, 0, w, h));


                double xMinView = (borderSize - mx - _horizOffset) / _dx;
                double xMaxView = (-borderSize + mx - _horizOffset) / _dx;

                double yMinView = (borderSize - my + _vertOffset) / _dy;
                double yMaxView = (-borderSize + my + _vertOffset) / _dy;

                double xMinViewScaled = xMinView * _xAxisScaleInterval;
                double xMaxViewScaled = xMaxView * _xAxisScaleInterval;

                double yMinViewScaled = yMinView * _yAxisScaleInterval;
                double yMaxViewScaled = yMaxView * _yAxisScaleInterval;

                //------------------------------------------------------------
                // Render grid
                //------------------------------------------------------------
                Pen gridPen = GridPen;

                // Vertical grid lines
                int visibleVertGridLines = (int)(Math.Floor(xMaxView) - Math.Ceiling(xMinView)) + 1;
                double firstVisibleVertLine = borderSize + (Math.Ceiling(xMinView) - xMinView) * _dx;

                if (VertGridLinesVisible)
                {
                    for (int i = 0; i < visibleVertGridLines; i++)
                    {
                        double x = firstVisibleVertLine + i * _dx;
                        dc.DrawLine(gridPen, new Point(x, borderSize), new Point(x, h - borderSize));
                    }
                }

                // Horizontal grid lines
                int visibleHorizGridLines = (int)(Math.Floor(yMaxView) - Math.Ceiling(yMinView)) + 1;
                double firstVisibleHorizLine = borderSize + (yMaxView - Math.Floor(yMaxView)) * _dy;

                if (HorizGridLinesVisible)
                {
                    for (int i = 0; i < visibleHorizGridLines; i++)
                    {
                        double y = firstVisibleHorizLine + i * _dy;
                        dc.DrawLine(gridPen, new Point(borderSize, y), new Point(w - borderSize, y));
                    }
                }
                //------------------------------------------------------------

                //------------------------------------------------------------
                // Render rule
                //------------------------------------------------------------
                Pen borderPen = BorderPen;

                double borderThickness = borderPen.Thickness;

                if (!VertGridLinesVisible)
                {
                    for (int i = 0; i < visibleVertGridLines; i++)
                    {
                        double x = firstVisibleVertLine + i * _dx;

                        dc.DrawLine(borderPen, new Point(x, borderSize + borderThickness), new Point(x, borderSize + 7));
                        dc.DrawLine(borderPen, new Point(x, h - borderSize - borderThickness), new Point(x, h - (borderSize + 7)));
                    }
                }

                if (!HorizGridLinesVisible)
                {
                    for (int i = 0; i < visibleHorizGridLines; i++)
                    {
                        double y = firstVisibleHorizLine + i * _dy;

                        dc.DrawLine(borderPen, new Point(borderSize + borderThickness, y), new Point(borderSize + 7, y));
                        dc.DrawLine(borderPen, new Point(w - borderSize - borderThickness, y), new Point(w - (borderSize + 7), y));
                    }
                }

                //------------------------------------------------------------

                //------------------------------------------------------------
                // Render axes
                //------------------------------------------------------------
                if (XAxisVisible && (((my + _vertOffset) > borderSize) && ((my + _vertOffset) < h - borderSize)))
                {
                    dc.DrawLine(AxisPen, new Point(borderSize, my + _vertOffset), new Point(w - borderSize, my + _vertOffset));
                }

                if (YAxisVisible && (((mx + _horizOffset) > borderSize) && ((mx + _horizOffset) < w - borderSize)))
                {
                    dc.DrawLine(AxisPen, new Point(mx + _horizOffset, borderSize), new Point(mx + _horizOffset, h - borderSize));
                }
                //------------------------------------------------------------

                //------------------------------------------------------------
                // Render traces
                //------------------------------------------------------------
                if (_currentDragState == null)
                {
                    double x_step = _xAxisScaleInterval / _dx;
                    double inv_x_step = _dx / _xAxisScaleInterval;
                    double inv_y_step = _dy / _yAxisScaleInterval;

                    for (int idx = 0; idx < _traces.Count; idx++)
                    {
                        SimpleTrace trace = _traces[idx];

                        if (trace.Visible == false || trace.Function == null)
                            continue;

                        double x_min = trace.LowerBound;
                        double x_max = trace.UpperBound;
                        double yTemp;

                        double x1, x2, y1, y2;
                        x1 = x2 = y1 = y2 = 0.0;

                        Point p1 = new Point();
                        Point p2 = new Point();


                        StreamGeometry geometry = new StreamGeometry();
                        StreamGeometryContext sgc = geometry.Open();

                        if (xMinViewScaled >= x_min)
                            x_min = xMinViewScaled;

                        if (xMaxViewScaled <= x_max)
                            x_max = xMaxViewScaled;

                        int points = (int)Math.Round((x_max - x_min) / x_step);

                        Func<double, double> function = trace.Function;
                        yTemp = function(x_min);

                        for (int i = 0; i < points; i++)
                        {
                            x1 = x_min + i * x_step;
                            x2 = x1 + x_step;

                            y1 = yTemp;
                            y2 = function(x2);
                            yTemp = y2;

                            if (Math.Abs(y1 - y2) > 70.0 * _xAxisScaleInterval) continue;
                            //if (Math.Abs(x1 - x2) > 70.0) continue;
                            if (double.IsNaN(y1) || double.IsNaN(y2)) continue;
                            //if (double.IsNaN(x1) || double.IsNaN(x2)) continue;

                            if ((y1 >= yMaxViewScaled) && (y2 >= yMaxViewScaled)) continue;
                            if ((y1 <= yMinViewScaled) && (y2 <= yMinViewScaled)) continue;

                            p1.X = x1 * inv_x_step + mx + _horizOffset;
                            p1.Y = -y1 * inv_y_step + my + _vertOffset;
                            p2.X = x2 * inv_x_step + mx + _horizOffset;
                            p2.Y = -y2 * inv_y_step + my + _vertOffset;

                            sgc.BeginFigure(p1, false, false);
                            sgc.LineTo(p2, true, false);
                        }

                        sgc.Close();
                        dc.DrawGeometry(null, trace.Pen, geometry);
                    }
                }
                //------------------------------------------------------------

                //------------------------------------------------------------
                // Clear borders
                //------------------------------------------------------------
                dc.DrawRectangle(Background, null, new Rect(0, 0, borderSize, h)); // Left border
                dc.DrawRectangle(Background, null, new Rect(borderSize, 0, w - borderSize, borderSize)); // Top border
                dc.DrawRectangle(Background, null, new Rect(borderSize, h - borderSize, w - borderSize, borderSize)); // Bottom border
                dc.DrawRectangle(Background, null, new Rect(w - borderSize, borderSize, borderSize, h - borderSize)); // Right border

                //------------------------------------------------------------
                // Render borders
                //------------------------------------------------------------
                dc.DrawRectangle(null, borderPen, new Rect(borderSize, borderSize, w - borderSize * 2, h - borderSize * 2));
                //------------------------------------------------------------

                //------------------------------------------------------------
                // Render text
                //------------------------------------------------------------
                Brush foreground = Foreground;

                // Vertical
                double firstVert = Math.Ceiling(xMinView);

                for (int i = 0; i < visibleVertGridLines; i++)
                {
                    string text = ((firstVert + i) * _xAxisScaleInterval).ToString(_numericFormat);
                    FormattedText formattedText = new FormattedText(text, CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight, _typeface, _fontSize, foreground);

                    Point loc = new Point(firstVisibleVertLine + i * _dx - formattedText.Width / 2, h - borderSize);

                    dc.DrawText(formattedText, loc);
                }

                // Horizontal
                double firstHoriz = Math.Floor(yMaxView);

                for (int i = 0; i < visibleHorizGridLines; i++)
                {
                    string text = ((firstHoriz - i) * _yAxisScaleInterval).ToString(_numericFormat);
                    FormattedText formattedText = new FormattedText(text, CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight, _typeface, _fontSize, foreground);

                    Point loc = new Point(Math.Max(borderSize - 2 - formattedText.Width, 2), firstVisibleHorizLine + i * _dy - formattedText.Height / 2);

                    dc.DrawText(formattedText, loc);
                }
                //------------------------------------------------------------

                if (_currentDragState == DragMode.HandTool)
                {
                    FormattedText formattedText = new FormattedText(_splashText, CultureInfo.InvariantCulture,
                        FlowDirection.LeftToRight, _splashTypeface, _splashEmSize, _splashBrush);
                    dc.DrawText(formattedText, new Point(mx - formattedText.Width / 2, my - formattedText.Height / 2));
                }
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
                return _drawingVisual;
            else
                throw new ArgumentOutOfRangeException();
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return 1;
            }
        }

        #endregion

        #region Helpers

        public void SetTrace(Func<double, double> function)
        {
            if (_traces.Count == 0)
                _traces.Add(new SimpleTrace(function));
            else
                _traces[0] = new SimpleTrace(function);

            RenderPlot();
        }

        public Point PointToPlot2DCoordinate(Point point)
        {
            Point coord = new Point();
            coord.X = (point.X - ActualWidth / 2.0 - _horizOffset) / _dx * _xAxisScaleInterval;
            coord.Y = (-point.Y + ActualHeight / 2.0 + _vertOffset) / _dy * _yAxisScaleInterval;

            return coord;
        }

        public Rect GetPlot2DViewport()
        {
            Point p1 = PointToPlot2DCoordinate(new Point(borderSize, borderSize));
            Point p2 = PointToPlot2DCoordinate(new Point(ActualWidth - borderSize, ActualHeight - borderSize));

            return new Rect(p1.X, p1.Y, p2.X - p1.X, p1.Y - p2.Y);
        }

        public void SetPlot2DViewport(Rect newViewport)
        {
            if (newViewport.Width == 0.0 || newViewport.Height == 0.0)
            {
                return;
            }

            Rect viewport = GetPlot2DViewport();

            _xAxisScaleInterval /= (viewport.Width / newViewport.Width);
            _yAxisScaleInterval /= (viewport.Height / newViewport.Height);
            _dx = DefaultStepX;
            _dy = DefaultStepY;

            _horizOffset = borderSize - ActualWidth / 2 - (newViewport.X * _dx) / _xAxisScaleInterval;
            _vertOffset = borderSize - ActualHeight / 2 + (newViewport.Y * _dy) / _yAxisScaleInterval;

            RenderPlot();
        }

        public void UndoZoomPan()
        {
            if (_undoStack.Count != 0)
            {
                ViewportState viewportState = _undoStack.Pop();
                viewportState.SetViewport(this);
            }
            else
            {
                ResetViewport();
            }
        }

        public void ResetViewport()
        {
            _undoStack.Clear();

            _horizOffset = 0.0;
            _vertOffset = 0.0;

            _dx = DefaultStepX;
            _dy = DefaultStepY;
            _xAxisScaleInterval = DefaultXAxisScaleInterval;
            _yAxisScaleInterval = DefaultYAxisScaleInterval;

            RenderPlot();
        }

        public void Zooming(double deltaZoom)
        {
            // Coordinates of the center point of Plot2D
            double x = -_horizOffset / _dx * _xAxisScaleInterval;
            double y = _vertOffset / _dy * _yAxisScaleInterval;

            _dx += deltaZoom;
            _dy += deltaZoom;

            double factor = 2.0;

            if (_dx > DefaultStepX * factor)
            {
                _dx = DefaultStepX + _dx % (DefaultStepX * factor);
                _xAxisScaleInterval /= (factor);
            }
            else if (_dx < DefaultStepX / factor)
            {
                _dx = DefaultStepX - (DefaultStepX / factor) % _dx;
                _xAxisScaleInterval *= (factor);
            }
            
            if (_dy > DefaultStepY * factor)
            {
                _dy = DefaultStepY + _dy % (DefaultStepY * factor);
                _yAxisScaleInterval /= (factor);
            }
            else if (_dy < DefaultStepY / factor)
            {
                _dy = DefaultStepY - (DefaultStepY / factor) % _dy;
                _yAxisScaleInterval *= (factor);
            }

            _horizOffset = -x * _dx / _xAxisScaleInterval;
            _vertOffset = y * _dy / _yAxisScaleInterval;

            RenderPlot();
        }

        private static void Copy(Plot2D source, Plot2D destination, bool fullCopy)
        {
            destination._horizOffset = source._horizOffset;
            destination._vertOffset = source._vertOffset;
            destination._dx = source.StepX;
            destination._dy = source.StepY;
            destination._undoStack = source._undoStack;
            destination.VertGridLinesVisible = source.VertGridLinesVisible;
            destination.HorizGridLinesVisible = source.HorizGridLinesVisible;
            destination.XAxisVisible = source.XAxisVisible;
            destination.YAxisVisible = source.YAxisVisible;
            destination.XAxisScaleInterval = source.XAxisScaleInterval;
            destination.YAxisScaleInterval = source.YAxisScaleInterval;

            if (fullCopy)
            {
                destination.Background = source.Background;
                destination.Foreground = source.Foreground;
                destination.BorderPen = source.BorderPen;
                destination.GridPen = source.GridPen;
                destination.AxisPen = source.AxisPen;
                destination.SelectedRegionBrush = source.SelectedRegionBrush;
                destination.SelectedRegionBorderPen = source.SelectedRegionBorderPen;

                if (source._traces.Count != 0)
                    destination.SetTrace(source._traces[0].Function);
            }
        }

        private static ContextMenu CreateContextMenuForFullscreen(Plot2D plot2D, Window window)
        {
            ContextMenu contextMenu = new ContextMenu();

            MenuItem fullscreenMenuItem = new MenuItem();
            fullscreenMenuItem.Header = "Fullscreen";
            fullscreenMenuItem.IsCheckable = true;
            fullscreenMenuItem.IsChecked = true;
            fullscreenMenuItem.Click += new RoutedEventHandler(delegate(object o, RoutedEventArgs args) { plot2D.FullScreen = false; });

            MenuItem copyToClipboardMenuItem = new MenuItem();
            copyToClipboardMenuItem.Header = "Copy to clipboard";
            copyToClipboardMenuItem.Click += new RoutedEventHandler(delegate(object o, RoutedEventArgs args) { Snapshot.ToClipboard(plot2D); });

            MenuItem saveImageAsMenuItem = new MenuItem();
            saveImageAsMenuItem.Header = "Save image as...";
            saveImageAsMenuItem.Click += new RoutedEventHandler(delegate(object o, RoutedEventArgs args) { Snapshot.ToFile(plot2D, window); });

            MenuItem unZoomMenuItem = new MenuItem();
            unZoomMenuItem.Header = "Undo Zoom/Pan";
            unZoomMenuItem.Click += new RoutedEventHandler(delegate(object o, RoutedEventArgs args) { plot2D.UndoZoomPan(); });

            MenuItem undoAllZoomPanMenuItem = new MenuItem();
            undoAllZoomPanMenuItem.Header = "Reset Viewport";
            undoAllZoomPanMenuItem.Click += new RoutedEventHandler(delegate(object o, RoutedEventArgs args) { plot2D.ResetViewport(); });

            contextMenu.Items.Add(fullscreenMenuItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(copyToClipboardMenuItem);
            contextMenu.Items.Add(saveImageAsMenuItem);
            contextMenu.Items.Add(new Separator());
            contextMenu.Items.Add(unZoomMenuItem);
            contextMenu.Items.Add(undoAllZoomPanMenuItem);

            return contextMenu;
        }

        #endregion

        #endregion

        #region Nested types

        private enum DragMode
        {
            HandTool,
            MarqueeZoom
        }

        private struct ViewportState
        {
            #region Fields

            private double _horizOffset;

            private double _vertOffset;

            private double _dx;

            private double _dy;

            private double _xAxisScaleInterval;

            private double _yAxisScaleInterval;

            public static readonly ViewportState DefaultViewportState;

            #endregion

            #region Constructors

            static ViewportState()
            {
                DefaultViewportState = new ViewportState();
                DefaultViewportState._horizOffset = 0.0;
                DefaultViewportState._vertOffset = 0.0;
                DefaultViewportState._dx = DefaultStepX;
                DefaultViewportState._dy = DefaultStepY;
                DefaultViewportState._xAxisScaleInterval = DefaultXAxisScaleInterval;
                DefaultViewportState._yAxisScaleInterval = DefaultYAxisScaleInterval;
            }

            public ViewportState(Plot2D plot2D)
            {
                _horizOffset = plot2D._horizOffset;
                _vertOffset = plot2D._vertOffset;
                _dx = plot2D._dx;
                _dy = plot2D._dy;
                _xAxisScaleInterval = plot2D._xAxisScaleInterval;
                _yAxisScaleInterval = plot2D._yAxisScaleInterval;
            }

            #endregion

            #region Methods

            public void SetViewport(Plot2D plot2D)
            {
                plot2D._horizOffset = _horizOffset;
                plot2D._vertOffset = _vertOffset;
                plot2D._dx = _dx;
                plot2D._dy = _dy;
                plot2D._xAxisScaleInterval = _xAxisScaleInterval;
                plot2D._yAxisScaleInterval = _yAxisScaleInterval;

                plot2D.RenderPlot();
            }

            public override bool Equals(object obj)
            {
                if (obj is ViewportState)
                    return (this == (ViewportState)obj);
                else
                    return false;
            }

            public override int GetHashCode()
            {
                return _horizOffset.GetHashCode() ^ _vertOffset.GetHashCode() ^
                    _dx.GetHashCode() ^ _dy.GetHashCode() ^
                    _xAxisScaleInterval.GetHashCode() ^ _yAxisScaleInterval.GetHashCode();
            }

            #endregion

            #region Operations

            public static bool operator ==(ViewportState viewport1, ViewportState viewport2)
            {
                if (viewport1._horizOffset != viewport2._horizOffset)
                    return false;
                if (viewport1._vertOffset != viewport2._vertOffset)
                    return false;
                if (viewport1._dx != viewport2._dx)
                    return false;
                if (viewport1._dy != viewport2._dy)
                    return false;
                if (viewport1._xAxisScaleInterval != viewport2._xAxisScaleInterval)
                    return false;
                if (viewport1._yAxisScaleInterval != viewport2._yAxisScaleInterval)
                    return false;

                return true;
            }

            public static bool operator !=(ViewportState viewport1, ViewportState viewport2)
            {
                return !(viewport1 == viewport2);
            }

            #endregion
        }

        public class SimpleTrace
        {
            #region Fields

            private Pen _pen = new Pen(Brushes.Blue, 1);

            private double _lowerBound = double.NegativeInfinity;

            private double _upperBound = double.PositiveInfinity;

            private bool _visible = true;

            private string _title;

            private Func<double, double> _function;

            #endregion

            #region Properties

            public Pen Pen
            {
                get
                {
                    return _pen;
                }

                set
                {
                    _pen = value;
                }
            }

            public double LowerBound
            {
                get
                {
                    return _lowerBound;
                }

                set
                {
                    _lowerBound = value;
                }
            }

            public double UpperBound
            {
                get
                {
                    return _upperBound;
                }

                set
                {
                    _upperBound = value;
                }
            }

            public bool Visible
            {
                get
                {
                    return _visible;
                }

                set
                {
                    _visible = value;
                }
            }

            public string Title
            {
                get
                {
                    return _title;
                }

                set
                {
                    _title = value;
                }
            }

            public Func<double, double> Function
            {
                get
                {
                    return _function;
                }

                set
                {
                    _function = value;
                }
            }

            #endregion

            #region Constructors

            public SimpleTrace(Func<double, double> function)
            {
                _function = function;
            }

            public SimpleTrace(Func<double, double> function, Pen pen, double lowerBound, double upperBound)
            {
                _function = function;
                _pen = pen;
                _lowerBound = lowerBound;
                _upperBound = upperBound;
            }

            #endregion
        }

        #endregion
    }
}
