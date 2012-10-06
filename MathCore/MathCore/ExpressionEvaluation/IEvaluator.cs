//---------------------------------------------------------------------------
//
// (c) Copyright T-Alex Software Corporation.
// All rights reserved.
//
// This file is part of the MathCore project.
//
//---------------------------------------------------------------------------

using System;

namespace TAlex.MathCore.ExpressionEvaluation
{
    /// <summary>
    /// Defines a method to evaluate expressions.
    /// </summary>
    public interface IEvaluator<T>
    {
        /// <summary>
        /// Returns the result of evaluation expression.
        /// </summary>
        /// <returns>An object containing the result of evaluation.</returns>
        T Evaluate();
    }
}
