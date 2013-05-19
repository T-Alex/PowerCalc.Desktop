using System;
using System.Collections.Generic;
using TAlex.MathCore.ExpressionEvaluation.Trees;


namespace TAlex.PowerCalc.Helpers
{
    public static class ExpressionExtensions
    {
        public static void SetAllVariables<T>(this Expression<T> expression, IDictionary<string, T> variables)
        {
            foreach (var var in expression.FindAllVariables())
            {
                T value;
                if (variables.TryGetValue(var.VariableName, out value))
                {
                    var.Value = value;
                }
            }
        }
    }
}
