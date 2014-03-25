using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;

using TAlex.PowerCalc.Locators;
using TAlex.PowerCalc.Helpers;

using TAlex.MathCore;
using TAlex.MathCore.LinearAlgebra;
using TAlex.MathCore.ExpressionEvaluation;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;
using TAlex.MathCore.ExpressionEvaluation.Trees;
using TAlex.PowerCalc.ViewModels;
using TAlex.WPF.Mvvm;
using System.ComponentModel;


namespace TAlex.PowerCalc.Controls
{
    /// <summary>
    /// Interaction logic for WorksheetMatrix.xaml
    /// </summary>
    public partial class WorksheetMatrix : UserControl
    {
        #region Fields

        private static readonly int DefaultColumnCount = 256;
        private static readonly int DefaultRowCount = 256;

        private bool _formulaBarEdit = false;
        private DataGridCell _lastEditedCellViaFormulaBar = null;

        #endregion

        #region Properties

        public TextBox FormulaBar
        {
            get
            {
                return formulaBarTextBox;
            }
        }

        public ContextMenu FormulaBarContextMenu
        {
            get
            {
                return formulaBarTextBox.ContextMenu;
            }

            set
            {
                formulaBarTextBox.ContextMenu = value;
            }
        }

        #endregion

        #region Constructors

        public WorksheetMatrix()
        {
            InitializeComponent();

            Initialize();
        }

        #endregion

        #region Methods

        public void Initialize()
        {
            int colCount = DefaultColumnCount;
            int rowCount = DefaultRowCount;

            dataGrid.Columns.Clear();
            for (int i = 0; i < colCount; i++)
            {
                dataGrid.Columns.Add(new DataGridTextColumnEx
                {
                    Header = Helpers.A1ReferenceHelper.IntegerToA1ReferenceColumn(i),
                    Binding = new Binding(String.Format("[{0}].FormattedValue", i)) { ValidatesOnExceptions = true, NotifyOnValidationError = true },
                    EditingBinding = new Binding(String.Format("[{0}].Expression", i)) { Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged }
                });
            }

            DataTable dataTable = new DataTable(((WorksheetMatrixViewModel)DataContext).ExpressionTreeBuilder);

            for (int i = 0; i < rowCount; i++)
            {
                dataTable.Rows.Add(new DataRow(dataTable, colCount) { RowNumber = i + 1 });
            }

            dataGrid.ItemsSource = dataTable.Rows;
        }

        #region Event Handlers

        /// <summary>
        /// Auto switching to edit mode after typing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_TextInput(object sender, TextCompositionEventArgs e)
        {
            if (DataGridHelper.HasNonEscapeCharacters(e))
            {
                ((DataGrid)sender).BeginEdit(e);
            }
        }

        /// <summary>
        /// Set binding (Expression property) between formula bar text box and selected cell.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGrid.CurrentCell.Column != null)
            {
                formulaBarTextBox.SetBinding(TextBox.TextProperty, new Binding("Expression") { Source = GetDataCell(dataGrid.CurrentCell), Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            }
        }

        /// <summary>
        /// 1. Commit editing if last editing was via formula bar
        /// 2. Display cell range.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (_lastEditedCellViaFormulaBar != null && _lastEditedCellViaFormulaBar != dataGrid.TryToFindGridCell(dataGrid.CurrentItem, dataGrid.CurrentColumn))
            {
                _lastEditedCellViaFormulaBar.IsEditing = false;
                _lastEditedCellViaFormulaBar = null;
            }
            
            int selectedCells = dataGrid.SelectedCells.Count;
            if (selectedCells > 0)
            {
                DataGridCellInfo firstCellInfo = dataGrid.SelectedCells[0];

                if (selectedCells == 1)
                {
                    nameBoxTextBox.Text = Helpers.A1ReferenceHelper.ToString(firstCellInfo.Column.DisplayIndex, dataGrid.Items.IndexOf(firstCellInfo.Item) + 1);
                }
                else
                {
                    DataGridCellInfo lastCellInfo = dataGrid.SelectedCells[selectedCells - 1];

                    nameBoxTextBox.Text = Helpers.A1ReferenceHelper.ToString(firstCellInfo.Column.DisplayIndex, dataGrid.Items.IndexOf(firstCellInfo.Item) + 1,
                        lastCellInfo.Column.DisplayIndex, dataGrid.Items.IndexOf(lastCellInfo.Item) + 1);
                }
            }
        }

        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (_formulaBarEdit)
            {
                e.Cancel = true;
                _formulaBarEdit = false;
                _lastEditedCellViaFormulaBar = dataGrid.TryToFindGridCell(e.Row.Item, e.Column);
            }

