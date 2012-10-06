//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Collections.ObjectModel;

namespace Microsoft.Windows.Controls
{
    public class DataGridRowEventArgs : EventArgs
    {
        public DataGridRowEventArgs(DataGridRow row)
        {
            Row = row;
        }

        public DataGridRow Row
        {
            get;
            private set;
        }
    }


    /// <summary>
    ///     EventArgs used for events related to DataGridColumn.
    /// </summary>
    public class DataGridColumnEventArgs : EventArgs
    {
        /// <summary>
        ///     Instantiates a new instance of this class.
        /// </summary>
        public DataGridColumnEventArgs(DataGridColumn column)
        {
            _column = column;
        }

        /// <summary>
        ///     DataGridColumn that the DataGridColumnEventArgs refers to
        /// </summary>
        public DataGridColumn Column
        {
            get { return _column; }
        }

        private DataGridColumn _column;
    }

    /// <summary>
    /// Provides information just before a cell enters edit mode.
    /// </summary>
    public class DataGridBeginningEditEventArgs : EventArgs
    {
        /// <summary>
        ///     Instantiates a new instance of this class.
        /// </summary>
        /// <param name="column">The column of the cell that is about to enter edit mode.</param>
        /// <param name="row">The row container of the cell container that is about to enter edit mode.</param>
        /// <param name="editingEventArgs">The event arguments, if any, that led to the cell entering edit mode.</param>
        public DataGridBeginningEditEventArgs(DataGridColumn column, DataGridRow row, RoutedEventArgs editingEventArgs)
        {
            _dataGridColumn = column;
            _dataGridRow = row;
            _editingEventArgs = editingEventArgs;
        }

        /// <summary>
        ///     When true, prevents the cell from entering edit mode.
        /// </summary>
        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

        /// <summary>
        ///     The column of the cell that is about to enter edit mode.
        /// </summary>
        public DataGridColumn Column
        {
            get { return _dataGridColumn; }
        }

        /// <summary>
        ///     The row container of the cell container that is about to enter edit mode.
        /// </summary>
        public DataGridRow Row
        {
            get { return _dataGridRow; }
        }

        /// <summary>
        ///     Event arguments, if any, that led to the cell entering edit mode.
        /// </summary>
        public RoutedEventArgs EditingEventArgs
        {
            get { return _editingEventArgs; }
        }

        private bool _cancel;
        private DataGridColumn _dataGridColumn;
        private DataGridRow _dataGridRow;
        private RoutedEventArgs _editingEventArgs;
    }


    /// <summary>
    /// This class encapsulates a cell information necessary for CopyingCellClipboardContent and PastingCellClipboardContent events
    /// </summary>
    public class DataGridCellClipboardEventArgs : EventArgs
    {
        /// <summary>
        /// Construct DataGridCellClipboardEventArgs object
        /// </summary>
        /// <param name="item"></param>
        /// <param name="column"></param>
        /// <param name="content"></param>
        public DataGridCellClipboardEventArgs(object item, DataGridColumn column, object content)
        {
            _item = item;
            _column = column;
            _content = content;
        }

        /// <summary>
        /// Content of the cell to be set or get from clipboard
        /// </summary>
        public object Content
        {
            get { return _content; }
            set { _content = value; }
        }

        /// <summary>
        /// DataGrid row item containing the cell
        /// </summary>
        public object Item
        {
            get { return _item; }
        }

        /// <summary>
        /// DataGridColumn containing the cell
        /// </summary>
        public DataGridColumn Column
        {
            get { return _column; }
        }

        private object _content;
        private object _item;
        private DataGridColumn _column;
    }


