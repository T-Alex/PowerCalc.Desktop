using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using TAlex.MathCore.ExpressionEvaluation;

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

        public static string FormattingExpressions(string expression)
        {
            expression = expression.Trim();
            expression = expression.Replace(" ", String.Empty);
            expression = expression.Replace("\t", String.Empty);

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < expression.Length; i++)
            {
                if (("*/".IndexOf(expression[i]) != -1) ||
                    (i > 0 && "-+".IndexOf(expression[i]) != -1 && "({,".IndexOf(expression[i - 1]) == -1))
                {
                    result.AppendFormat(" {0} ", expression[i]);
                }
                else if (",;:".IndexOf(expression[i]) != -1)
                {
                    result.AppendFormat("{0} ", expression[i]);
                }
                else
                {
                    result.Append(expression[i]);
                }
            }

            return result.ToString();
        }

        public static Dictionary<string, Object> CalculateVariables(string[] lines)
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
                            ComplexMathEvaluator evaluator = new ComplexMathEvaluator(match.Groups["expr"].Value, vars);
                            vars[varName] = evaluator.Evaluate();
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
