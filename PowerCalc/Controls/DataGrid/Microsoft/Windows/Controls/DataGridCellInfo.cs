//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Defines a structure that identifies a specific cell based on row and column data.
    /// </summary>
    public struct DataGridCellInfo
    {
        #region Fields

        private object _item;
        private DataGridColumn _column;
        private WeakReference _owner;

        #endregion

        #region Properties

        /// <summary>
        /// The item who's row contains the cell.
        /// </summary>
        public object Item
        {
            get { return _item; }
        }

        public string Value
        {
            get
            {
                FrameworkElement elem = Column.GetCellContent(Item);

                if (elem is TextBlock)
                    return ((TextBlock)elem).Text;
                else if (elem is TextBox)
                    return ((TextBox)elem).Text;
                else
                    return null;
            }

            set
            {
                FrameworkElement elem = Column.GetCellContent(Item);

                if (elem is TextBlock)
                    ((TextBlock)elem).Text = value;
                else if (elem is TextBox)
                    ((TextBox)elem).Text = value;
            }
        }

        /// <summary>
        /// The column of the cell within the row.
        /// </summary>
        public DataGridColumn Column
        {
            get
            {
                return _column;
            }
        }

        /// <summary>
        /// Gets the row index for this cell.
        /// </summary>
        public int RowIndex
        {
            get
            {
                return Owner.Items.IndexOf(Item);
            }
        }

        /// <summary>
        /// Gets the column index for this cell.
        /// </summary>
        public int ColumnIndex
        {
            get
            {
                return Column.DisplayIndex;
            }
        }

        private DataGrid Owner
        {
            get
            {
                if (_owner != null)
                {
                    return (DataGrid)_owner.Target;
                }

                return null;
            }
        }

        /// <summary>
        /// Whether the structure holds valid information.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return ArePropertyValuesValid;
            }
        }

        private bool ArePropertyValuesValid
        {
            get { return (_item != DependencyProperty.UnsetValue) && (_column != null); }
        }

        /// <summary>
        /// Used for default values.
        /// </summary>
        internal static DataGridCellInfo Unset
        {
            get { return new DataGridCellInfo(DependencyProperty.UnsetValue); }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Identifies a cell at the column within the row for the specified item.
        /// </summary>
        /// <param name="item">The item who's row contains the cell.</param>
        /// <param name="column">The column of the cell within the row.</param>
        /// <remarks>
        /// This constructor will not tie the DataGridCellInfo to any particular
        /// DataGrid.
        /// </remarks>
        public DataGridCellInfo(object item, DataGridColumn column)
        {
            if (column == null)
            {
                throw new ArgumentNullException("column");
            }

            _item = item;
            _column = column;
            _owner = null;
        }

        /// <summary>
        /// Creates a structure that identifies the specific cell container.
        /// </summary>
        /// <param name="cell">
        /// A reference to a cell.
        /// This structure does not maintain a strong reference to the cell.
        /// Changes to the cell will not affect this structure.
        /// </param>
        /// <remarks>
        /// This constructor will tie the DataGridCellInfo to the specific
        /// DataGrid that owns the cell container.
        /// </remarks>
        public DataGridCellInfo(DataGridCell cell)
        {
            if (cell == null)
            {
                throw new ArgumentNullException("cell");
            }

            _item = cell.RowDataItem;
            _column = cell.Column;
            _owner = new WeakReference(cell.DataGridOwner);
        }

        internal DataGridCellInfo(object item, DataGridColumn column, DataGrid owner)
        {
            Debug.Assert(item != null, "item should not be null.");
            Debug.Assert(column != null, "column should not be null.");
            Debug.Assert(owner != null, "owner should not be null.");

            _item = item;
            _column = column;
            _owner = new WeakReference(owner);
        }

        /// <summary>
        /// Used to create an unset DataGridCellInfo.
        /// </summary>
        internal DataGridCellInfo(object item)
        {
            Debug.Assert(item == DependencyProperty.UnsetValue, "This should only be used to make an Unset CellInfo.");
            _item = item;
            _column = null;
            _owner = null;
        }

        /// <summary>
        /// This is used strictly to create the partial CellInfos.
        /// </summary>
        /// <remarks>
        /// This is being kept private so that it is explicit that the
        /// caller expects invalid data.
        /// </remarks>
        private DataGridCellInfo(DataGrid owner, DataGridColumn column, object item)
        {
            Debug.Assert(owner != null, "owner should not be null.");

            _item = item;
            _column = column;
            _owner = new WeakReference(owner);
        }

        #endregion

        #region Methods

        /// <summary>
        /// This is used by CurrentCell if there isn't a valid CurrentItem or CurrentColumn.
        /// </summary>
        internal static DataGridCellInfo CreatePossiblyPartialCellInfo(object item, DataGridColumn column, DataGrid owner)
        {
            Debug.Assert(owner != null, "owner should not be null.");

            if ((item == null) && (column == null))
            {
                return Unset;
            }
            else
            {
                return new DataGridCellInfo(owner, column, (item == null) ? DependencyProperty.UnsetValue : item);
            }
        }

        /// <summary>
        /// Whether the two objects are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is DataGridCellInfo)
            {
                return EqualsImpl((DataGridCellInfo)obj);
            }

            return false;
        }

        /// <summary>
        /// Returns whether the two values are equal.
        /// </summary>
        public static bool operator ==(DataGridCellInfo cell1, DataGridCellInfo cell2)
        {
            return cell1.EqualsImpl(cell2);
        }

        /// <summary>
        /// Returns whether the two values are not equal.
        /// </summary>
        public static bool operator !=(DataGridCellInfo cell1, DataGridCellInfo cell2)
        {
            return !cell1.EqualsImpl(cell2);
        }

        internal bool EqualsImpl(DataGridCellInfo cell)
        {
            return (cell._item == _item) && (cell._column == _column) && (cell.Owner == Owner);
        }

        /// <summary>
        /// Returns a hash code for the structure.
        /// </summary>
        public override int GetHashCode()
        {
            return ((_item == null) ? 0 : _item.GetHashCode()) ^
                   ((_column == null) ? 0 : _column.GetHashCode());
        }

        /// <summary>
        /// Assumes that if the owner matches, then the column and item fields are valid.
        /// </summary>
        internal bool IsValidForDataGrid(DataGrid dataGrid)
        {
            DataGrid owner = Owner;
            return (ArePropertyValuesValid && (owner == dataGrid)) || (owner == null);
        }

        #endregion
    }
}
