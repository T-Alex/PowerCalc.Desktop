using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TAlex.WPF.Mvvm;
using TAlex.WPF.Mvvm.Commands;


namespace TAlex.PowerCalc.ViewModels
{
    public class InsertMatrixViewModel : ViewModelBase
    {
        #region Fields

        private MatrixViewModel _matrix;
        private bool _closeSignal;

        #endregion

        #region Properties

        public MatrixViewModel Matrix
        {
            get
            {
                return _matrix;
            }

            set
            {
                Set(() => Matrix, ref _matrix, value);

                if (_matrix != null)
                {
                    _matrix.ColumnsCountChanged -= Matrix_ColumnsCountChanged;
                    _matrix.ColumnsCountChanged += Matrix_ColumnsCountChanged;
                }
            }
        }

        public bool CloseSignal
        {
            get
            {
                return _closeSignal;
            }

            set
            {
                Set(() => CloseSignal, ref _closeSignal, value);
            }
        }

        #endregion

        #region Commands

        public ICommand InsertCommand { get; set; }

        #endregion

        #region Constructors

        public InsertMatrixViewModel()
        {
            InitializeCommands();
        }

        #endregion

        #region Methods

        private void InitializeCommands()
        {
            InsertCommand = new RelayCommand(Insert);
        }

        private void Insert()
        {
            CloseSignal = true;
        }

        #endregion

        #region Event Handlers

        private void Matrix_ColumnsCountChanged(object sender, EventArgs e)
        {
            var temp = Matrix;
            Matrix = null;
            Matrix = temp;
        }

        #endregion
    }

    public class MatrixViewModel : ViewModelBase, IList, INotifyCollectionChanged
    {
        #region Fields

        private int _rowsCount;
        private int _columnsCount;
        private ObservableCollection<MatrixRowViewModel> _rows;

        #endregion

        #region Properties

        public int Rows
        {
            get
            {
                return _rowsCount;
            }

            set
            {
                Set(() => Rows, ref _rowsCount, value);
                ResizeRows(value);
            }
        }

        public int Columns
        {
            get
            {
                return _columnsCount;
            }

            set
            {
                Set(() => Columns, ref _columnsCount, value);
                ResizeColumns(value);
            }
        }

        public string this[int row, int col]
        {
            get
            {
                return _rows[row][col];
            }
        }

        #endregion

        #region Events

        public event EventHandler ColumnsCountChanged;

        #endregion

        #region Constructors

        public MatrixViewModel(int rows, int columns)
        {
            _rows = new ObservableCollection<MatrixRowViewModel>();
            Initialize(rows, columns);
        }

        #endregion

        #region Methods

        protected virtual void Initialize(int rows, int columns)
        {
            _rows.Clear();
            for (int i = 0; i < rows; i++)
            {
                _rows.Add(new MatrixRowViewModel(columns) { RowNumber = i });
            }

            Set(() => Rows, ref _rowsCount, rows);
            Set(() => Columns, ref _columnsCount, columns);
        }

        protected virtual void ResizeRows(int rows)
        {
            if (_rows.Count < rows)
            {
                int rowsToAdd = rows - _rows.Count;
                for (int i = 0; i < rowsToAdd; i++)
                {
                    _rows.Add(new MatrixRowViewModel(Columns) { RowNumber = _rows.Count });
                }
                OnCollectionChanged();
            }
            else if (_rows.Count > rows)
            {
                int rowsToDelete = _rows.Count - rows;
                for (int i = 0; i < rowsToDelete; i++)
                {
                    _rows.RemoveAt(_rows.Count - 1);
                }
                OnCollectionChanged();
            }
        }

        protected virtual void ResizeColumns(int columns)
        {
            if (!_rows.Any()) return;

            for (int i = 0; i < _rows.Count; i++)
            {
                _rows[i].Resize(columns);
            }
            OnColumnsCountChanged();
        }

        protected virtual void OnColumnsCountChanged()
        {
            if (ColumnsCountChanged != null)
            {
                ColumnsCountChanged(this, new EventArgs());
            }
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("{");
            sb.Append(String.Join("; ", _rows.Select(x => x.ToString())));
            sb.Append("}");

            return sb.ToString();
        }

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

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void OnCollectionChanged()
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        #endregion
    }

    public class MatrixRowViewModel : ICustomTypeDescriptor
    {
        #region Fields

        private List<string> _cells;

        #endregion

        #region Properties

        public int RowNumber { get; set; }

        public string this[int i]
        {
            get
            {
                return _cells[i];
            }

            set
            {
                _cells[i] = value;
            }
        }

        #endregion

        #region Constructors

        public MatrixRowViewModel(int cells)
        {
            _cells = new List<string>(cells);

            for (int i = 0; i < cells; i++)
            {
                _cells.Add(null);
            }
        }

        #endregion

        #region Methods

        public virtual void Resize(int cells)
        {
            if (_cells.Count < cells)
            {
                int columnsToAdd = cells - _cells.Count;
                for (int i = 0; i < columnsToAdd; i++)
                {
                    _cells.Add(null);
                }
            }
            else if (_cells.Count > cells)
            {
                int columnsToDelete = _cells.Count - cells;
                for (int i = 0; i < columnsToDelete; i++)
                {
                    _cells.RemoveAt(_cells.Count - 1);
                }
            }
        }

        public override string ToString()
        {
            return String.Join(", ", _cells.Select(x => (x + String.Empty).Trim()));
        }

        #endregion

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            return new AttributeCollection();
        }

        public string GetClassName()
        {
            return typeof(MatrixRowViewModel).Name;
        }

        public string GetComponentName()
        {
            throw new NotImplementedException();
        }

        public TypeConverter GetConverter()
        {
            throw new NotImplementedException();
        }

        public EventDescriptor GetDefaultEvent()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptor GetDefaultProperty()
        {
            throw new NotImplementedException();
        }

        public object GetEditor(Type editorBaseType)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public EventDescriptorCollection GetEvents()
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            throw new NotImplementedException();
        }

        public PropertyDescriptorCollection GetProperties()
        {
            return new PropertyDescriptorCollection(
                Enumerable.Range(0, _cells.Count)
                .Select(x => new MatrixRowPropertyDescriptor(x.ToString()))
                .ToArray()
            );
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class MatrixRowPropertyDescriptor : PropertyDescriptor
    {
        public MatrixRowPropertyDescriptor(string name)
            : base(name, null)
        {
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            MatrixRowViewModel row = component as MatrixRowViewModel;
            return row[(int)int.Parse(Name)];
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            MatrixRowViewModel row = component as MatrixRowViewModel;
            row[(int)int.Parse(Name)] = (string)value;
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return typeof(MatrixRowViewModel); }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return typeof(string); }
        }
    }
}
