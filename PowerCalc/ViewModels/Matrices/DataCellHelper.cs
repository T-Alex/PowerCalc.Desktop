using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TAlex.PowerCalc.ViewModels.Matrices
{
    public static class DataCellHelper
    {
        public static List<DataArray> GetAllUniqueArrays(IList<DataCell> dataCells)
        {
            return dataCells.Select(x => x.Parent).Where(x => x != null).Distinct().ToList();
        }

        public static void EnsureAllArraysEnclosing(IList<DataCell> dataCells)
        {
            IList<DataArray> allArrays = GetAllUniqueArrays(dataCells);
            bool isValid = allArrays.All(a => IsSequenceContainsArray(a, dataCells));

            if (!isValid)
            {
                throw new ArgumentException(Properties.Resources.WARN_CannotChangePartOfArray);
            }
        }


        private static bool IsSequenceContainsArray(DataArray array, IList<DataCell> dataCells)
        {
            foreach (var x in array.Array)
            {
                if (!dataCells.Contains(x)) return false;
            }
            return true;
        }
    }
}
