using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using TAlex.MathCore;
using TAlex.MathCore.LinearAlgebra;
using TAlex.PowerCalc.Properties;
using TAlex.WPF.Converters;


namespace TAlex.PowerCalc.Converters
{
    public class WorksheetItemResultConverter : ConverterBase<WorksheetItemResultConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Exception)
            {
                return ((Exception)value).Message;
            }
            else if (value is KeyValuePair<string, Object>)
            {
                var pair = (KeyValuePair<string, Object>)value;
                return String.Format(Resources.VariableValueAssigned, pair.Key) +
                    (pair.Value is CMatrix ? Environment.NewLine : " ") + ValueToString(pair.Value as IFormattable);
            }
            else if (value is IFormattable)
            {
                return ValueToString(value as IFormattable);
            }

            return value;
        }

        private string ValueToString(IFormattable value)
        {
            Properties.Settings settings = Properties.Settings.Default;

            object result = null;

            if (value is Complex) result = NumericUtil.ComplexZeroThreshold((Complex)value, settings.ComplexThreshold, settings.ZeroThreshold);
            else if (value is CMatrix) result = NumericUtilExtensions.ComplexZeroThreshold((CMatrix)value, settings.ComplexThreshold, settings.ZeroThreshold);

            if (result is CMatrix)
            {
                return MatrixToString(result as CMatrix, settings.NumericFormat);
            }
            return ((IFormattable)result).ToString(settings.NumericFormat, CultureInfo.InvariantCulture);
        }

        private string MatrixToString(CMatrix m, string format)
        {
            string[,] values = new string[m.RowCount, m.ColumnCount];
            int[] maxLengths = new int[m.ColumnCount];
            for (int i = 0; i < m.RowCount; i++)
            {
                for (int j = 0; j < m.ColumnCount; j++)
                {
                    values[i, j] = m[i, j].ToString(format, CultureInfo.InvariantCulture);
                    maxLengths[j] = Math.Max(values[i, j].Length, maxLengths[j]);
                }
            }

            StringBuilder result = new StringBuilder();
            
            for (int i = 0; i < m.RowCount; i++)
            {
                for (int j = 0; j < m.ColumnCount; j++)
                {
                    string stringFormat = String.Format("{{0,{0}}}", -(maxLengths[j] + 2));
                    result.AppendFormat(stringFormat, values[i, j]);

                }
                if (i < m.RowCount - 1) result.AppendLine();
            }

            return result.ToString();
        }
    }
}
