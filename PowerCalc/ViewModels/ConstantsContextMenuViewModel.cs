using System;
using System.Collections.Generic;
using System.Linq;


namespace TAlex.PowerCalc.ViewModels
{
    public class ConstantsContextMenuViewModel
    {
        public List<ConstantViewModel> Constants { get; set; }


        public ConstantsContextMenuViewModel()
        {
        }
    }

    public class ConstantViewModel
    {
        public string DisplayName { get; set; }

        public string Value { get; set; }

        public string NumericValue { get; set; }
    }
}
