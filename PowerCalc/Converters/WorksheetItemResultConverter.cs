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
            int maxRows = Properties.Settings.Default.WorksheetMaxMatrixRows;
            int maxCols = Properties.Settings.Default.WorksheetMaxMatrixColumns;
            int rows = Math.Min(maxRows + 1, m.RowCount);
            int cols = Math.Min(maxCols + 1, m.ColumnCount);

            string[,] values = new string[rows, cols];
            int[] maxLengths = new int[cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if ((i >= rows - 1 && maxRows < m.RowCount) || (j >= cols - 1 && maxCols < m.ColumnCount)) values[i, j] = "…";
                    else values[i, j] = m[i, j].ToString(format, CultureInfo.InvariantCulture);
                    maxLengths[j] = Math.Max(values[i, j].Length, maxLengths[j]);
                }
            }

            StringBuilder result = new StringBuilder();
            
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    string stringFormat = String.Format("{{0,{0}}}", -(maxLengths[j] + 2));
                    result.AppendFormat(stringFormat, values[i, j]);

                }
                if (i < rows - 1) result.AppendLine();
            }

            return result.ToString();
        }
    }
}
