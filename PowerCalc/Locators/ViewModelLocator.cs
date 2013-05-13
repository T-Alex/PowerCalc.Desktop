using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TAlex.PowerCalc.ViewModels;


namespace TAlex.PowerCalc.Locators
{
    public class ViewModelLocator
    {
        public AboutWindowViewModel AboutWindowViewModel
        {
            get
            {
                return new AboutWindowViewModel();
            }
        }
    }
}
