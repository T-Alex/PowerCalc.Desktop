using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TAlex.MathCore.ExpressionEvaluation.Trees;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;


namespace ExpressionEvaluationDemo
{
    class Program
    {
        private static readonly Regex _exprRegex = new Regex(@"^(?<var>[_a-zA-Z][_0-9a-zA-Z]*):(?<expr>.+)$", RegexOptions.Compiled);

        static void Main(string[] args)
        {
            IExpressionTreeBuilder<double> expressionBuilder = new DoubleExpressionTreeBuilder();
            IDictionary<string, double> variables = new Dictionary<string, double>();

            while (true)
            {
                Console.Write("Expression: ");
                String expression = Console.ReadLine().Trim();

                if (String.IsNullOrEmpty(expression))
                {
                    Environment.Exit(0);
                }

                Match match = _exprRegex.Match(expression);
                string varName = null;
                if (match.Success)
                {
                    varName = match.Groups["var"].Value;
                    expression = match.Groups["expr"].Value;
                }

                try
                {
                    Expression<double> exprTree = expressionBuilder.BuildTree(expression);
                    foreach (var var in exprTree.FindAllVariables())
                    {
                        var.Value = variables[var.VariableName];
                    }
                    double result = exprTree.Evaluate();

                    Console.Write("Result: ");
                    Console.Write("{0}", result);
                    Console.WriteLine();

                    if (varName != null) variables[varName] = result;
                }
                catch (Exception exc)
                {
                    Console.Write("Error: ");
                    Console.WriteLine(exc.Message);
                    Console.WriteLine();
                }
            }
        }
    }
}
