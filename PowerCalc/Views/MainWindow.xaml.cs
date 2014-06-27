using System;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
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

using TAlex.Common.Environment;
using TAlex.PowerCalc.Helpers;
using TAlex.PowerCalc.Commands;
using TAlex.PowerCalc.ViewModels;
using TAlex.PowerCalc.ViewModels.Plot2D;
using TAlex.WPF3DToolkit;
using TAlex.WPF3DToolkit.Surfaces;
using TAlex.PowerCalc.ViewModels.Worksheet;


namespace TAlex.PowerCalc.Views
{
    public partial class MainWindow : Window
    {        
        #region Constructors

        public MainWindow()
        {
            InitializeComponent();
            LoadSettings(true);

            ((INotifyCollectionChanged)WorksheetListView.Items).CollectionChanged += Worksheet_CollectionChanged;
            ((WorksheetModel)WorksheetListView.DataContext).InputHistoryNavigated += Worksheet_InputHistoryNavigated;
        }

        #endregion

        #region Methods

        private void LoadSettings(bool isStarting)
        {
            Properties.Settings settings = Properties.Settings.Default;

            try
            {
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

                plot2D.Background = new SolidColorBrush(settings.Plot2DBackground);
                plot2D.Foreground = new SolidColorBrush(settings.Plot2DForeground);
                plot2D.GridPen = new Pen(new SolidColorBrush(settings.Plot2DGridlinesColor), plot2D.GridPen.Thickness);
                plot2D.AxisPen = new Pen(new SolidColorBrush(settings.Plot2DAxisColor), plot2D.AxisPen.Thickness);
                plot2D.SelectedRegionBrush = new SolidColorBrush(settings.Plot2DSelectionRegionColor);

                plot2D.VertGridLinesVisible = settings.Plot2DVertGridlinesVisible;
                plot2D.HorizGridLinesVisible = settings.Plot2DHorizGridlinesVisible;
                plot2D.XAxisVisible = settings.Plot2DXAxisVisible;
                plot2D.YAxisVisible = settings.Plot2DYAxisVisible;
            }
            catch (Exception)
            {
                settings.Reset();
                settings.Save();
                LoadSettings(isStarting);
            }
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

        private void CommandBindingHelp_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            new ReferencesWindow { Owner = this }.Show();
        }

        #endregion

        #region Window

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 3D Plot
            viewPort3D.Children.Add(Utils3D.GetCubeRectCoordSystem(10, 2));

            if (Properties.Settings.Default.ShowHowToOnStartup)
            {
                new HowToWindow { Owner = this }.Show();
            }
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

        private void viewMenuItems_Click(object sender, RoutedEventArgs e)
        {
            mainTabControl.SelectedIndex = int.Parse((string)((Control)(sender)).Tag);
        }

        private void preferencesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowPreferencesDialog(0);
        }

