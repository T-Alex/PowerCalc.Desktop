using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAlex.PowerCalc.ViewModels
{
    public class InsertFunctionContextMenuViewModel
    {
        public List<FunctionCategoryViewModel> Functions { get; set; }


        public InsertFunctionContextMenuViewModel()
        {
            Functions = new List<FunctionCategoryViewModel>();
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
    }
}
