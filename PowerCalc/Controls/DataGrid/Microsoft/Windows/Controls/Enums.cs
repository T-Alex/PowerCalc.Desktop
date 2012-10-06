//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Used to specify which container to take out of edit mode.
    /// </summary>
    public enum DataGridEditingUnit
    {
        /// <summary>
        ///     Targets a cell container.
        /// </summary>
        Cell,

        /// <summary>
        ///     Targets a row container.
        /// </summary>
        Row
    }

    /// <summary>
    /// Used to indicate the type of value that DataGridLength is holding.
    /// </summary>
    public enum DataGridLengthUnitType
    {
        // Keep in sync with DataGridLengthConverter.UnitStrings

        /// <summary>
        ///     The value indicates that content should be calculated based on the 
        ///     unconstrained sizes of all cells and header in a column.
        /// </summary>
        Auto,

        /// <summary>
        ///     The value is expressed in pixels.
        /// </summary>
        Pixel,

        /// <summary>
        ///     The value indicates that content should be be calculated based on the
        ///     unconstrained sizes of all cells in a column.
        /// </summary>
        SizeToCells,

        /// <summary>
        ///     The value indicates that content should be calculated based on the
        ///     unconstrained size of the column header.
        /// </summary>
        SizeToHeader,

        /// <summary>
        ///     The value is expressed as a weighted proportion of available space.
        /// </summary>
        Star,
    }

    /// <summary>
    /// The selection modes supported by DataGrid.
    /// </summary>
    public enum DataGridSelectionMode
    {
        /// <summary>
        ///     Only one item can be selected at a time.
        /// </summary>
        Single,

        /// <summary>
        ///     Multiple items can be selected, and the input gestures will default
        ///     to the "extended" mode.
        /// </summary>
        /// <remarks>
        ///     In Extended mode, selecting multiple items requires holding down 
        ///     the SHIFT or CTRL keys to extend the selection from an anchor point.
        /// </remarks>
        Extended,
    }

    /// <summary>
    /// The accepted selection units used in selection on a DataGrid.
    /// </summary>
    public enum DataGridSelectionUnit
    {
        /// <summary>
        ///     Only cells are selectable.
        ///     Clicking on a cell will select the cell.
        ///     Clicking on row or column headers does nothing.
        /// </summary>
        Cell,

        /// <summary>
        ///     Only full rows are selectable.
        ///     Clicking on row headers or on cells will select the whole row.
        /// </summary>
        FullRow,

        /// <summary>
        ///     Cells and rows are selectable.
        ///     Clicking on a cell will select the cell. Selecting all cells in the row will not select the row.
        ///     Clicking on a row header will select the row and all cells in the row.
        /// </summary>
        CellOrRowHeader
    }

    /// <summary>
    /// Defines the visibility modes for DataGrid grid lines.
    /// </summary>
    public enum DataGridGridLinesVisibility
    {
        All,
        Horizontal,
        None,
        Vertical
    }

    /// <summary>
    /// Used to specify action to take out of edit mode.
    /// </summary>
    public enum DataGridEditAction
    {
        /// <summary>
        ///     Cancel the changes.
        /// </summary>
        Cancel,

        /// <summary>
        ///     Commit edited value.
        /// </summary>
        Commit
    }

    [Flags]
    /// <summary>
    /// Determines whether the row/column headers are shown or not.
    /// </summary>
    public enum DataGridHeadersVisibility
    {
        /// <summary>
        /// Show Row, Column, and Corner Headers
        /// </summary>
        All = Row | Column,

        /// <summary>
        /// Show only Column Headers with top-right corner Header
        /// </summary>
        Column = 0x01,

        /// <summary>
        /// Show only Row Headers with bottom-left corner
        /// </summary>
        Row = 0x02,

        /// <summary>
        /// Don’t show any Headers
        /// </summary>
        None = 0x00
    }

    /// <summary>
    /// Defines modes that indicate how DataGrid content is copied to the Clipboard. 
    /// </summary>
    public enum DataGridClipboardCopyMode
    {
        /// <summary>
        /// Copying to the Clipboard is disabled.
        /// </summary>
        None,

        /// <summary>
        /// The text values of selected cells can be copied to the Clipboard. Column header is not included. 
        /// </summary>
        ExcludeHeader,

        /// <summary>
        /// The text values of selected cells can be copied to the Clipboard. Column header is included for columns that contain selected cells.  
        /// </summary>
        IncludeHeader,
    }

    /// <summary>
    ///     Enum to specify the scroll orientation of cells in selective scroll grid
    /// </summary>
    public enum SelectiveScrollingOrientation
    {
        /// <summary>
        /// The cell will not be allowed to get
        /// sctolled in any direction
        /// </summary>
        None = 0,

        /// <summary>
        /// The cell will be allowed to
        /// get scrolled only in horizontal direction
        /// </summary>
        Horizontal = 1,

        /// <summary>
        /// The cell will be allowed to
        /// get scrolled only in vertical directions
        /// </summary>
        Vertical = 2,

        /// <summary>
        /// The cell will be allowed to get
        /// scrolled in all directions
        /// </summary>
        Both = 3
    }

    /// <summary>
    /// Enum used to specify where we want an internal property change notification to be routed.
    /// </summary>
    [Flags]
    internal enum NotificationTarget
    {
        None = 0x00, // this means don't send it on; likely handle it on the same object that raised the event.
        Cells = 0x01,
        CellsPresenter = 0x02,
        Columns = 0x04,
        ColumnCollection = 0x08,
        ColumnHeaders = 0x10,
        ColumnHeadersPresenter = 0x20,
        DataGrid = 0x40,
        DetailsPresenter = 0x80,
        RefreshCellContent = 0x100,
        RowHeaders = 0x200,
        Rows = 0x400,
        All = 0xFFF,
    }
}