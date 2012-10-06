using System;
using System.Globalization;

using TAlex.MathCore.LinearAlgebra;

namespace TAlex.MathCore.ExpressionEvaluation
{
    /// <summary>
    /// Converts data type to another data type.
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// Convert object ot integer number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt32(Object value)
        {
            if (value is Complex && ((Complex)value).IsReal)
            {
                double d = ((Complex)value).Re;

                if (!ExMath.IsInt32(d))
                    throw new SyntaxException(String.Format("The number {0} is not an integer", d));
                else
                    return (int)d;
            }
            else if (value is int)
            {
                return (int)value;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Convert object to real number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToDouble(Object value)
        {
            if (value is Complex && ((Complex)value).IsReal)
                return ((Complex)value).Re;
            else if (value is Double)
                return (double)value;
            else if (value is int)
                return (int)value;
            else
                throw new SyntaxException(String.Format("The number {0} is not an real", value));
        }

        /// <summary>
        /// Convert string to real number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static double ToDouble(string value)
        {
            string str = value;

            double result = 0.0;

            if (Double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
            {
                return result;
            }


            if (str.Length < 2)
                throw new SyntaxException();

            int radix;
            switch (str[str.Length - 1])
            {
                case 'b': radix = 2; break;
                case 'o': radix = 8; break;
                case 'h': radix = 16; break;

                default:
                    throw new FormatException();
            }
            str = str.Remove(str.Length - 1);

            int dot = str.IndexOf('.');

            if (dot == -1)
                dot = 0;
            else
            {
                str = str.Remove(dot, 1);
                dot = -(str.Length - dot);
            }

            int idx = dot;
            str = str.ToUpper();
            for (int i = str.Length - 1; i >= 0; i--)
            {
                int digit;
                if (Char.IsDigit(str[i]))
                    digit = Int32.Parse(str[i].ToString());
                else if (Char.IsLetter(str[i]))
                    digit = 10 + (str[i] - 'A');
                else
                    throw new SyntaxException();

                if (digit >= radix)
                    throw new SyntaxException(String.Format("The number {0} contains an invalid character '{1}'", value, str[i]));

                result += digit * Math.Pow(radix, idx++);
            }

            return result;
        }

        /// <summary>
        /// Convert object to complex number.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Complex ToComplex(Object value)
        {
            if (value is Complex)
                return (Complex)value;
            else if (value is Double)
                return (Double)value;
            else if (value is int)
                return (int)value;
            else
                throw new SyntaxException();
        }

        public static CMatrix ToCMatrix(double[] value)
        {
            return value;
        }

        public static CMatrix ToCMatrix(Complex[] value)
        {
            return value;
        }

        public static CMatrix ToCMatrix(CPolynomial value)
        {
            CMatrix result = new CMatrix(value.Length);

            for (int i = 0; i < result.Length; i++)
                result[i] = value[i];

            return result;
        }

        public static Complex[] ToComplexArray(CMatrix value)
        {
            if (!value.IsVector)
                throw new ArgumentException();

            Complex[] result = new Complex[value.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = value[i];

            return result;
        }

        public static Complex[] ToComplexArray(CMatrix value, bool expandToRow)
        {
            if (expandToRow == false)
            {
                return ToComplexArray(value);
            }
            else
            {
                int idx = 0;
                Complex[] result = new Complex[value.Length];

                for (int i = 0; i < value.RowCount; i++)
                {
                    for (int j = 0; j < value.ColumnCount; j++)
                    {
                        result[idx] = value[i, j];
                        idx++;
                    }
                }

                return result;
            }
        }

        public static Complex[] ToComplexArray(Object value)
        {
            Complex[] result = null;

            if (value is CMatrix)
            {
                return ToComplexArray((CMatrix)value);
            }
            else if (value is Complex[])
            {
                result = (Complex[])value;
            }

            return result;
        }

        public static double[] ToDoubleArray(Object value)
        {
            double[] result = null;

            if (value is CMatrix)
            {
                CMatrix m = (CMatrix)value;

                if (!m.IsVector || !m.IsReal)
                    return null;

                result = new double[m.Length];

                for (int i = 0; i < result.Length; i++)
                    result[i] = m[i].Re;
            }
            else if (value is double[])
            {
                result = (double[])value;
            }

            return result;
        }

        public static double[] ToDoubleArray(CMatrix value, bool expandToRow)
        {
            if (expandToRow == false)
            {
                return ToDoubleArray(value);
            }
            else
            {
                int idx = 0;
                double[] result = new double[value.Length];

                for (int i = 0; i < value.RowCount; i++)
                {
                    for (int j = 0; j < value.ColumnCount; j++)
                    {
                        if (!value[i, j].IsReal)
                            throw new ArgumentException();

                        result[idx] = value[i, j].Re;
                        idx++;
                    }
                }

                return result;
            }
        }

        public static int[] ToInt32Array(Object[] value)
        {
            int[] array = new int[value.Length];

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = ToInt32(value[i]);
            }

            return array;
        }

        public static CPolynomial ToCPolynomial(Object obj)
        {
            if (obj is CMatrix)
            {
                CMatrix m = obj as CMatrix;
                return ToCPolynomial(m);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static CPolynomial ToCPolynomial(CMatrix m)
        {
            if (m.IsVector)
            {
                CPolynomial poly = new CPolynomial(m.Length);

                for (int i = 0; i < poly.Length; i++)
                    poly[i] = m[i];

                return poly;
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }
}