            //---------------------------------------------------------------
            var currentDataCell = GetDataCell(new DataGridCellInfo(e.Row.Item, e.Column));

            if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                DataGridCellInfo firstCell = dataGrid.SelectedCells[0];
                DataGridCellInfo lastCell = dataGrid.SelectedCells[dataGrid.SelectedCells.Count - 1];

                int firstSelectedItemIndex = dataGrid.Items.IndexOf(firstCell.Item);
                int lastSelectedItemIndex = dataGrid.Items.IndexOf(lastCell.Item);
                int firstSelectedColumnIndex = firstCell.Column.DisplayIndex;
                int lastSelectedColumnIndex = lastCell.Column.DisplayIndex;

                int rows = Math.Abs(firstSelectedItemIndex - lastSelectedItemIndex) + 1;
                int cols = Math.Abs(firstSelectedColumnIndex - lastSelectedColumnIndex) + 1;

                int rowOffset = Math.Min(firstSelectedItemIndex, lastSelectedItemIndex);
                int colOffset = Math.Min(firstSelectedColumnIndex, lastSelectedColumnIndex);

                //if (currentDataCell.Parent == null)
                    new DataArray(currentDataCell, colOffset, rowOffset, rows, cols);
                //else
                //    currentDataCell.Parent.Expand(colOffset, rowOffset, rows, cols);
            }
            else
            {
                if (!Keyboard.IsKeyDown(Key.Escape) && currentDataCell.Parent != null)
                {
                    MessageBox.Show("You can't edit part of array", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    e.Cancel = true;
                }
            }
        }

        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    OnDataGridDeleteKeyDown(e);
                    break;
            }
        }

        private void formulaBarTextBox_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool?)e.NewValue == true)
            {
                if (!dataGrid.SelectedCells.Any())
                {
                    SelectFirstCell();
                }

                _formulaBarEdit = true;
                dataGrid.BeginEdit();
                formulaBarTextBox.Focus();
            }
        }

        private void formulaBarTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                    OnFormulaBarEnterKeyDown(e);
                    break;
            }
        }


        protected void OnDataGridDeleteKeyDown(KeyEventArgs e)
        {
            if (!dataGrid.GetCurrentDataGridCell().IsEditing)
            { 
                int selectedCells = dataGrid.SelectedCells.Count;

                for (int i = 0; i < selectedCells; i++)
                {
                    DataGridCellInfo cellInfo = dataGrid.SelectedCells[i];
                    DataCell dataCell = GetDataCell(cellInfo);
                    dataCell.Clear();
                }
            }
        }

        protected void OnFormulaBarEnterKeyDown(KeyEventArgs e)
        {
            if (dataGrid.SelectedCells.Any())
            {
                DataGridCell editingCell = dataGrid.GetSelectedDataCells().FirstOrDefault(x => x.IsEditing);

                if (editingCell != null)
                {
                    e.Handled = true;
                    editingCell.Focus();

                    var keyEventArgs = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, Key.Return) { RoutedEvent = Keyboard.KeyDownEvent };
                    editingCell.RaiseEvent(keyEventArgs);
                }
            }
        }

        #endregion

        #region Helpers

        public DataCell GetDataCell(int column, int row)
        {
            return (dataGrid.Items[row] as DataRow)[column];
        }

        private DataCell GetDataCell(DataGridCellInfo cellInfo)
        {
            return (cellInfo.Item as DataRow)[cellInfo.Column.DisplayIndex];
        }

        private void SelectFirstCell()
        {
            DataGridCellInfo firstCellInfo = new DataGridCellInfo(dataGrid.Items[0], dataGrid.Columns[0]);

            dataGrid.UnselectAllCells();
            dataGrid.SelectedCells.Add(firstCellInfo);
            dataGrid.CurrentCell = firstCellInfo;
        }

        #endregion

        #endregion

        #region Nested types

        public class DataTable
        {
            public IExpressionTreeBuilder<Object> ExpressionTreeBuilder;

            public List<DataRow> Rows { get; private set; }

            public DataCell this[int row, int col]
            {
                get
                {
                    return Rows[row][col];
                }
            }


            public DataTable(IExpressionTreeBuilder<Object> expressionTreeBuilder)
            {
                Rows = new List<DataRow>();
                ExpressionTreeBuilder = expressionTreeBuilder;
            }
        }

        public class DataRow
        {
            private DataCell[] _cells;

            public int RowNumber { get; set; }

            public DataCell this[int i]
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

            public DataRow(DataTable dataTable, int cellCount)
            {
                _cells = new DataCell[cellCount];

                for (int i = 0; i < cellCount; i++)
                    _cells[i] = new DataCell(dataTable);
            }
        }


        public abstract class DataUnit : ViewModelBase
        {
            #region Fields

            private static Random _rand = new Random();

            #endregion

            #region Properties

            public virtual DataTable DataTable { get; protected set; }

            public virtual string Expression { get; set; }

            public abstract Object CachedValue { get; }

            #endregion

            #region Methods

            protected Object EvaluateExpression()
            {
                string expression = Expression;

                // Preparation variables
                IDictionary<string, Object> vars = new Dictionary<string, object>();

                MatchCollection referenceMatches = Regex.Matches(expression, Helpers.A1ReferenceHelper.A1ReferenceRangeOfCellsPattern);
                List<string> references = new List<string>();

                foreach (Match match in referenceMatches)
                {
                    string reference = match.Value;

                    if (!references.Contains(reference))
                        references.Add(reference);
                }

                references.Sort(StringLengthComparison);

                foreach (string reference in references)
                {
                    string varName = GetRandomVariableName();
                    vars.Add(varName, GetRangeOfCellValues(reference));
                    expression = expression.Replace(reference, varName);
                }

                referenceMatches = Regex.Matches(expression, Helpers.A1ReferenceHelper.A1ReferenceSingleCellPattern);
                references.Clear();

                foreach (Match match in referenceMatches)
                {
                    string reference = match.Value;

                    if (!references.Contains(reference))
                        references.Add(reference);
                }

                references.Sort(StringLengthComparison);

                foreach (string reference in references)
                {
                    string varName = GetRandomVariableName();
                    vars.Add(varName, GetSingleCellValue(reference));
                    expression = expression.Replace(reference, varName);
                }

                // Evaluation the expression
                Expression<Object> expr = DataTable.ExpressionTreeBuilder.BuildTree(expression);
                expr.SetAllVariables(vars);
                return expr.Evaluate();
            }


            private static string GetRandomVariableName()
            {
                return GetRandomVariableName(15);
            }

            private static string GetRandomVariableName(int length)
            {
                string name = String.Empty;

                for (int i = 0; i < length; i++)
                {
                    bool upperCase = (_rand.Next(0, 2) == 0) ? false : true;

                    if (upperCase)
                        name += (Char)('A' + _rand.Next(0, 26));
                    else
                        name += (Char)('a' + _rand.Next(0, 26));
                }

                return name;
            }

            private int StringLengthComparison(string str1, string str2)
            {
                int len1 = str1.Length;
                int len2 = str2.Length;

                if (len1 < len2) return 1;
                else if (len1 > len2) return -1;
                else return 0;
            }

            private Complex GetSingleCellValue(string a1Reference)
            {
                int row, column;
                Helpers.A1ReferenceHelper.Parse(a1Reference, out column, out row);

                return (Complex)DataTable[row, column].CachedValue;
            }

            private CMatrix GetRangeOfCellValues(string a1Reference)
            {
                int row1Idx, col1Idx, row2Idx, col2Idx;
                Helpers.A1ReferenceHelper.Parse(a1Reference, out col1Idx, out row1Idx, out col2Idx, out row2Idx);

                int n = row2Idx - row1Idx;
                int m = col2Idx - col1Idx;

                CMatrix matrix = new CMatrix(n, m);

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < m; j++)
                    {
                        if (DataTable[i + row1Idx, j + col1Idx].CachedValue != null)
                            matrix[i, j] = (Complex)DataTable[i + row1Idx, j + col1Idx].CachedValue;
                        else
                            matrix[i, j] = Complex.Zero;
                    }
                }

                return matrix;
            }

            #endregion
        }

        public class DataArray : DataUnit
        {
            #region Fields

            private string _expression;
            private Object _cachedValue;

            #endregion

            #region Properties

            public override string Expression
            {
                get
                {
                    return _expression;
                }

                set
                {
                    _expression = value;

                }
            }

            public override Object CachedValue
            {
                get
                {
                    return _cachedValue ?? EvaluateExpression();
                }
            }

            public DataCell[,] Array { get; set; }

            #endregion


            public DataArray(DataCell currentCell, int x, int y, int rows, int cols)
            {
                Expression = currentCell.Expression;
                DataTable = currentCell.DataTable;
                Array = new DataCell[rows, cols];

                for (int row = 0; row < rows; row++)
                {
                    for (int col = 0; col < cols; col++)
                    {
                        Array[row, col] = currentCell.DataTable[y + row, x + col];
                        Array[row, col].Parent = this;
                        Array[row, col].RefreshValue();
                    }
                }
            }


            public object FindValue(DataCell cell, Object cachedValue)
            {
                if (cachedValue is CMatrix)
                {
                    CMatrix matrix = cachedValue as CMatrix;
                    for (int i = 0; i < Array.GetLength(0); i++)
                    {
                        for (int j = 0; j < Array.GetLength(1); j++)
                        {
                            if (this.Array[i, j] == cell)
                            {
                                return matrix[i, j];
                            }
                        }
                    }
                }
                else if (cachedValue is Complex)
                {
                    return cachedValue;
                }

                throw new ArgumentException();
            }

            public virtual void Expand(int x, int y, int rows, int cols)
            {

            }
        }

        public class DataCell : DataUnit
        {
            #region Fields

            private static readonly string CellErrorText = "#ERROR";

            private string _expression;
            private Object _cachedValue;

            #endregion

            #region Properties

            public DataArray Parent { get; set; }

            public override Object CachedValue
            {
                get
                {
                    if (Parent == null)
                    {
                        Object value = _cachedValue ?? EvaluateExpression();
                        if (value is CMatrix)
                        {
                            return ((CMatrix)value)[0, 0];
                        }
                        return value;
                    }

                    return Parent.FindValue(this, Parent.CachedValue);
                }
            }

            public string FormattedValue
            {
                get
                {
                    if (!String.IsNullOrEmpty(Expression))
                    {
                        return ValueToStirng(CachedValue);
                    }
                    return null;
                }
            }

            public override string Expression
            {
                get
                {
                    return (Parent == null) ? _expression : Parent.Expression;
                }

                set
                {
                    if (Parent == null)
                    {
                        _expression = value;
                        _cachedValue = null;
                    }
                    else
                    {
                        Parent.Expression = value;
                    }

                    RaisePropertyChanged(() => Expression);
                    RefreshValue();
                }
            }

            #endregion

            #region Constructors

            public DataCell(DataTable dataTable)
            {
                DataTable = dataTable;
            }

            #endregion

            #region Methods

            public void Clear()
            {
                Expression = null;
            }

            public void RefreshValue()
            {
                RaisePropertyChanged(() => FormattedValue);
            }

            private string ValueToStirng(Object value)
            {
                Object normilizedValue = null;

                // Normalize the result
                if (value is Complex)
                {
                    normilizedValue = NumericUtil.ComplexZeroThreshold((Complex)value, Properties.Settings.Default.ComplexThreshold, Properties.Settings.Default.ZeroThreshold);
                }
                else if (value is CMatrix)
                {
                    normilizedValue = TAlex.MathCore.LinearAlgebra.NumericUtilExtensions.ComplexZeroThreshold((CMatrix)value, Properties.Settings.Default.ComplexThreshold, Properties.Settings.Default.ZeroThreshold);
                }

                return normilizedValue.ToString();
            }

            #endregion
        }

        #endregion
    }
}
