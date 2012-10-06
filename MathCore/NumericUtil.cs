﻿//---------------------------------------------------------------------------
//
// (c) Copyright T-Alex Software Corporation.
// All rights reserved.
// 
// This file is part of the MathCore project.
//
//---------------------------------------------------------------------------

using System;

using TAlex.MathCore.LinearAlgebra;

namespace TAlex.MathCore
{
    /// <summary>
    /// Represents various numerical utilities.
    /// </summary>
    public static class NumericUtil
    {
        #region Fields

        private const int MaxComplexZeroThreshold = 307;

        #endregion

        #region Methods

        /// <summary>
        /// Returns are much larger the real or imaginary part of a complex number.
        /// If the ratio of real and imaginary parts of complex number are not so large
        /// returns the initial value.
        /// </summary>
        /// <param name="value">A complex number.</param>
        /// <param name="complexThreshold">An integer representing the complex threshold.</param>
        /// <returns>
        /// Are much larger the real or imaginary part of the value.
        /// If the ratio of real and imaginary parts of the value are not so large
        /// returns the value.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// complexThreshold must be between 0 and 307.
        /// </exception>
        public static Complex ComplexThreshold(Complex value, int complexThreshold)
        {
            if (complexThreshold < 0 || complexThreshold > MaxComplexZeroThreshold)
                throw new ArgumentOutOfRangeException("complexThreshold", String.Format("Complex threshold must be between 0 and {0}.", MaxComplexZeroThreshold));

            if (value.IsReal || value.IsImaginary)
            {
                return value;
            }

            double d = Math.Pow(10, complexThreshold);

            double reAbs = Math.Abs(value.Re);
            double imAbs = Math.Abs(value.Im);

            if ((reAbs > imAbs) && (reAbs / imAbs > d))
            {
                return new Complex(value.Re, 0.0);
            }
            else if ((imAbs > reAbs) && (imAbs / reAbs > d))
            {
                return new Complex(0.0, value.Im);
            }

            return value;
        }

        /// <summary>
        /// Applies the complex threshold for each element of the complex matrix and returns the result.
        /// </summary>
        /// <param name="value">A complex matrix.</param>
        /// <param name="complexThreshold">An integer representing the complex threshold.</param>
        /// <returns>
        /// The result of applying a complex threshold for each element of the complex matrix value.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// complexThreshold must be between 0 and 307.
        /// </exception>
        public static CMatrix ComplexThreshold(CMatrix value, int complexThreshold)
        {
            CMatrix matrix = new CMatrix(value.RowCount, value.ColumnCount);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                    matrix[i, j] = ComplexThreshold(value[i, j], complexThreshold);
            }

            return matrix;
        }

        /// <summary>
        /// Returns a zero value if the initial value is close to him.
        /// Otherwise, returns the initial value.
        /// </summary>
        /// <param name="value">A real number.</param>
        /// <param name="zeroThreshold">An integer representing the zero threshold.</param>
        /// <returns>
        /// A zero value if the value is close to him.
        /// Otherwise, returns the value.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// zeroThreshold must be between 0 and 307.
        /// </exception>
        public static double ZeroThreshold(double value, int zeroThreshold)
        {
            if (zeroThreshold < 0 || zeroThreshold > MaxComplexZeroThreshold)
                throw new ArgumentOutOfRangeException("zeroThreshold", String.Format("The zero threshold must be between 0 and {0}.", MaxComplexZeroThreshold));

            double d = Math.Pow(10, -zeroThreshold);
            return (Math.Abs(value) < d) ? 0.0 : value;
        }

        /// <summary>
        /// Returns a zero value if the initial value is close to him.
        /// Otherwise, returns the initial value.
        /// </summary>
        /// <param name="value">A complex number.</param>
        /// <param name="zeroThreshold">An integer representing the zero threshold.</param>
        /// <returns>
        /// A zero value if the value is close to him.
        /// Otherwise, returns the value.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// zeroThreshold must be between 0 and 307.
        /// </exception>
        public static Complex ZeroThreshold(Complex value, int zeroThreshold)
        {
            if (zeroThreshold < 0 || zeroThreshold > MaxComplexZeroThreshold)
                throw new ArgumentOutOfRangeException("zeroThreshold", String.Format("The zero threshold must be between 0 and {0}.", MaxComplexZeroThreshold));

            double d = Math.Pow(10, -zeroThreshold);

            double re = (Math.Abs(value.Re) < d) ? 0.0 : value.Re;
            double im = (Math.Abs(value.Im) < d) ? 0.0 : value.Im;

            return new Complex(re, im);
        }

