using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TAlex.PowerCalc.Helpers
{
    public static class A1ReferenceHelper
    {
        #region Fields

        private const string A1SingleCellReferencePattern = @"(?<col>[A-Z]+)(?<row>[0-9]+)";
        private const string A1CellRangeReferencePattern = "(?<first>((?<col>[A-Z]+)(?<row>[0-9]+))):(?<last>((?<col>[A-Z]+)(?<row>[0-9]+)))";

        private static readonly Regex A1SingleCellReferenceRegex = new Regex(A1SingleCellReferencePattern, RegexOptions.Compiled);
        private static readonly Regex A1CellRangeReferenceRegex = new Regex(A1CellRangeReferencePattern, RegexOptions.Compiled);

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
            return IntegerToA1ReferenceColumn(column) + (row + 1);
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

            return String.Format("{0}{1}:{2}{3}", IntegerToA1ReferenceColumn(column1), row1 + 1, IntegerToA1ReferenceColumn(column2), row2 + 1);
        }

        public static void Parse(string a1Reference, out int column, out int row)
        {
            Match addr = A1SingleCellReferenceRegex.Match(a1Reference);

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
            Match match = A1CellRangeReferenceRegex.Match(a1Reference);

            if (match.Success)
            {
                string first = match.Groups["first"].Value;
                string last = match.Groups["last"].Value;

                Parse(first, out column1, out row1);
                Parse(last, out column2, out row2);
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public static bool Within(int row, int column, string address)
        {
            Match match = A1CellRangeReferenceRegex.Match(address);
            if (match.Success)
            {
                int row1, column1, row2, column2;
                string first = match.Groups["first"].Value;
                string last = match.Groups["last"].Value;

                Parse(first, out column1, out row1);
                Parse(last, out column2, out row2);

                return row >= row1 && row <= row2 && column >= column1 && column <= column2; 
            }
            else
            {
                int row1, column1;
                Parse(address, out column1, out row1);

                return column == column1 && row == row1;
            }
        }

        public static List<string> GetUniqueCellRangeReferences(string expression)
        {
            return A1CellRangeReferenceRegex.Matches(expression)
                .Cast<Match>().Select(x => x.Value).Distinct().OrderByDescending(x => x.Length).ToList();
        }

        public static List<string> GetUniqueSingleCellReferences(string expression)
        {
            return A1SingleCellReferenceRegex.Matches(expression)
                .Cast<Match>().Select(x => x.Value).Distinct().OrderByDescending(x => x.Length).ToList();
        }

        #endregion
    }
}
