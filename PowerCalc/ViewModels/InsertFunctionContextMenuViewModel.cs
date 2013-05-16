using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAlex.PowerCalc.ViewModels
{
    public class InsertFunctionContextMenuViewModel
    {
        public List<FunctionCategoryViewModel> Categories { get; set; }


        public InsertFunctionContextMenuViewModel()
        {
            Categories = new List<FunctionCategoryViewModel>();
        }
    }

    public class FunctionCategoryViewModel
    {
        public string CategoryName { get; set; }

        public List<FunctionViewModel> Functions { get; set; }
    }

    public class FunctionViewModel
    {
        public string DisplayName { get; set; }

        public string FunctionName { get; set; }

        public string InsertValue { get; set; }

        public IList<SignatureViewModel> Signatures { get; set; }
    }

    public class SignatureViewModel
    {
        public string Name { get; set; }
        public IList<KeyValuePair<string, string>> Arguments { get; set; }
        public int ArgumentsCount { get; set; }
    }
}
