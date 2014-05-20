using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TAlex.PowerCalc.Helpers;


namespace TAlex.PowerCalc.ViewModels.Matrices
{
    public class DataRow : ICustomTypeDescriptor
    {
        #region Fields

        private DataCell[] _cells;

        #endregion

        #region Properties

        public int RowNumber { get; set; }

        public int Cells
        {
            get { return _cells.Length; }
        }

        public DataCell this[int i]
        {
            get
            {
                return _cells[i];
            }
        }

        public DataCell this[string columnName]
        {
            get
            {
                return this[(int)Helpers.A1ReferenceHelper.IntegerFromA1ReferenceColumn(columnName)];
            }
        }

        #endregion

        #region Constructors

        public DataRow(DataTable dataTable, int cells)
        {
            _cells = new DataCell[cells];

            for (int i = 0; i < cells; i++)
                _cells[i] = new DataCell(dataTable, this);
        }

        #endregion

        #region Methods

        public int IndexOfCell(DataCell cell)
        {
            return ((IList<DataCell>)_cells).IndexOf(cell);
        }

        #endregion

        #region ICustomTypeDescriptor Members

        public AttributeCollection GetAttributes()
        {
            return new AttributeCollection();
        }

        public string GetClassName()
        {
            return typeof(DataRow).Name;
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
                Enumerable.Range(0, _cells.Length)
                .Select(x => new DataRowPropertyDescriptor(A1ReferenceHelper.IntegerToA1ReferenceColumn(x)))
                .ToArray()
            );
        }

        public object GetPropertyOwner(PropertyDescriptor pd)
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class DataRowPropertyDescriptor : PropertyDescriptor
    {
        public DataRowPropertyDescriptor(string name)
            : base(name, null)
        {
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            DataRow row = component as DataRow;
            return row[(int)Helpers.A1ReferenceHelper.IntegerFromA1ReferenceColumn(Name)];
        }

        public override void ResetValue(object component)
        {
        }

        public override void SetValue(object component, object value)
        {
            throw new NotImplementedException();
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }

        public override Type ComponentType
        {
            get { return typeof(DataRow); }
        }

        public override bool IsReadOnly
        {
            get { return false; }
        }

        public override Type PropertyType
        {
            get { return typeof(DataCell); }
        }
    }
}
