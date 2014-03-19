using System;
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

        private static readonly string CellErrorText = "#ERROR";

        private static Random _rand = new Random();

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

        public DataCell GetDataCell(int column, int row)
        {
            return (dataGrid.Items[row] as DataRow)[column];
        }

        private DataCell GetDataCell(DataGridCellInfo cellInfo)
        {
            return (cellInfo.Item as DataRow)[cellInfo.Column.DisplayIndex];
        }

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
                    Binding = new Binding(String.Format("[{0}].FormattedValue", i)) { ValidatesOnExceptions = true },
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

        private void dataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            if (lastEditedCell != null && !formulaBarTextBox.IsFocused)
            {
                lastEditedCell.IsEditing = false;
                lastEditedCell = null;
            }
        }

        // TODO: Need refactoring
        private void dataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            int selectedCells = dataGrid.SelectedCells.Count;

            if (selectedCells > 0)
            {
                DataGridCellInfo firstCellInfo1 = dataGrid.SelectedCells[0];
                formulaBarTextBox.SetBinding(TextBox.TextProperty, new Binding("Expression") { Source = GetDataCell(firstCellInfo1), Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });

                if (selectedCells == 1)
                {
                    nameBoxTextBox.Text = Helpers.A1ReferenceHelper.ToString(firstCellInfo1.Column.DisplayIndex, dataGrid.Items.IndexOf(firstCellInfo1.Item) + 1);
                }
                else
                {
                    DataGridCellInfo firstCellInfo = dataGrid.SelectedCells[0];
                    DataGridCellInfo lastCellInfo = dataGrid.SelectedCells[selectedCells - 1];

                    //nameBoxTextBox.Text = Helpers.A1ReferenceHelper.ToString(firstCellInfo.ColumnIndex, firstCellInfo.RowIndex + 1,
                    //    lastCellInfo.ColumnIndex, lastCellInfo.RowIndex + 1);
                }
            }
        }

        // TODO: Need refactoring
        private void dataGrid_PreparingCellForEdit(object sender, DataGridPreparingCellForEditEventArgs e)
        {
            //DataCell cell = (e.Row.Item as DataRow)[e.Column.DisplayIndex];
        }

        // TODO: Need refactoring
        private void dataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            FrameworkElement elem = e.EditingElement;
            
            if (formulaBarEdit)
            {
                e.Cancel = true;
                lastEditedCell = dataGrid.TryToFindGridCell(e.Row.Item, e.Column);
                formulaBarEdit = false;
            }

            //if (elem is TextBox)
            //{
            //    TextBox textBox = elem as TextBox;
            //    string expression = textBox.Text.Trim();
            //    DataCell dataCell = (e.Row.Item as DataRow)[e.Column.DisplayIndex];

            //    if (String.IsNullOrEmpty(expression))
            //    {
            //        dataCell.FormattedValue = null;
            //        dataCell.Expression = null;
            //        dataCell.Value = null;
            //        dataGrid.CurrentCellContainer.ToolTip = null;
            //        return;
            //    }
            //    else
            //    {
            //        dataCell.Expression = expression;

            //        try
            //        {
            //            Object result = EvaluateMatrixExpression(expression);

            //            dataGrid.CurrentCellContainer.ToolTip = null;

            //            if (Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift))
            //            {
            //                int selectedCells = dataGrid.SelectedCells.Count;

            //                if (selectedCells > 1)
            //                {
            //                    if (result is CMatrix)
            //                    {
            //                        string format = Properties.Settings.Default.NumericFormat;
            //                        DataGridCellInfo firstCell = dataGrid.SelectedCells[0];
            //                        DataGridCellInfo lastCell = dataGrid.SelectedCells[selectedCells - 1];

            //                        CMatrix m = (CMatrix)result;

            //                        int firstSelectedItemIndex = dataGrid.Items.IndexOf(firstCell.Item);
            //                        int lastSelectedItemIndex = dataGrid.Items.IndexOf(lastCell.Item);
            //                        int firstSelectedColumnIndex = firstCell.Column.DisplayIndex;
            //                        int lastSelectedColumnIndex = lastCell.Column.DisplayIndex;

            //                        int rows = Math.Min(m.RowCount, Math.Abs(firstSelectedItemIndex - lastSelectedItemIndex) + 1);
            //                        int cols = Math.Min(m.ColumnCount, Math.Abs(firstSelectedColumnIndex - lastSelectedColumnIndex) + 1);

            //                        int row_offset = Math.Min(firstSelectedItemIndex, lastSelectedItemIndex);
            //                        int col_offset = Math.Min(firstSelectedColumnIndex, lastSelectedColumnIndex);

            //                        for (int i = 0; i < rows; i++)
            //                        {
            //                            for (int j = 0; j < cols; j++)
            //                            {
            //                                Complex num = m[i, j];

            //                                DataGridCellInfo currCellInfo = new DataGridCellInfo(dataGrid.Items[row_offset + i], dataGrid.Columns[col_offset + j]);
            //                                currCellInfo.Value = num.ToString(format, CultureInfo.InvariantCulture);

            //                                DataCell dataCell2 = GetDataCell(currCellInfo);
            //                                dataCell2.Value = num;

            //                                if (dataGrid.CurrentCellContainer.Column.DisplayIndex == currCellInfo.ColumnIndex &&
            //                                    dataGrid.CurrentCellContainer.RowDataItem == currCellInfo.Item)
            //                                    continue;

            //                                dataCell2.FormattedValue = num.ToString(Properties.Settings.Default.NumericFormat, CultureInfo.InvariantCulture);
            //                                dataCell2.Expression = num.ToString(CultureInfo.InvariantCulture);
            //                            }
            //                        }

            //                        return;
            //                    }
            //                }
            //            }

            //            dataCell.Value = result;
            //            dataCell.FormattedValue = ((IFormattable)result).ToString(Properties.Settings.Default.NumericFormat, CultureInfo.InvariantCulture);
            //            textBox.Text = dataCell.FormattedValue;
            //        }
            //        catch (Exception exc)
            //        {
            //            dataCell.FormattedValue = CellErrorText;
            //            dataCell.Value = null;
            //            textBox.Text = dataCell.FormattedValue;

            //            dataGrid.CurrentCellContainer.ToolTip = exc.Message;
            //        }
            //    }
            //}
        }

        // TODO: Need refactoring
        private void dataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    //if (!dataGrid.CurrentCellContainer.IsEditing)
                    //{
                    //    int selectedCells = dataGrid.SelectedCells.Count;

                    //    for (int i = 0; i < selectedCells; i++)
                    //    {
                    //        DataGridCellInfo cellInfo = dataGrid.SelectedCells[i];
                    //        DataCell dataCell = GetDataCell(cellInfo);
                    //        dataCell.Clear();
                    //    }

                    //    formulaBarTextBox.Text = String.Empty;
                    //}

                    break;
            }
        }

        bool formulaBarEdit = false;
        DataGridCell lastEditedCell = null;

        // TODO: Need refactoring
        private void formulaBarTextBox_IsKeyboardFocusedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            bool? focused = e.NewValue as bool?;
            
            if (focused == true)
            {
                if (dataGrid.SelectedCells.Count == 0)
                {
                    SelectFirstCell();
                }

                formulaBarEdit = true;
                dataGrid.BeginEdit();
                formulaBarTextBox.Focus();
            }
        }

        // TODO: Need refactoring
        private void formulaBarTextBox_KeyDown(object sender, KeyEventArgs e)
        { 
            switch (e.Key)
            {
                case Key.Enter:
                    dataGrid.GetCurrentDataGridCell().Focus(); //dataGrid.Focus(); //dataGrid.CurrentCellContainer.Focus();
                    dataGrid.RaiseEvent(new KeyEventArgs(e.KeyboardDevice, e.InputSource, e.Timestamp, Key.Enter)); //dataGrid.OnEnterKeyDown(e);
                    break;
            }
        }

        #endregion

        #region Helpers

        // TODO: Need refactoring
        private void SelectFirstCell()
        {
            DataGridCellInfo firstCellInfo = new DataGridCellInfo(dataGrid.Items[0], dataGrid.Columns[0]);
            
            //dataGrid.CurrentCellContainer = dataGrid.TryFindCell(firstCellInfo.Item, firstCellInfo.Column);
            dataGrid.SelectedCells.Add(firstCellInfo);
        }

        #endregion

        #endregion

        #region Nested types

        public class DataTable
        {
            public IExpressionTreeBuilder<Object> ExpressionTreeBuilder;

            public List<DataRow> Rows { get; private set; }


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

        public class DataCell
        {
            #region Fields

            private string _expression;

            #endregion

            #region Properties

            public DataTable DataTable { get; private set; }

            public Object Value { get; set; }

            public string FormattedValue
            {
                get
                {
                    if (!String.IsNullOrEmpty(Expression))
                    {
                        return EvaluateExpression(Expression).ToString();
                    }
                    return null;
                }
            }

            public string Expression
            {
                get;
                set;
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
                Value = null;
            }

            private Object EvaluateExpression(string expression)
            {
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
                Object obj = expr.Evaluate();

                // Normalize the result
                if (obj is Complex)
                {
                    obj = NumericUtil.ComplexZeroThreshold((Complex)obj, Properties.Settings.Default.ComplexThreshold, Properties.Settings.Default.ZeroThreshold);
                }
                else if (obj is CMatrix)
                {
                    obj = TAlex.MathCore.LinearAlgebra.NumericUtilExtensions.ComplexZeroThreshold((CMatrix)obj, Properties.Settings.Default.ComplexThreshold, Properties.Settings.Default.ZeroThreshold);
                }

                return obj;
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

                return (Complex)DataTable.Rows[row][column].Value;
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
                        if (DataTable.Rows[i + row1Idx][j + col1Idx].Value != null)
                            matrix[i, j] = (Complex)DataTable.Rows[i + row1Idx][j + col1Idx].Value;
                        else
                            matrix[i, j] = Complex.Zero;
                    }
                }

                return matrix;
            }

            #endregion
        }

        #endregion
    }
}
