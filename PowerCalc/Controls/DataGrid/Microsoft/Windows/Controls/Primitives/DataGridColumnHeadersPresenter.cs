//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using MS.Internal;

namespace Microsoft.Windows.Controls.Primitives
{
    /// <summary>
    /// A control that will be responsible for generating column headers.
    /// This control is meant to be specified within the template of the DataGrid.
    ///     
    /// It typically isn't in the subtree of the main ScrollViewer for the DataGrid. 
    /// It thus handles scrolling the column headers horizontally.  For this to work
    /// it needs to be able to find the ScrollViewer -- this is done by setting the 
    /// SourceScrollViewerName property.
    /// </summary>
    public class DataGridColumnHeadersPresenter : ItemsControl
    {
        #region Fields

        private ContainerTracking<DataGridColumnHeader> _headerTrackingRoot;

        private DataGrid _parentDataGrid = null;

        private Panel _internalItemsHost;

        #endregion 

        #region Constructors

        static DataGridColumnHeadersPresenter()
        {
            Type ownerType = typeof(DataGridColumnHeadersPresenter);

            DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
            FocusableProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(false));

            FrameworkElementFactory factory = new FrameworkElementFactory(typeof(DataGridCellsPanel));
            ItemsPanelProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(new ItemsPanelTemplate(factory)));

