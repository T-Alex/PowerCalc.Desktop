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

        protected readonly IExpressionTreeBuilder<Object> ExpressionTreeBuilder;

        #endregion

        #region Properties

        public IDictionary<string, Object> Variables { get; set; }

        public ObservableCollection<WorksheetItem> Items { get; private set; }

        #endregion

        #region Commands

        public ICommand EvaluateCommand { get; set; }

        public ICommand ClearCommand { get; set; }

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
            WorksheetItem lastItem = Items.Last();
            string expression = lastItem.Expression.Trim();

            // Handle empty expression
            if (IsEmptyExpression(expression)) return;

            // Handle clear screen expression
            if (IsClearScreenExpression(expression))
            {
                Clear();
                return;
            }

            // Handle comments
            if (IsCommentExpression(expression))
            {
                HandleCommentExpression(lastItem);
                return;
            }

            Match match = Helpers.WorksheetHelper.GetMatch(expression);
            if (!match.Success)
                throw new SyntaxException();
            string expressionString = match.Groups["expr"].Value;

            //------------------
            var expressionTree = ExpressionTreeBuilder.BuildTree(expressionString);
            expressionTree.SetAllVariables(Variables);

            try
            {
                lastItem.Result = expressionTree.Evaluate();
                string varName = match.Groups["var"].Value;
                if (!String.IsNullOrEmpty(varName))
                {
                    Variables[varName] = lastItem.Result;
                }
            }
            catch (Exception exc)
            {
                lastItem.Result = exc;
            }

            AddItem(lastItem);
        }


        protected virtual void Initialize()
        {
            Clear();
        }

        protected virtual void InitializeCommands()
        {
            EvaluateCommand = new RelayCommand(Evaluate);
            ClearCommand = new RelayCommand(Clear);
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

        private bool IsCommentExpression(string expression)
        {
            return expression.StartsWith(CommentStatement);
        }

        private string GetCommentFromExpression(string expression)
        {
            return expression.Substring(CommentStatement.Length).Trim();
        }

        private void HandleCommentExpression(WorksheetItem item)
        {
            WorksheetItem lastItem = item;
            string expression = lastItem.Expression.Trim();
            string comment = GetCommentFromExpression(expression);

            if (Items.Count > 2)
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

        #endregion

        #endregion
    }
}