        private void howToMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new HowToWindow { Owner = this }.Show();
        }

        private void navHomepageMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(PowerCalc.Properties.Resources.HomepageUrl);
        }

        private void registrationMenuItem_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow window = new RegistrationWindow { Owner = this };
            window.ShowDialog();
        }

        private void aboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow window = new AboutWindow(this);
            window.ShowDialog();
        }

        #endregion

        #region Worksheet

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

            new InsertFunctionCommand().Execute(new InsertFunctionParameter(text, GetWorksheetInputElement()));
        }

        private void Worksheet_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count == 1)
            {
                object lastItem = e.NewItems[e.NewItems.Count - 1];
                WorksheetListView.UpdateLayout();
                WorksheetListView.ScrollIntoView(lastItem);
                WorksheetListView.SelectedItem = lastItem;

                ItemContainerGenerator generator = WorksheetListView.ItemContainerGenerator;
                ListBoxItem selectedItem = (ListBoxItem)generator.ContainerFromItem(lastItem);
                selectedItem.Loaded += selectedItem_Loaded;
            }
        }

        private void Worksheet_InputHistoryNavigated(object sender, EventArgs e)
        {
            var textBox = GetWorksheetInputElement() as TextBox;
            if (textBox != null)
            {
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private void selectedItem_Loaded(object sender, RoutedEventArgs e)
        {
            ListBoxItem selectedItem = sender as ListBoxItem;
            IInputElement firstFocusable = VisualHelper.FindFirstFocusableElement(selectedItem);
            if (firstFocusable != null)
            {
                firstFocusable.Focus();
            }
        }

        private IInputElement GetWorksheetInputElement()
        {
            int itemIndex = WorksheetListView.Items.Count - 1;
            ItemContainerGenerator generator = WorksheetListView.ItemContainerGenerator;
            ListBoxItem lastItem = (ListBoxItem)generator.ContainerFromIndex(itemIndex);
            IInputElement firstFocusable = VisualHelper.FindFirstFocusableElement(lastItem);

            WorksheetListView.ScrollIntoView(WorksheetListView.Items[itemIndex]);
            firstFocusable.Focus();
            return firstFocusable;
        }

        #endregion

        #region Worksheet matrices

        private void insertTextToFormulaBar_Click(object sender, RoutedEventArgs e)
        {
            string functionHeader = (string)(((Control)sender).Tag);
            new InsertFunctionCommand().Execute(new InsertFunctionParameter(functionHeader, worksheetMatrix.FormulaBar));
        }

        private void evaluateMatricesButton_Click(object sender, RoutedEventArgs e)
        {
            worksheetMatrix.CommitEdit();
        }

        #endregion

        #region 2D Plot

        private void plot2D_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition((IInputElement)sender);
            point.X = Math.Round(point.X);
            point.Y = Math.Round(point.Y);

            Point coord = plot2D.PointToRoundedPlot2DCoordinate(point);

            MainWindowViewModel viewModel = DataContext as MainWindowViewModel;
            viewModel.CanShowXYCoords = true;
            viewModel.XCoord2dPlot = coord.X;
            viewModel.YCoord2dPlot = coord.Y;
        }

        private void plot2D_MouseLeave(object sender, MouseEventArgs e)
        {
            MainWindowViewModel viewModel = DataContext as MainWindowViewModel;
            viewModel.CanShowXYCoords = false;
        }


        private void addTrace2DButton_Click(object sender, RoutedEventArgs e)
        {
            plot2D.ShowAddTraceDialog();
        }

        private void showTraces2DButton_Click(object sender, RoutedEventArgs e)
        {
            plot2D.ShowEditTracesDialog();
        }

        #endregion

        #region 3D Plot

        private void trackball_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                Grid parent = plot3DGrid;

                if (trackball.Tag == null)
                {
                    trackball.Tag = true;

                    parent.Children.Remove(trackball);

                    Window fullwindow = new Window
                    {
                        Background = parent.Background,
                        ShowInTaskbar = false,
                        Topmost = false,
                        WindowStyle = WindowStyle.None,
                        WindowState = WindowState.Maximized,
                        Content = trackball
                    };
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

                // Refresh numeric values
                WorksheetListView.Items.Refresh();
                worksheetMatrix.Refresh();
            }
        }

        private void Plot3DRender(string expr)
        {
            //if (expr.Length > 0)
            //{
            //    try
            //    {
            //        Expression<Object> expression = ExpressionTreeBuilder.BuildTree(expr);
            //        Func<Object, Object, Object> f = ParametricFunctionCreator.CreateTwoParametricFunction(expression, "x", "y");

            //        Func<double, double, double> func = (x, y) => { var res = (Complex)f((Complex)x, (Complex)y); return res.IsReal ? res.Re : double.NaN; };
            //        surface.Geometry = new SimpleSurface(func).BuildGeometry();
            //    }
            //    catch (Exception exc)
            //    {
            //        MessageBox.Show(this, exc.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}
            //else
            //{
            //    surface.Geometry = null;
            //}
        }

        #endregion
    }
}