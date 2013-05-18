using System;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using Microsoft.Win32;

using TAlex.PowerCalc.Helpers;

using TAlex.MathCore;
using TAlex.MathCore.ExpressionEvaluation;
using TAlex.MathCore.LinearAlgebra;
using TAlex.WPF3DToolkit;
using TAlex.WPF3DToolkit.Surfaces;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;
using TAlex.MathCore.ExpressionEvaluation.Trees.Metadata;
using TAlex.MathCore.ExpressionEvaluation.Trees;
using TAlex.Common.Environment;
using TAlex.PowerCalc.Commands;


namespace TAlex.PowerCalc
{
    public partial class MainWindow : Window
    {
        private IExpressionTreeBuilder<Object> ExpressionTreeBuilder;


        #region Constructors

        public MainWindow()
        {
            InitializeComponent();

            LoadSettings(true);

            string productTitle = ApplicationInfo.Title;

            if (Licensing.License.IsTrial)
                Title = String.Format("{0} - Evaluation version (days left: {1})", productTitle, Licensing.License.TrialDaysLeft);
            else
                Title = productTitle;

            aboutMenuItem.Header = "_About " + productTitle;

            InitEvaluator();
        }

        #endregion

        #region Methods

        private void InitEvaluator()
        {
            ConstantFlyweightFactory<Object> constantFactory = new ConstantFlyweightFactory<Object>();
            constantFactory.AddFromAssemblies(GetAssembliesFromPath(Properties.Settings.Default.ExtensionsPath));


            FunctionFactory<Object> functionFactory = new FunctionFactory<Object>();
            functionFactory.AddFromAssemblies(GetAssembliesFromPath(Properties.Settings.Default.ExtensionsPath));

            ExpressionTreeBuilder = new ComplexExpressionTreeBuilder
            {
                ConstantFactory = constantFactory,
                FunctionFactory = functionFactory
            };
        }

        private static IEnumerable<Assembly> GetAssembliesFromPath(string path)
        {
            string[] files = Directory.GetFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path), "*.dll", SearchOption.TopDirectoryOnly);

