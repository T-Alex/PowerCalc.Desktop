using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TAlex.MathCore.ExpressionEvaluation;
using TAlex.MathCore.ExpressionEvaluation.Trees;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;

namespace TAlex.PowerCalc.Helpers
{
    static class WorksheetHelper
    {
        #region Fields

        public const string SymbolDefinition = ":";

        public const string SymbolEquality = "=";

        private const string VariableNamePattern = @"(?<var>[a-zA-Z_][a-zA-Z_0-9]*)";

        private const string ExpressionPattern = @"(?<expr>[^=]+)";

        private const string LinePattern = "(" + VariableNamePattern + @"\s*\:)?" + ExpressionPattern;

        #endregion

        #region Methods

        public static Match GetMatch(string line)
        {
            return Regex.Match(line, LinePattern);
        }

        public static bool SplitLine(string line, out string varName, out string expression)
        {
            Match match = Regex.Match(line, LinePattern);

            if (match.Success)
            {
                varName = match.Groups["var"].Value;
                expression = match.Groups["expr"].Value.Trim();
            }
            else
            {
                varName = String.Empty;
                expression = String.Empty;
            }

            return match.Success;
        }

        public static Dictionary<string, Object> CalculateVariables(IExpressionTreeBuilder<Object> builder, string[] lines)
        {
            Dictionary<string, Object> vars = new Dictionary<string, Object>();

            for (int i = 0; i < lines.Length; i++)
            {
                Match match = Regex.Match(lines[i], LinePattern);

                if (match.Success)
                {
                    string varName = match.Groups["var"].Value;

                    if (varName != String.Empty)
                    {
                        try
                        {
                            Expression<Object> expression = builder.BuildTree(match.Groups["expr"].Value);
                            foreach (var var in expression.FindAllVariables())
                            {
                                object value;
                                if (vars.TryGetValue(var.VariableName, out value))
                                {
                                    var.Value = value;
                                }
                            }
                            
                            //ComplexMathEvaluator evaluator = new ComplexMathEvaluator(, vars);
                            
                            vars[varName] = expression.Evaluate();
                        }
                        catch (Exception)
                        {
                            vars[varName] = null;
                        }
                    }
                }
            }

            return vars;
        }

        #endregion
    }
}