        /// <summary>
        /// Applies the zero threshold for each element of the complex matrix and returns the result.
        /// </summary>
        /// <param name="value">A complex matrix.</param>
        /// <param name="zeroThreshold">An integer representing the zero threshold.</param>
        /// <returns>
        /// The result of applying a zero threshold for each element of the complex matrix value.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// zeroThreshold must be between 0 and 307.
        /// </exception>
        public static CMatrix ZeroThreshold(CMatrix value, int zeroThreshold)
        {
            CMatrix matrix = new CMatrix(value.RowCount, value.ColumnCount);

            for (int i = 0; i < matrix.RowCount; i++)
            {
                for (int j = 0; j < matrix.ColumnCount; j++)
                    matrix[i, j] = ZeroThreshold(value[i, j], zeroThreshold);
            }

            return matrix;
        }

        /// <summary>
        /// Applies the complex and zero threshold for a complex number and returns the result.
        /// </summary>
        /// <param name="value">A complex number.</param>
        /// <param name="complexThreshold">An integer representing the complex threshold.</param>
        /// <param name="zeroThreshold">An integer representing the zero threshold.</param>
        /// <returns>
        /// The result of applying a complex and zero threshold for the complex number value.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// complexThreshold and zeroThreshold must be between 0 and 307.
        /// </exception>
        public static Complex ComplexZeroThreshold(Complex value, int complexThreshold, int zeroThreshold)
        {
            return ZeroThreshold(ComplexThreshold(value, complexThreshold), zeroThreshold);
        }

        /// <summary>
        /// Applies the complex and zero threshold for each element of the complex matrix and returns the result.
        /// </summary>
        /// <param name="value">A complex matrix.</param>
        /// <param name="complexThreshold">An integer representing the complex threshold.</param>
        /// <param name="zeroThreshold">An integer representing the zero threshold.</param>
        /// <returns>
        /// The result of applying a complex and zero threshold for each element of the complex matrix value.
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// complexThreshold and zeroThreshold must be between 0 and 307.
        /// </exception>
        public static CMatrix ComplexZeroThreshold(CMatrix value, int complexThreshold, int zeroThreshold)
        {
            return ZeroThreshold(ComplexThreshold(value, complexThreshold), zeroThreshold);
        }


        public static bool FuzzyEquals(double value1, double value2, double relativeTolerance)
        {
            return Math.Abs(value1 - value2) <= relativeTolerance * Math.Max(Math.Abs(value1), Math.Abs(value2));
        }

        public static bool FuzzyEquals(Complex value1, Complex value2, double relativeTolerance)
        {
            return FuzzyEquals(value1.Re, value2.Re, relativeTolerance) && FuzzyEquals(value1.Im, value2.Im, relativeTolerance);
        }

        public static bool FuzzyEquals(CMatrix value1, CMatrix value2, double relativeTolerance)
        {
            if (value1.RowCount != value2.RowCount || value1.ColumnCount != value2.ColumnCount)
                return false;

            int rows = value1.RowCount;
            int cols = value1.ColumnCount;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (!FuzzyEquals(value1[i, j], value2[i, j], relativeTolerance))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns whether or not two doubles are "close".  That is, whether or 
        /// not they are within epsilon of each other.  Note that this epsilon is proportional
        /// to the numbers themselves to that AreClose survives scalar multiplication.
        /// There are plenty of ways for this to return false even for numbers which
        /// are theoretically identical, so no code calling this should fail to work if this 
        /// returns false.
        /// </summary>
        /// <param name="value1"> The first double to compare. </param>
        /// <param name="value2"> The second double to compare. </param>
        /// <returns>result of the AreClose comparision.</returns>
        public static bool AreClose(double value1, double value2)
        {
            // in case they are Infinities (then epsilon check does not work)
            if (value1 == value2)
            {
                return true;
            }

            // This computes (|value1-value2| / (|value1| + |value2| + 10.0)) < DBL_EPSILON
            double eps = (Math.Abs(value1) + Math.Abs(value2) + 10.0) * Machine.Epsilon;
            double delta = value1 - value2;
            return (-eps < delta) && (eps > delta);
        }

        #endregion
    }
}