            VirtualizingStackPanel.IsVirtualizingProperty.OverrideMetadata(
                ownerType, 
                new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsVirtualizingPropertyChanged), new CoerceValueCallback(OnCoerceIsVirtualizingProperty)));

            VirtualizingStackPanel.VirtualizationModeProperty.OverrideMetadata(
                ownerType, 
                new FrameworkPropertyMetadata(VirtualizationMode.Recycling));
        }

        #endregion

        #region Methods

        #region Initialization

        /// <summary>
        /// Tells the row owner about this element.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // Find the columns collection and set the ItemsSource.
            DataGrid grid = ParentDataGrid;

            if (grid != null)
            {
                ItemsSource = new ColumnHeaderCollection(grid.Columns);
                grid.ColumnHeadersPresenter = this;
                DataGridHelper.TransferProperty(this, VirtualizingStackPanel.IsVirtualizingProperty);
            }
            else
            {
                ItemsSource = null;
            }
        }

        #endregion

        #region Layout

        /// <summary>
        /// Measure
        /// </summary>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size desiredSize;
            Size childConstraint = availableSize;
            childConstraint.Width = Double.PositiveInfinity;

            desiredSize = base.MeasureOverride(childConstraint);

            desiredSize.Width = Math.Min(availableSize.Width, desiredSize.Width);

            return desiredSize;
        }

        /// <summary>
        /// Arrange
        /// </summary>
        /// <param name="finalSize">Arrange size</param>
        protected override Size ArrangeOverride(Size finalSize)
        {
            UIElement child = (VisualTreeHelper.GetChildrenCount(this) > 0) ? VisualTreeHelper.GetChild(this, 0) as UIElement : null;

            if (child != null)
            {
                Rect childRect = new Rect(finalSize);
                DataGrid dataGrid = ParentDataGrid;
                if (dataGrid != null)
                {
                    childRect.X = -dataGrid.HorizontalScrollOffset;
                    childRect.Width = Math.Max(finalSize.Width, dataGrid.CellsPanelActualWidth);
                }

                child.Arrange(childRect);
            }

            return finalSize;
        }

        /// <summary>
        /// Override of UIElement.GetLayoutClip().  This is a tricky way to ensure we always clip regardless of the value of ClipToBounds.
        /// </summary>
        protected override Geometry GetLayoutClip(Size layoutSlotSize)
        {
            RectangleGeometry clip = new RectangleGeometry(new Rect(RenderSize));
            clip.Freeze();
            return clip;
        }

        #endregion

        #region Column Header Generation

        /// <summary>
        /// Instantiates an instance of a container.
        /// </summary>
        /// <returns>A new DataGridColumnHeader.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DataGridColumnHeader();
        }

        /// <summary>
        /// Determines if an item is its own container.
        /// </summary>
        /// <param name="item">The item to test.</param>
        /// <returns>true if the item is a DataGridColumnHeader, false otherwise.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DataGridColumnHeader;
        }

        /// <summary>
        /// Method which returns the result of IsItemItsOwnContainerOverride to be used internally
        /// </summary>
        internal bool IsItemItsOwnContainerInternal(object item)
        {
            return IsItemItsOwnContainerOverride(item);
        }

        /// <summary>
        /// Prepares a new container for a given item.
        /// </summary>
        /// <remarks>We do not want to call base.PrepareContainerForItemOverride in this override because it will set local values on the header</remarks>
        /// <param name="element">The new container.</param>
        /// <param name="item">The item that the container represents.</param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            DataGridColumnHeader header = element as DataGridColumnHeader;

            if (header != null)
            {
                DataGridColumn column = ColumnFromContainer(header);
                Debug.Assert(column != null, "We shouldn't have generated this column header if we don't have a column.");

                if (header.Column == null)
                {
                    // A null column means this is a fresh container.  PrepareContainer will also be called simply if the column's
                    // Header property has changed and this container needs to be given a new item.  In that case it'll already be tracked.
                    header.Tracker.Debug_AssertNotInList(_headerTrackingRoot);
                    header.Tracker.StartTracking(ref _headerTrackingRoot);
                }

                header.Tracker.Debug_AssertIsInList(_headerTrackingRoot);

                header.PrepareColumnHeader(item, column);
            }
        }

        /// <summary>
        /// Clears a container of references.
        /// </summary>
        /// <param name="element">The container being cleared.</param>
        /// <param name="item">The data item that the container represented.</param>
        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            DataGridColumnHeader header = element as DataGridColumnHeader;

            base.ClearContainerForItemOverride(element, item);

            if (header != null)
            {
                header.Tracker.StopTracking(ref _headerTrackingRoot);
                header.ClearHeader();
            }
        }

        private DataGridColumn ColumnFromContainer(DataGridColumnHeader container)
        {
            Debug.Assert(HeaderCollection != null, "This is a helper method for preparing and clearing a container; if it's called we must have a valid ItemSource");
         
            int index = ItemContainerGenerator.IndexFromContainer(container);    
            return HeaderCollection.ColumnFromIndex(index);
        }

        #endregion

        #region Notification Propagation

        /// <summary>
        /// General notification for DependencyProperty changes from the grid.
        /// </summary>
        internal void NotifyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e, NotificationTarget target)
        {
            NotifyPropertyChanged(d, string.Empty, e, target);
        }

        /// <summary>
        /// Notification for column header-related DependencyProperty changes from the grid or from columns.
        /// </summary>
        internal void NotifyPropertyChanged(DependencyObject d, string propertyName, DependencyPropertyChangedEventArgs e, NotificationTarget target)
        {
            DataGridColumn column = d as DataGridColumn;
            if (DataGridHelper.ShouldNotifyColumnHeadersPresenter(target))
            {
                if (e.Property == DataGridColumn.WidthProperty ||
                    e.Property == DataGridColumn.DisplayIndexProperty)
                {
                    if (column.IsVisible)
                    {
                        InvalidateDataGridCellsPanelMeasureAndArrange();
                    }
                }
                else if (e.Property == DataGridColumn.VisibilityProperty ||
                    e.Property == DataGrid.CellsPanelHorizontalOffsetProperty ||
                    string.Compare(propertyName, "ViewportWidth", StringComparison.Ordinal) == 0 ||
                    string.Compare(propertyName, "DelayedColumnWidthComputation", StringComparison.Ordinal) == 0)
                {
                    InvalidateDataGridCellsPanelMeasureAndArrange();
                }
                else if (e.Property == DataGrid.HorizontalScrollOffsetProperty)
                {
                    InvalidateArrange();
                    InvalidateDataGridCellsPanelMeasureAndArrange();
                }
                else if (string.Compare(propertyName, "RealizedColumnsBlockListForNonVirtualizedRows", StringComparison.Ordinal) == 0)
                {
                    InvalidateDataGridCellsPanelMeasureAndArrange(/* withColumnVirtualization */ false);
                }
                else if (string.Compare(propertyName, "RealizedColumnsBlockListForVirtualizedRows", StringComparison.Ordinal) == 0)
                {
                    InvalidateDataGridCellsPanelMeasureAndArrange(/* withColumnVirtualization */ true);
                }
                else if (e.Property == DataGrid.CellsPanelActualWidthProperty)
                {
                    InvalidateArrange();
                }
                else if (e.Property == DataGrid.EnableColumnVirtualizationProperty)
                {
                    DataGridHelper.TransferProperty(this, VirtualizingStackPanel.IsVirtualizingProperty);
                }
            }
            
            if (DataGridHelper.ShouldNotifyColumnHeaders(target))
            {
                if (e.Property == DataGridColumn.HeaderProperty)
                {
                    if (HeaderCollection != null)
                    {
                        HeaderCollection.NotifyHeaderPropertyChanged(column, e);
                    }
                }
                else
                {
                    // Notify the DataGridColumnHeader objects about property changes
                    ContainerTracking<DataGridColumnHeader> tracker = _headerTrackingRoot;

                    while (tracker != null)
                    {
                        tracker.Container.NotifyPropertyChanged(d, e);
                        tracker = tracker.Next;
                    }
                }
            }
        }

        #endregion 

        #region Column Virtualization

        /// <summary>
        ///     Property changed callback for VirtualizingStackPanel.IsVirtualizing property
        /// </summary>
        private static void OnIsVirtualizingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataGridColumnHeadersPresenter headersPresenter = (DataGridColumnHeadersPresenter)d;
            DataGridHelper.TransferProperty(headersPresenter, VirtualizingStackPanel.IsVirtualizingProperty);
            if (e.OldValue != headersPresenter.GetValue(VirtualizingStackPanel.IsVirtualizingProperty))
            {
                headersPresenter.InvalidateDataGridCellsPanelMeasureAndArrange();
            }
        }

        /// <summary>
        ///     Coercion callback for VirtualizingStackPanel.IsVirtualizing property
        /// </summary>
        private static object OnCoerceIsVirtualizingProperty(DependencyObject d, object baseValue)
        {
            var headersPresenter = d as DataGridColumnHeadersPresenter;
            return DataGridHelper.GetCoercedTransferPropertyValue(
                headersPresenter, 
                baseValue, 
                VirtualizingStackPanel.IsVirtualizingProperty,
                headersPresenter.ParentDataGrid, 
                DataGrid.EnableColumnVirtualizationProperty);
        }

        /// <summary>
        ///     Helper method which invalidate the underlying itemshost's measure and arrange
        /// </summary>
        private void InvalidateDataGridCellsPanelMeasureAndArrange()
        {
            if (_internalItemsHost != null)
            {
                _internalItemsHost.InvalidateMeasure();
                _internalItemsHost.InvalidateArrange();
            }
        }

        /// <summary>
        ///     Helper method which invalidate the underlying itemshost's measure and arrange
        /// </summary>
        /// <param name="withColumnVirtualization">
        ///     True to invalidate only when virtualization is on.
        ///     False to invalidate only when virtualization is off.
        /// </param>
        private void InvalidateDataGridCellsPanelMeasureAndArrange(bool withColumnVirtualization)
        {
            // Invalidates measure and arrange if the flag and the virtualization
            // are either both true or both false.
            if (withColumnVirtualization == VirtualizingStackPanel.GetIsVirtualizing(this))
            {
                InvalidateDataGridCellsPanelMeasureAndArrange();
            }
        }

        /// <summary>
        ///     Workaround for not being able to access the panel instance of 
        ///     itemscontrol directly
        /// </summary>
        internal Panel InternalItemsHost
        {
            get { return _internalItemsHost; }
            set { _internalItemsHost = value; }
        }

        #endregion

        /// <summary>
        /// Override of VisualChildrenCount which accomodates the indicators as visual children.
        /// </summary>
        protected override int VisualChildrenCount
        {
            get
            {
                int visualChildrenCount = base.VisualChildrenCount;
                return visualChildrenCount;
            }
        }

        /// <summary>
        /// Override of GetVisualChild which accomodates the indicators as visual children.
        /// </summary>
        protected override Visual GetVisualChild(int index)
        {
            return base.GetVisualChild(index);
        }

        /// <summary>
        /// Gets called on mouse left button down of child header, and ensures preparation for column header drag.
        /// </summary>
        internal void OnHeaderMouseLeftButtonDown(MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Gets called on mouse move of child header, and ensures column header drag
        /// </summary>
        internal void OnHeaderMouseMove(MouseEventArgs e)
        {
        }

        /// <summary>
        /// Gets called on mouse left button up of child header, and ensures reordering of columns on successful completion of drag.
        /// </summary>
        internal void OnHeaderMouseLeftButtonUp(MouseButtonEventArgs e)
        {
        }

        /// <summary>
        /// Gets called on mouse lost capture of child header and ensures that when capture gets lost
        /// the drag ends in appropriate state. In this case it restore the drag state to
        /// the start of the operation by finishing the drag with cancel flag.
        /// </summary>
        internal void OnHeaderLostMouseCapture(MouseEventArgs e)
        {
        }

        /// <summary>
        /// Helper method to determine the display index based on the given position.
        /// </summary>
        private int FindDisplayIndexByPosition(Point startPos, bool findNearestColumn)
        {
            Point headerPos;
            int displayIndex;
            DataGridColumnHeader header;
            FindDisplayIndexAndHeaderPosition(startPos, findNearestColumn, out displayIndex, out headerPos, out header);
            return displayIndex;
        }

        /// <summary>
        /// Helper method to determine the column header based on the given position.
        /// </summary>
        private DataGridColumnHeader FindColumnHeaderByPosition(Point startPos)
        {
            Point headerPos;
            int displayIndex;
            DataGridColumnHeader header;
            FindDisplayIndexAndHeaderPosition(startPos, false, out displayIndex, out headerPos, out header);
            return header;
        }

        /// <summary>
        /// Helper method to determine the position of drop indicator based on the given mouse position.
        /// </summary>
        private Point FindColumnHeaderPositionByCurrentPosition(Point startPos, bool findNearestColumn)
        {
            Point headerPos;
            int displayIndex;
            DataGridColumnHeader header;
            FindDisplayIndexAndHeaderPosition(startPos, findNearestColumn, out displayIndex, out headerPos, out header);
            return headerPos;
        }

        /// <summary>
        /// Helper method which estimates the column width.
        /// </summary>
        private static double GetColumnEstimatedWidth(DataGridColumn column, double averageColumnWidth)
        {
            double columnEstimatedWidth = column.Width.DisplayValue;
            if (DoubleUtil.IsNaN(columnEstimatedWidth))
            {
                columnEstimatedWidth = Math.Max(averageColumnWidth, column.MinWidth);
                columnEstimatedWidth = Math.Min(columnEstimatedWidth, column.MaxWidth);
            }

            return columnEstimatedWidth;
        }

        /// <summary>
        /// Helper method to find display index, header and header start position based on given mouse position.
        /// </summary>
        private void FindDisplayIndexAndHeaderPosition(Point startPos, bool findNearestColumn, out int displayIndex, out Point headerPos, out DataGridColumnHeader header)
        {
            Debug.Assert(ParentDataGrid != null, "ParentDataGrid is null");

            Point originPoint = new Point(0, 0);
            headerPos = originPoint;
            displayIndex = -1;
            header = null;

            if (startPos.X < 0.0)
            {
                if (findNearestColumn)
                {
                    displayIndex = 0;
                }

                return;
            }

            double headerStartX = 0.0;
            double headerEndX = 0.0;
            int i = 0;
            DataGrid dataGrid = ParentDataGrid;
            double averageColumnWidth = dataGrid.InternalColumns.AverageColumnWidth;
            bool firstVisibleNonFrozenColumnHandled = false;
            for (i = 0; i < dataGrid.Columns.Count; i++)
            {
                displayIndex++;
                DataGridColumnHeader currentHeader = dataGrid.ColumnHeaderFromDisplayIndex(i);
                if (currentHeader == null)
                {
                    DataGridColumn column = dataGrid.ColumnFromDisplayIndex(i);
                    if (!column.IsVisible)
                    {
                        continue;
                    }
                    else
                    {
                        headerStartX = headerEndX;
                        if (!firstVisibleNonFrozenColumnHandled)
                        {
                            headerStartX -= dataGrid.HorizontalScrollOffset;
                            firstVisibleNonFrozenColumnHandled = true;
                        }

                        headerEndX = headerStartX + GetColumnEstimatedWidth(column, averageColumnWidth);
                    }
                }
                else
                {
                    GeneralTransform transform = currentHeader.TransformToAncestor(this);
                    headerStartX = transform.Transform(originPoint).X;
                    headerEndX = headerStartX + currentHeader.RenderSize.Width;
                }

                if (DoubleUtil.LessThanOrClose(startPos.X, headerStartX))
                {
                    break;
                }

                if (DoubleUtil.GreaterThanOrClose(startPos.X, headerStartX) &&
                    DoubleUtil.LessThanOrClose(startPos.X, headerEndX))
                {
                    if (findNearestColumn)
                    {
                        double headerMidX = (headerStartX + headerEndX) * 0.5;
                        if (DoubleUtil.GreaterThanOrClose(startPos.X, headerMidX))
                        {
                            headerStartX = headerEndX;
                            displayIndex++;
                        }
                    }
                    else
                    {
                        header = currentHeader;
                    }

                    break;
                }
            }

            if (i == dataGrid.Columns.Count)
            {
                displayIndex = dataGrid.Columns.Count - 1;
                headerStartX = headerEndX;
            }

            headerPos.X = headerStartX;
            return;
        }

        #region Helpers

        private ColumnHeaderCollection HeaderCollection
        {
            get
            {
                return ItemsSource as ColumnHeaderCollection;
            }
        }

        internal DataGrid ParentDataGrid
        {
            get
            {
                if (_parentDataGrid == null)
                {
                    _parentDataGrid = DataGridHelper.FindParent<DataGrid>(this);
                }

                return _parentDataGrid;
            }
        }

        internal ContainerTracking<DataGridColumnHeader> HeaderTrackingRoot
        {
            get
            {
                return _headerTrackingRoot;
            }
        }

        #endregion 

        #endregion
    }
}