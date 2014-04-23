using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;
using TAlex.PowerCalc.ViewModels.Matrices;


namespace TAlex.PowerCalc.ViewModels.Matrices
{
    public class DataTable : IList
    {
        #region Fields

        public readonly IExpressionTreeBuilder<Object> ExpressionTreeBuilder;

        private List<DataRow> _rows;

        #endregion

        #region Properties

        public int Rows
        {
            get;
            private set;
        }

        public int Columns
        {
            get;
            private set;
        }

        public DataCell this[int row, int col]
        {
            get
            {
                return _rows[row][col];
            }
        }

        #endregion

        #region Constructors

        public DataTable(IExpressionTreeBuilder<Object> expressionTreeBuilder)
        {
            ExpressionTreeBuilder = expressionTreeBuilder;
            _rows = new List<DataRow>();
        }

        public DataTable(IExpressionTreeBuilder<Object> expressionTreeBuilder, int rows, int columns)
            : this(expressionTreeBuilder)
        {
            Initialize(rows, columns);
        }

        #endregion

        #region Methods

        public virtual void Initialize(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;

            _rows.Clear();
            for (int i = 0; i < rows; i++)
            {
                _rows.Add(new DataRow(this, columns) { RowNumber = i + 1 });
            }
        }

        public virtual void DeleteCells(IList<DataCellInfo> cells)
        {
            IList<DataCell> dataCells = GetDataCells(cells);
            DataCellHelper.EnsureAllArraysEnclosing(dataCells);
            DataCellHelper.GetAllUniqueArrays(dataCells).ForEach(x => x.Clear());

            foreach (DataCell dataCell in dataCells)
            {
                dataCell.Clear();
            }
        }

        #region Helpers

        public IList<DataCell> GetDataCells(IList<DataCellInfo> cells)
        {
            return cells.Select(x => this[x.RowIndex, x.ColumnIndex]).ToList();
        }

        #endregion

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _rows.GetEnumerator();
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            ((IList)_rows).CopyTo(array, index);
        }

        bool ICollection.IsSynchronized
        {
            get { return ((IList)_rows).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((IList)_rows).SyncRoot; }
        }

        int ICollection.Count
        {
            get { return _rows.Count; }
        }

        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            return ((IList)_rows).Add(value);
        }

        bool IList.Contains(object value)
        {
            return ((IList)_rows).Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return ((IList)_rows).IndexOf(value);
        }

        void IList.Insert(int index, object value)
        {
            ((IList)_rows).Insert(index, value);
        }

        bool IList.IsFixedSize
        {
            get { return ((IList)_rows).IsFixedSize; }
        }

        bool IList.IsReadOnly
        {
            get { return ((IList)_rows).IsReadOnly; }
        }

        void IList.Remove(object value)
        {
            ((IList)_rows).Remove(value);
        }

        void IList.RemoveAt(int index)
        {
            _rows.RemoveAt(index);
        }

        void IList.Clear()
        {
            _rows.Clear();
        }

        object IList.this[int index]
        {
            get
            {
                return ((IList)_rows)[index];
            }
            set
            {
                ((IList)_rows)[index] = value;
            }
        }

        #endregion
    }
}
