using System;
using System.Text;
using System.Text.RegularExpressions;

namespace TAlex.PowerCalc.Helpers
{
    public static class A1ReferenceHelper
    {
        #region Fields

        public const string A1ReferenceSingleCellPattern = @"(?<col>[A-Z]+)(?<row>[0-9]+)";
        public const string A1ReferenceRangeOfCellsPattern = "(?<first>((?<col>[A-Z]+)(?<row>[0-9]+))):(?<last>((?<col>[A-Z]+)(?<row>[0-9]+)))";

        #endregion

        #region Methods

        /// <summary>
        /// Converts an integer that represents a column index to the corresponding letters.
        /// Note columns start at 0. E.g. IntegerToA1ReferenceColumn(27)="AB"
        /// </summary>
        /// <param name="integer">An integer.</param>
        /// <returns></returns>
        public static string IntegerToA1ReferenceColumn(long integer)
        {
            if (integer < 0)
                throw new ArgumentOutOfRangeException("integer");

            string str = String.Empty;

            while (integer >= 0)
            {
                str = (Char)('A' + (integer % 26)) + str;
                integer = (integer / 26) - 1;
            }

            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceColumn"></param>
        /// <returns></returns>
        public static long IntegerFromA1ReferenceColumn(string referenceColumn)
        {
            if (String.IsNullOrEmpty(referenceColumn))
                throw new ArgumentException();

            long integer = 0;

            int a = 1;
            for (int i = referenceColumn.Length - 1; i >= 0; i--)
            {
                Char ch = referenceColumn[i];

                if (ch < 'A' || ch > 'Z')
                    throw new ArgumentOutOfRangeException("referenceColumn");

                integer += ((int)(ch - 'A') + 1) * a;
                a = a * 26;
            }

            return integer - 1;
        }

        public static string ToString(int column, int row)
        {
            return IntegerToA1ReferenceColumn(column) + row;
        }

        public static string ToString(int column1, int row1, int column2, int row2)
        {
            if (row1 > row2)
            {
                int temp = row1;
                row1 = row2;
                row2 = temp;
            }

            if (column1 > column2)
            {
                int temp = column1;
                column1 = column2;
                column2 = temp;
            }

            return String.Format("{0}{1}:{2}{3}", IntegerToA1ReferenceColumn(column1), row1, IntegerToA1ReferenceColumn(column2), row2);
        }

        public static void Parse(string a1Reference, out int column, out int row)
        {
            Match addr = Regex.Match(a1Reference, A1ReferenceSingleCellPattern);

            if (addr.Success)
            {
                column = (int)IntegerFromA1ReferenceColumn(addr.Groups["col"].Value);
                row = int.Parse(addr.Groups["row"].Value) - 1;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static void Parse(string a1Reference, out int column1, out int row1, out int column2, out int row2)
        {
            Match match = Regex.Match(a1Reference, A1ReferenceRangeOfCellsPattern);

            if (match.Success)
            {
                string first = match.Groups["first"].Value;
                string last = match.Groups["last"].Value;

                Parse(first, out column1, out row1);
                Parse(last, out column2, out row2);
                column2++;
                row2++;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        #endregion
    }
}