    /// <summary>
    /// Provides information just before a cell exits edit mode.
    /// </summary>
    public class DataGridCellEditEndingEventArgs : EventArgs
    {
        /// <summary>
        ///     Instantiates a new instance of this class.
        /// </summary>
        /// <param name="column">The column of the cell that is about to exit edit mode.</param>
        /// <param name="row">The row container of the cell container that is about to exit edit mode.</param>
        /// <param name="editingElement">The editing element within the cell.</param>
        /// <param name="editingUnit">The editing unit that is about to leave edit mode.</param>
        public DataGridCellEditEndingEventArgs(DataGridColumn column, DataGridRow row, FrameworkElement editingElement, DataGridEditAction editAction)
        {
            _dataGridColumn = column;
            _dataGridRow = row;
            _editingElement = editingElement;
            _editAction = editAction;
        }

        /// <summary>
        ///     When true, prevents the cell from exiting edit mode.
        /// </summary>
        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

        /// <summary>
        ///     The column of the cell that is about to exit edit mode.
        /// </summary>
        public DataGridColumn Column
        {
            get { return _dataGridColumn; }
        }

        /// <summary>
        ///     The row container of the cell container that is about to exit edit mode.
        /// </summary>
        public DataGridRow Row
        {
            get { return _dataGridRow; }
        }

        /// <summary>
        ///     The editing element within the cell. 
        /// </summary>
        public FrameworkElement EditingElement
        {
            get { return _editingElement; }
        }

        /// <summary>
        ///     The edit action when leave edit mode.
        /// </summary>
        public DataGridEditAction EditAction
        {
            get { return _editAction; }
        }

        private bool _cancel;
        private DataGridColumn _dataGridColumn;
        private DataGridRow _dataGridRow;
        private FrameworkElement _editingElement;
        private DataGridEditAction _editAction;
    }


    /// <summary>
    /// Provides information about a cell that has just entered edit mode.
    /// </summary>
    public class DataGridPreparingCellForEditEventArgs : EventArgs
    {
        /// <summary>
        ///     Constructs a new instance of these event arguments.
        /// </summary>
        /// <param name="column">The column of the cell that just entered edit mode.</param>
        /// <param name="row">The row container that contains the cell container that just entered edit mode.</param>
        /// <param name="editingEventArgs">The event arguments, if any, that led to the cell being placed in edit mode.</param>
        /// <param name="cell">The cell container that just entered edit mode.</param>
        /// <param name="editingElement">The editing element within the cell container.</param>
        public DataGridPreparingCellForEditEventArgs(DataGridColumn column, DataGridRow row, RoutedEventArgs editingEventArgs, FrameworkElement editingElement)
        {
            _dataGridColumn = column;
            _dataGridRow = row;
            _editingEventArgs = editingEventArgs;
            _editingElement = editingElement;
        }

        /// <summary>
        ///     The column of the cell that just entered edit mode.
        /// </summary>
        public DataGridColumn Column
        {
            get { return _dataGridColumn; }
        }

        /// <summary>
        ///     The row container that contains the cell container that just entered edit mode.
        /// </summary>
        public DataGridRow Row
        {
            get { return _dataGridRow; }
        }

        /// <summary>
        ///     The event arguments, if any, that led to the cell being placed in edit mode.
        /// </summary>
        public RoutedEventArgs EditingEventArgs
        {
            get { return _editingEventArgs; }
        }

        /// <summary>
        ///     The editing element within the cell container.
        /// </summary>
        public FrameworkElement EditingElement
        {
            get { return _editingElement; }
        }

        private DataGridColumn _dataGridColumn;
        private DataGridRow _dataGridRow;
        private RoutedEventArgs _editingEventArgs;
        private FrameworkElement _editingElement;
    }


    /// <summary>
    /// This class encapsulates a selected row information necessary for CopyingRowClipboardContent event
    /// </summary>
    public class DataGridRowClipboardEventArgs : EventArgs
    {
        /// <summary>
        /// Creates DataGridRowClipboardEventArgs object initializing the properties.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="startColumnDisplayIndex"></param>
        /// <param name="endColumnDisplayIndex"></param>
        /// <param name="isColumnHeadersRow"></param>
        public DataGridRowClipboardEventArgs(object item, int startColumnDisplayIndex, int endColumnDisplayIndex, bool isColumnHeadersRow)
        {
            _item = item;
            _startColumnDisplayIndex = startColumnDisplayIndex;
            _endColumnDisplayIndex = endColumnDisplayIndex;
            _isColumnHeadersRow = isColumnHeadersRow;
        }