            foreach (string filePath in files)
            {
                yield return Assembly.LoadFile(filePath);
            }
        }


        private void LoadSettings(bool isStarting)
        {
            Properties.Settings settings = Properties.Settings.Default;

            if (isStarting)
            {
                WindowState = (settings.WindowState == System.Windows.WindowState.Minimized) ? System.Windows.WindowState.Normal : settings.WindowState;

                Rect windowBounds = settings.WindowBounds;
                if (!(windowBounds.Width == 0 || windowBounds.Height == 0))
                {
                    WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
                    Left = windowBounds.Left;
                    Top = windowBounds.Top;
                    Width = windowBounds.Width;
                    Height = windowBounds.Height;
                }
            }

            worksheetTextBox.FontFamily = settings.WorksheetFontFamily;
            worksheetTextBox.FontWeight = settings.WorksheetFontWeight;
            worksheetTextBox.FontStyle = settings.WorksheetFontStyle;
            worksheetTextBox.FontStretch = settings.WorksheetFontStretch;
            worksheetTextBox.FontSize = settings.WorksheetFontSize;
            worksheetTextBox.Foreground = new SolidColorBrush(settings.WorksheetForeground);

            plot2D.Background = new SolidColorBrush(settings.Plot2DBackground);
        }

        private void SaveSettings()
        {
            Properties.Settings settings = Properties.Settings.Default;

            settings.WindowState = WindowState;
            settings.WindowBounds = RestoreBounds;

            settings.Save();
        }

        #region Event Handlers

        #region Command Bindings

        private void CommandBindingNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(worksheetTextBox.Text))
            {
                return;
            }

            if (MessageBox.Show(this, "", "Question", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                worksheetTextBox.Text = String.Empty;
            }
        }

        private void CommandBindingOpen_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PowerCalc Worksheet (*.pcx)|*.pcx|All Files (*.*)|*.*";

            if (ofd.ShowDialog(this) == true)
            {
                worksheetTextBox.Text = System.IO.File.ReadAllText(ofd.FileName);
            }
        }

        private void CommandBindingSaveAs_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PowerCalc Worksheet (*.pcx)|*.pcx|All Files (*.*)|*.*";

            if (sfd.ShowDialog(this) == true)
            {
                System.IO.File.WriteAllText(sfd.FileName, worksheetTextBox.Text);
            }
        }

        private void CommandBindingHelp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string helpFileName = Properties.Resources.HelpFileName;

            if (System.IO.File.Exists(helpFileName))
            {
                Process.Start(helpFileName);
            }
        }

        #endregion

        #region Window

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Worksheet
            worksheetTextBox.Focus();

            // 3D Plot
            viewPort3D.Children.Add(Utils3D.GetCubeRectCoordSystem(10, 2));
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }

        #endregion

        #region Menu

        private void exitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void viewMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;

            int selectedTabItem = mainTabControl.SelectedIndex;

            for (int i = 0; i < 4; i++)
            {
                MenuItem item = menuItem.Items[i] as MenuItem;

                if (i == selectedTabItem)
                    item.IsChecked = true;
                else
                    item.IsChecked = false;
            }
        }

        private void colorSchemeMenuItem_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            string currentColorScheme = PowerCalc.Properties.Settings.Default.ColorScheme;

            for (int i = 0; i < menuItem.Items.Count; i++)
            {
                MenuItem item = menuItem.Items[i] as MenuItem;

                if ((string)item.Tag == currentColorScheme)
                    item.IsChecked = true;
                else
                    item.IsChecked = false;
            }
        }

        private void viewMenuItems_Click(object sender, RoutedEventArgs e)
        {
            mainTabControl.SelectedIndex = int.Parse((string)((Control)(sender)).Tag);
        }

        private void applyColorSchemeMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            string colorScheme = (string)menuItem.Tag;

            int selectedIndex = mainTabControl.SelectedIndex;

            if (TAlex.WPFThemes.Twilight.TwilightThemeManager.ApplyTheme(colorScheme))
            {
                PowerCalc.Properties.Settings.Default.ColorScheme = colorScheme;
                PowerCalc.Properties.Settings.Default.Save();

                mainTabControl.SelectedIndex = selectedIndex;
            }
        }

        private void preferencesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowPreferencesDialog(0);
        }

        private void navHomepageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(PowerCalc.Properties.Resources.HomepageUrl);
        }

        private void registrationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow window = new RegistrationWindow();
            window.Owner = this;

            window.ShowDialog();
        }

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow window = new AboutWindow(this);
            window.ShowDialog();
        }

        #endregion

        #region Worksheet

        private void worksheetTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            int caretIndex = worksheetTextBox.CaretIndex;
            int currentLineIndex = worksheetTextBox.GetLineIndexFromCharacterIndex(caretIndex);
            int currentLineLength = worksheetTextBox.GetLineLength(currentLineIndex);

            if (worksheetTextBox.GetLineText(currentLineIndex).EndsWith(Environment.NewLine))
                currentLineLength -= Environment.NewLine.Length;

            int currentLineCharacterIndex = worksheetTextBox.GetCharacterIndexFromLineIndex(currentLineIndex);


            switch (e.Text)
            {
                case "=":
                    string currentLineString = worksheetTextBox.GetLineText(currentLineIndex).Trim();

                    if (String.IsNullOrEmpty(currentLineString))
                    {
                        e.Handled = true;
                        return;
                    }

                    string[] lines = new string[currentLineIndex];
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lines[i] = worksheetTextBox.GetLineText(i).Trim();
                    }

                    Dictionary<string, Object> variables = Helpers.WorksheetHelper.CalculateVariables(ExpressionTreeBuilder, lines);


                    System.Text.RegularExpressions.Match match = Helpers.WorksheetHelper.GetMatch(currentLineString);

                    string resultString = currentLineString;
                    
                    try
                    {
                        if (!match.Success)
                            throw new FormatException();

                        resultString = match.Value.Trim();
                        string expr = match.Groups["expr"].Value;

                        
                        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                        watch.Start();

                        // Evaluation expression
                        Expression<Object> expression = ExpressionTreeBuilder.BuildTree(expr);
                        foreach (var var in expression.FindAllVariables())
                        {
                            object value;
                            if (variables.TryGetValue(var.VariableName, out value))
                            {
                                var.Value = value;
                            }
                        }

                        Object result = expression.Evaluate();

                        watch.Stop();

                        if (result is IFormattable)
                        {
                            if (result is Complex) result = NumericUtil.ComplexZeroThreshold((Complex)result, Properties.Settings.Default.ComplexThreshold, Properties.Settings.Default.ZeroThreshold);
                            if (result is CMatrix) result = TAlex.MathCore.LinearAlgebra.NumericUtilExtensions.ComplexZeroThreshold((CMatrix)result, Properties.Settings.Default.ComplexThreshold, Properties.Settings.Default.ZeroThreshold);
                            resultString += String.Format(" = {0}", ((IFormattable)result).ToString(Properties.Settings.Default.NumericFormat, CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            resultString += String.Format(" = {0}", result);
                        }
                    }
                    catch (FormatException exc)
                    {
                        resultString += String.Format(" = syntax_error ({0})", exc.Message);
                    }
                    catch (Exception exc)
                    {
                        resultString += String.Format(" = calc_error ({0})", exc.Message);
                    }

                    StringBuilder sb = new StringBuilder(worksheetTextBox.Text);

                    sb.Remove(currentLineCharacterIndex, currentLineLength);
                    sb.Insert(currentLineCharacterIndex, resultString);

                    worksheetTextBox.Text = sb.ToString();
                    worksheetTextBox.CaretIndex = currentLineCharacterIndex + resultString.Length;

                    e.Handled = true;
                    break;


                default:
                    string currentFullLineString = worksheetTextBox.GetLineText(currentLineIndex);

                    string secondPart = String.Empty;
                    if (!String.IsNullOrEmpty(currentFullLineString))
                        secondPart = currentFullLineString.Substring(0, caretIndex - currentLineCharacterIndex);

                    if (secondPart.Contains("="))
                    {
                        e.Handled = true;
                    }

                    break;
            }
        }

        private void worksheetTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            int caretIndex = worksheetTextBox.CaretIndex;
            int currentLineIndex = worksheetTextBox.GetLineIndexFromCharacterIndex(caretIndex);
            int currentLineLength = worksheetTextBox.GetLineLength(currentLineIndex);

            if (worksheetTextBox.GetLineText(currentLineIndex).EndsWith(Environment.NewLine))
                currentLineLength -= Environment.NewLine.Length;

            int currentLineCharacterIndex = worksheetTextBox.GetCharacterIndexFromLineIndex(currentLineIndex);

            string currentLineString = worksheetTextBox.GetLineText(currentLineIndex);

            string secondPart = String.Empty;
            if (!String.IsNullOrEmpty(currentLineString))
                secondPart = currentLineString.Substring(0, caretIndex - currentLineCharacterIndex);

            if (secondPart.Contains("="))
            {
                if (e.Key == Key.Back || e.Key == Key.Delete)
                {
                    StringBuilder sb = new StringBuilder(worksheetTextBox.Text);

                    string result = currentLineString.Split('=')[0].TrimEnd();

                    sb.Remove(currentLineCharacterIndex, currentLineLength);
                    sb.Insert(currentLineCharacterIndex, result);

                    worksheetTextBox.Text = sb.ToString();
                    worksheetTextBox.CaretIndex = currentLineCharacterIndex + result.Length;

                    e.Handled = true;
                }
            }
        }

        private void worksheetTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            int caretIndex = worksheetTextBox.CaretIndex;
            int currentLineIndex = worksheetTextBox.GetLineIndexFromCharacterIndex(caretIndex);
            int currentLineLength = worksheetTextBox.GetLineLength(currentLineIndex);

            if (worksheetTextBox.GetLineText(currentLineIndex).EndsWith(Environment.NewLine))
                currentLineLength -= Environment.NewLine.Length;

            int currentLineCharacterIndex = worksheetTextBox.GetCharacterIndexFromLineIndex(currentLineIndex);


            string currentFullLineString = worksheetTextBox.GetLineText(currentLineIndex);

            string secondPart = String.Empty;
            if (!String.IsNullOrEmpty(currentFullLineString))
                secondPart = currentFullLineString.Substring(0, caretIndex - currentLineCharacterIndex);

            if (secondPart.Contains("="))
            {
                e.CancelCommand();
                e.Handled = true;
            }
        }

        private void buttonInsertText_Click(object sender, RoutedEventArgs e)
        {
            string text = (string)(((Control)sender).Tag);
            if (text.Length > 2 && text.Substring(text.Length - 2) == "()")
            {
                switch (text.Substring(0, text.Length - 2))
                {
                    case "sin":
                    case "cos":
                    case "tan":
                    case "cot":
                    case "sec":
                    case "csc":
                        if (invCheckBox.IsChecked == true) text = text.Insert(0, "a");
                        if (hypCheckBox.IsChecked == true) text = text.Insert(text.Length - 2, "h");
                        break;
                }
            }

            new InsertFunctionCommand().Execute(new InsertFunctionParameter(text, worksheetTextBox));
        }

        private void buttonEvaluate_Click(object sender, RoutedEventArgs e)
        {
            TextCompositionEventArgs args =
                new TextCompositionEventArgs(null, new TextComposition(null, worksheetTextBox, "="));
            args.RoutedEvent = e.RoutedEvent;
            worksheetTextBox_PreviewTextInput(worksheetTextBox, args);
            worksheetTextBox.Focus();
        }

        #endregion

        #region Worksheet matrices

        private void insertTextToFormulaBar_Click(object sender, RoutedEventArgs e)
        {
            worksheetMatrix.FormulaBar.Focus();
            string functionHeader = (string)(((Control)sender).Tag);
            new InsertFunctionCommand().Execute(new InsertFunctionParameter(functionHeader, worksheetMatrix.FormulaBar));
        }

        private void evaluateMatricesButton_Click(object sender, RoutedEventArgs e)
        {
            //worksheetMatrix.CommitEdit();
        }

        #endregion

        #region 2D Plot

        private void plot2D_MouseMove(object sender, MouseEventArgs e)
        {
            xyCoordTitleStatusBarItem.Visibility = Visibility.Visible;
            xyCoordStatusBarItem.Visibility = Visibility.Visible;

            Point point = e.GetPosition((IInputElement)sender);
            point.X = Math.Round(point.X);
            point.Y = Math.Round(point.Y);

            Point coord = plot2D.PointToPlot2DCoordinate(point);
            xyCoordStatusBarItem.Content = String.Format(CultureInfo.InvariantCulture, "{0:G14}, {1:G14}", coord.X, coord.Y);
        }

        private void plot2D_MouseLeave(object sender, MouseEventArgs e)
        {
            xyCoordTitleStatusBarItem.Visibility = Visibility.Collapsed;
            xyCoordStatusBarItem.Visibility = Visibility.Collapsed;
        }

        private void buttonPlot2DDraw_Click(object sender, RoutedEventArgs e)
        {
            Plot2DDraw(textBoxPlot2D.Text);
        }

        private void textBoxPlot2D_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Plot2DDraw(textBoxPlot2D.Text);
            }
        }


        private void fullscreenPlot2DMenuItem_Click(object sender, RoutedEventArgs e)
        {
            plot2D.FullScreen = true;
        }

        private void copyPlotMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Snapshot.ToClipboard(plot2D);
        }

        private void saveAsPlotMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Snapshot.ToFile(plot2D, this);
        }

        private void undoZoomPanMenuItem_Click(object sender, RoutedEventArgs e)
        {
            plot2D.UndoZoomPan();
        }

        private void resetViewportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            plot2D.ResetViewport();
        }

        private void plot2DPropertiesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowPreferencesDialog(3);
        }

        #endregion

        #region 3D Plot

        private void surfaceRenderButton_Click(object sender, RoutedEventArgs e)
        {
            Plot3DRender(textBoxSurface.Text);
        }

        private void textBoxSurface_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Plot3DRender(textBoxSurface.Text);
            }
        }

        private void trackball_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Grid parent = plot3DGrid;

                if (trackball.Tag == null)
                {
                    trackball.Tag = true;

                    parent.Children.Remove(trackball);

                    Window fullwindow = new Window();
                    fullwindow.Background = parent.Background;
                    fullwindow.ShowInTaskbar = false;
                    fullwindow.Topmost = false;
                    fullwindow.WindowStyle = WindowStyle.None;
                    fullwindow.WindowState = WindowState.Maximized;
                    fullwindow.Content = trackball;
                    fullwindow.ShowDialog();
                }
                else
                {
                    trackball.Tag = null;
                    Window window = trackball.Parent as Window;
                    window.Content = null;
                    window.Close();

                    parent.Children.Add(trackball);
                }
            }
        }


        private void copyPlot3DMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Snapshot.ToClipboard(plot3DGrid);
        }

        private void saveAsPlot3DMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Snapshot.ToFile(plot3DGrid, this);
        }

        private void plot3DPropertiesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowPreferencesDialog(4);
        }

        #endregion

        #endregion

        private void ShowPreferencesDialog(int tabindex)
        {
            PreferencesWindow window = new PreferencesWindow(tabindex);
            window.Owner = this;

            if (window.ShowDialog() == true)
            {
                LoadSettings(false);
            }
        }

        private void Plot2DDraw(string expr)
        {
            if (expr.Length > 0)
            {
                try
                {
                    Expression<Object> expression = ExpressionTreeBuilder.BuildTree(expr);

                    Func<Object, Object> f = ParametricFunctionCreator.CreateOneParametricFunction(expression, "x");
                    Func<double, double> func = (x) => { Complex res = (Complex)f((Complex)x); return res.IsReal ? res.Re : double.NaN; };
                    func(0.0);
                    plot2D.SetTrace(func);
                }
                catch (Exception exc)
                {
                    MessageBox.Show(this, exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            else
            {
                plot2D.SetTrace(null);
            }
        }

        private void Plot3DRender(string expr)
        {
            if (expr.Length > 0)
            {
                try
                {
                    Expression<Object> expression = ExpressionTreeBuilder.BuildTree(expr);
                    Func<Object, Object, Object> f = ParametricFunctionCreator.CreateTwoParametricFunction(expression, "x", "y");

                    Func<double, double, double> func = (x, y) => { var res = (Complex)f((Complex)x, (Complex)y); return res.IsReal ? res.Re : double.NaN; };
                    surface.Geometry = new SimpleSurface(func).BuildGeometry();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(this, exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                surface.Geometry = null;
            }
        }

        #endregion
    }
}