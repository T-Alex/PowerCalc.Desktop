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

        public RegistrationWindowViewModel RegistrationWindowViewModel
        {
            get
            {
                return new RegistrationWindowViewModel();
            }
        }


        public ConstantsContextMenuViewModel ConstantsContextMenuViewModel
        {
            get
            {
                return new ConstantsContextMenuViewModel() { Constants = new List<ConstantViewModel> { new ConstantViewModel { DisplayName = "Number 'pi'" } } };
            }
        }
    }
}
