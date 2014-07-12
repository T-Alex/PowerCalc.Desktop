using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;
using TAlex.WPF.Mvvm.Commands;
using TAlex.PowerCalc.Helpers;
using System.Text.RegularExpressions;
using TAlex.MathCore.ExpressionEvaluation.Tokenize;


namespace TAlex.PowerCalc.ViewModels.Worksheet
{
    public class WorksheetModel
    {
        #region Fields

        public static readonly string ClearCommandString = "cls";
        public static readonly string CommentStatement = "//";
        public static readonly string[] HelpCommands = new string[] { "?", "/?", "help" };
        
        private static readonly string HelpText =
@"Commands list:
<expression> – calculate expression
<var>: <expression> – define variable
cls – clear console
// – add comments
?, /?, help – show help
";

        private static readonly string VariableNamePattern = @"(?<var>[^\W\d]\w*)";
        private static readonly string ExpressionPattern = @"(?<expr>.+)";

        private static readonly string StatementPattern = "(" + VariableNamePattern + @"\s*\:)?" + ExpressionPattern;
        private static readonly Regex StatementRegex = new Regex(StatementPattern, RegexOptions.Compiled);

        protected readonly IExpressionTreeBuilder<Object> ExpressionTreeBuilder;

        protected List<string> InputHistory = new List<string>();
        protected int InputHistoryIndex = -1;
        protected bool IsInputHistoryNavigated = false;

        #endregion

        #region Properties

        public IDictionary<string, Object> Variables { get; set; }

        public ObservableCollection<WorksheetItem> Items { get; private set; }

        public WorksheetItem EditableItem
        {
            get
            {
                return Items.Last();
            }
        }

        #endregion

        #region Events

        public event EventHandler InputHistoryNavigated;

        #endregion

        #region Commands

        public ICommand EvaluateCommand { get; set; }

        public ICommand ClearCommand { get; set; }

        public ICommand NavigateUpInputHistoryCommand { get; set; }

        public ICommand NavigateDownInputHistoryCommand { get; set; }

        #endregion

        #region Constructors

        public WorksheetModel(IExpressionTreeBuilder<Object> treeBuilder)
        {
            ExpressionTreeBuilder = treeBuilder;

            Variables = new Dictionary<string, Object>();
            Items = new ObservableCollection<WorksheetItem>();

            Initialize();
            InitializeCommands();
        }

        #endregion

        #region Methods

        public void Clear()
        {
            Variables.Clear();
            Items.Clear();
            Items.Add(new WorksheetItem());
        }

        public void Evaluate()
        {
            WorksheetItem lastItem = EditableItem;
            string expression = (lastItem.Expression + String.Empty).Trim();

            // Handle empty expression
            if (IsEmptyExpression(expression)) return;

            // Add input history if needed
            AddInputHistory(expression);

            if (IsClearScreenExpression(expression)) // Handle clear screen expression
                Clear();
            else if (IsCommentExpression(expression) || IsHelpExpression(expression)) // Handle comments and help
            {
                if (IsHelpExpression(expression)) HandleHelpExpression(lastItem);
                HandleCommentExpression(lastItem);
            }
            else
                HandleCalculatedExpression(lastItem); // Handle calculated expression
        }

        public void NavigateUpInputHistory()
        {
            if (InputHistoryIndex > -1)
            {
                if (InputHistoryIndex > 0 && IsInputHistoryNavigated) InputHistoryIndex--;
                EditableItem.Expression = InputHistory[InputHistoryIndex];

                IsInputHistoryNavigated = true;
                OnInputHistoryNavigated();
            }
        }

        public void NavigateDownInputHistory()
        {
            if (InputHistoryIndex < InputHistory.Count - 1)
            {
                InputHistoryIndex++;
                EditableItem.Expression = InputHistory[InputHistoryIndex];

                IsInputHistoryNavigated = true;
                OnInputHistoryNavigated();
            }
        }

        protected void OnInputHistoryNavigated()
        {
            if (InputHistoryNavigated != null)
            {
                InputHistoryNavigated(this, new EventArgs());
            }
        }

        protected virtual void Initialize()
        {
            Clear();
        }

        protected virtual void InitializeCommands()
        {
            EvaluateCommand = new RelayCommand(Evaluate);
            ClearCommand = new RelayCommand(Clear);
            NavigateUpInputHistoryCommand = new RelayCommand(NavigateUpInputHistory);
            NavigateDownInputHistoryCommand = new RelayCommand(NavigateDownInputHistory);
        }


        #region Helpers

        private void AddItem(WorksheetItem item)
        {
            Items[Items.IndexOf(item)] = new WorksheetItem(item.Expression, item.Result); // Refresh item
            Items.Add(new WorksheetItem()); // Insert placeholder
        }

        private bool IsEmptyExpression(string expression)
        {
            return String.IsNullOrWhiteSpace(expression);
        }

        private bool IsClearScreenExpression(string expression)
        {
            return String.Equals(ClearCommandString, expression, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsHelpExpression(string expression)
        {
            return HelpCommands.Contains(expression);
        }

        private bool IsCommentExpression(string expression)
        {
            return expression.StartsWith(CommentStatement);
        }

        private string GetCommentFromExpression(string expression)
        {
            if (IsCommentExpression(expression))
                return expression.Substring(CommentStatement.Length).Trim();
            else
                return expression.Trim();
        }

        private void HandleCommentExpression(WorksheetItem item)
        {
            WorksheetItem lastItem = item;
            string expression = lastItem.Expression.Trim();
            string comment = GetCommentFromExpression(expression);

            if (Items.Count > 1)
            {
                WorksheetItem prevItem = Items[Items.IndexOf(lastItem) - 1];
                if (prevItem.Result is String)
                {
                    comment = prevItem.Result + Environment.NewLine + comment;
                    Items.Remove(lastItem);
                    lastItem = prevItem;
                }
            }

            lastItem.Expression = String.Empty;
            lastItem.Result = comment;
            AddItem(lastItem);
        }

        private void HandleHelpExpression(WorksheetItem item)
        {
            item.Expression = HelpText;
        }

        private void HandleCalculatedExpression(WorksheetItem item)
        {
            string expression = item.Expression.Trim();
            Match match = Regex.Match(expression, StatementPattern);
            
            if (match.Success)
            {
                string expressionString = match.Groups["expr"].Value;
                
                try
                {
                    var expressionTree = ExpressionTreeBuilder.BuildTree(expressionString);
                    expressionTree.SetAllVariables(Variables);
                    item.Result = expressionTree.Evaluate();

                    string varName = match.Groups["var"].Value;
                    if (!String.IsNullOrEmpty(varName))
                    {
                        Variables[varName] = item.Result;
                        item.Result = new KeyValuePair<string, object>(varName, item.Result);
                    }
                }
                catch (Exception exc)
                {
                    item.Result = exc;
                }
            }
            else
            {
                item.Result = new SyntaxException();
            }
            
            AddItem(item);
        }


        private bool IsNeededToAddInputHistory(string expression)
        {
            return InputHistory.Count == 0 || !String.Equals(InputHistory.Last(), expression);
        }

        private void AddInputHistory(string expression)
        {
            if (IsNeededToAddInputHistory(expression))
            {
                InputHistory.Add(expression);

                if (!IsInputHistoryNavigated || (IsInputHistoryNavigated && !String.Equals(InputHistory[InputHistoryIndex], expression)))
                {
                    InputHistoryIndex = InputHistory.Count - 1;
                } 
            }
            IsInputHistoryNavigated = false;
        }

        #endregion

        #endregion
    }
}