        internal DataGridRowClipboardEventArgs(object item, int startColumnDisplayIndex, int endColumnDisplayIndex, bool isColumnHeadersRow, int rowIndexHint) :
            this(item, startColumnDisplayIndex, endColumnDisplayIndex, isColumnHeadersRow)
        {
            _rowIndexHint = rowIndexHint;
        }

        /// <summary>
        /// DataGrid row item for which we prepare ClipboardRowContent
        /// </summary>
        public object Item
        {
            get { return _item; }
        }

        /// <summary>
        /// This list should be used to modify, add ot remove a cell content before it gets stored into the clipboard.
        /// </summary>
        public List<DataGridClipboardCellContent> ClipboardRowContent
        {
            get
            {
                if (_clipboardRowContent == null)
                {
                    _clipboardRowContent = new List<DataGridClipboardCellContent>();
                }

                return _clipboardRowContent;
            }
        }

        /// <summary>
        /// This method serialize ClipboardRowContent list into string using the specified format.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string FormatClipboardCellValues(string format)
        {
            StringBuilder sb = new StringBuilder();
            int count = ClipboardRowContent.Count;
            for (int i = 0; i < count; i++)
            {
                ClipboardHelper.FormatCell(ClipboardRowContent[i].Content, i == 0 /* firstCell */, i == count - 1 /* lastCell */, sb, format);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Represents the DisplayIndex of the first selected column
        /// </summary>
        public int StartColumnDisplayIndex
        {
            get { return _startColumnDisplayIndex; }
        }

        /// <summary>
        /// Represents the DisplayIndex of the last selected column
        /// </summary>
        public int EndColumnDisplayIndex
        {
            get { return _endColumnDisplayIndex; }
        }

        /// <summary>
        /// This property is true when the ClipboardRowContent represents column headers. In this case Item is null.
        /// </summary>
        public bool IsColumnHeadersRow
        {
            get { return _isColumnHeadersRow; }
        }

        /// <summary>
        /// If the row index was known at creation time, this will be non-negative.
        /// </summary>
        internal int RowIndexHint
        {
            get { return _rowIndexHint; }
        }

        private int _startColumnDisplayIndex;
        private int _endColumnDisplayIndex;
        private object _item;
        private bool _isColumnHeadersRow;
        private List<DataGridClipboardCellContent> _clipboardRowContent;
        private int _rowIndexHint = -1;
    }


    /// <summary>
    /// Provides information just before a row exits edit mode.
    /// </summary>
    public class DataGridRowEditEndingEventArgs : EventArgs
    {
        /// <summary>
        ///     Instantiates a new instance of this class.
        /// </summary>
        /// <param name="row">The row container of the cell container that is about to exit edit mode.</param>
        /// <param name="editingUnit">The editing unit that is about to leave edit mode.</param>
        public DataGridRowEditEndingEventArgs(DataGridRow row, DataGridEditAction editAction)
        {
            _dataGridRow = row;
            _editAction = editAction;
        }

        /// <summary>
        ///     When true, prevents the row from exiting edit mode.
        /// </summary>
        public bool Cancel
        {
            get { return _cancel; }
            set { _cancel = value; }
        }

        /// <summary>
        ///     The row container of the cell container that is about to exit edit mode.
        /// </summary>
        public DataGridRow Row
        {
            get { return _dataGridRow; }
        }

        /// <summary>
        ///     The edit action when leave edit mode.
        /// </summary>
        public DataGridEditAction EditAction
        {
            get { return _editAction; }
        }

        private bool _cancel;
        private DataGridRow _dataGridRow;
        private DataGridEditAction _editAction;
    }



    /// <summary>
    /// Provides access to the new item during the InitializingNewItem event.
    /// </summary>
    public class InitializingNewItemEventArgs : EventArgs
    {
        /// <summary>
        /// Instantiates a new instance of this class.
        /// </summary>
        public InitializingNewItemEventArgs(object newItem)
        {
            _newItem = newItem;
        }

        /// <summary>
        /// The new item.
        /// </summary>
        public object NewItem
        {
            get { return _newItem; }
        }

        private object _newItem;
    }

    /// <summary>
    ///     Delegate used for the InitializingNewItem event on DataGrid.
    /// </summary>
    /// <param name="sender">The DataGrid that raised the event.</param>
    /// <param name="e">The event arguments where callbacks can access the new item.</param>
    public delegate void InitializingNewItemEventHandler(object sender, InitializingNewItemEventArgs e);



    /// <summary>
    ///     Communicates which cells were added or removed from the SelectedCells collection.
    /// </summary>
    public class SelectedCellsChangedEventArgs : EventArgs
    {
        /// <summary>
        ///     Creates a new instance of this class.
        /// </summary>
        /// <param name="addedCells">The cells that were added. Must be non-null, but may be empty.</param>
        /// <param name="removedCells">The cells that were removed. Must be non-null, but may be empty.</param>
        public SelectedCellsChangedEventArgs(List<DataGridCellInfo> addedCells, List<DataGridCellInfo> removedCells)
        {
            if (addedCells == null)
            {
                throw new ArgumentNullException("addedCells");
            }

            if (removedCells == null)
            {
                throw new ArgumentNullException("removedCells");
            }

            _addedCells = addedCells.AsReadOnly();
            _removedCells = removedCells.AsReadOnly();
        }

        /// <summary>
        ///     Creates a new instance of this class.
        /// </summary>
        /// <param name="addedCells">The cells that were added. Must be non-null, but may be empty.</param>
        /// <param name="removedCells">The cells that were removed. Must be non-null, but may be empty.</param>
        public SelectedCellsChangedEventArgs(ReadOnlyCollection<DataGridCellInfo> addedCells, ReadOnlyCollection<DataGridCellInfo> removedCells)
        {
            if (addedCells == null)
            {
                throw new ArgumentNullException("addedCells");
            }

            if (removedCells == null)
            {
                throw new ArgumentNullException("removedCells");
            }

            _addedCells = addedCells;
            _removedCells = removedCells;
        }

        internal SelectedCellsChangedEventArgs(DataGrid owner, VirtualizedCellInfoCollection addedCells, VirtualizedCellInfoCollection removedCells)
        {
            _addedCells = (addedCells != null) ? addedCells : VirtualizedCellInfoCollection.MakeEmptyCollection(owner);
            _removedCells = (removedCells != null) ? removedCells : VirtualizedCellInfoCollection.MakeEmptyCollection(owner);

            Debug.Assert(_addedCells.IsReadOnly, "_addedCells should have ended up as read-only.");
            Debug.Assert(_removedCells.IsReadOnly, "_removedCells should have ended up as read-only.");
        }

        /// <summary>
        ///     The cells that were added.
        /// </summary>
        public IList<DataGridCellInfo> AddedCells
        {
            get { return _addedCells; }
        }

        /// <summary>
        ///     The cells that were removed.
        /// </summary>
        public IList<DataGridCellInfo> RemovedCells
        {
            get { return _removedCells; }
        }

        private IList<DataGridCellInfo> _addedCells;
        private IList<DataGridCellInfo> _removedCells;
    }

    /// <summary>
    ///     An event handler used to notify of changes to the SelectedCells collection.
    /// </summary>
    /// <param name="sender">The DataGrid that owns the SelectedCells collection that changed.</param>
    /// <param name="e">Event arguments that communicate which cells were added or removed.</param>
    public delegate void SelectedCellsChangedEventHandler(object sender, SelectedCellsChangedEventArgs e);
}
