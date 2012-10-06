﻿//---------------------------------------------------------------------------
//
// (c) Copyright T-Alex Software Corporation.
// All rights reserved.
// 
// This file is part of the MathCore project.
//
//---------------------------------------------------------------------------

using System;

namespace TAlex.MathCore.EquationSolvers
{
    /// <summary>
    /// Represents the solver of equation of a real variable that uses the secant algorithm.
    /// </summary>
    /// <remarks>
    /// Secant method is a root-finding algorithm that uses a succession
    /// of roots of secant lines to better approximate a root of a function.
    /// </remarks>
    public class SecantEquationSolver : InitialGuessEquationSolver
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the SecantEquationSolver class.
        /// </summary>
        public SecantEquationSolver()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the SecantEquationSolver class
        /// with the specified target function and initial guess for the root.
        /// </summary>
        /// <param name="function">A delegate that specifies the target function.</param>
        /// <param name="initialGuess">The initial guess for the root.</param>
        public SecantEquationSolver(Function1Real function, double initialGuess)
            : base(function, initialGuess)
        {
        }

        /// <summary>
        /// Initializes a new instance of the SecantEquationSolver class
        /// with the specified target function, initial guess for the root and tolerance.
        /// </summary>
        /// <param name="function">A delegate that specifies the target function.</param>
        /// <param name="initialGuess">The initial guess for the root.</param>
        /// <param name="tolerance">The tolerance used in the convergence test.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// tolerance must be non negative.
        /// </exception>
        public SecantEquationSolver(Function1Real function, double initialGuess, double tolerance)
            : base(function, initialGuess, tolerance)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Returns the best approximation to the root of the nonlinear equation.
        /// </summary>
        /// <returns>The best approximation to the root.</returns>
        /// <exception cref="NotConvergenceException">
        /// The algorithm does not converged for a certain number of iterations.
        /// </exception>
        public override double Solve()
        {
            if (Math.Abs(Function(InitialGuess)) <= Tolerance)
            {
                IterationsNeeded = 0;
                return InitialGuess;
            }

            Function1Real func = Function;

            double a = InitialGuess;
            double b = a + 2 * Tolerance * ((Math.Abs(a) > 1.0) ? a : 1.0);

            for (int i = 0; i < MaxIterations; i++)
            {
                double x = b - func(b) * (b - a) / (func(b) - func(a));

                if (Math.Abs(func(x)) <= Tolerance)
                {
                    IterationsNeeded = i + 1;
                    return x;
                }

                a = b;
                b = x;
            }

            IterationsNeeded = -1;
            throw new NotConvergenceException();
        }

        #endregion
    }
}
