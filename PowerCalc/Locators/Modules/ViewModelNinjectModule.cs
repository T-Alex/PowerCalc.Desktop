using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.PowerCalc.ViewModels;


namespace TAlex.PowerCalc.Locators.Modules
{
    public class ViewModelNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<MainWindowViewModel>().ToSelf();
            Bind<AboutWindowViewModel>().ToSelf().InSingletonScope();
            Bind<RegistrationWindowViewModel>().ToSelf();
            Bind<InsertFunctionContextMenuViewModel>().ToSelf();
            Bind<ConstantsContextMenuViewModel>().ToSelf();
        }
    }
}
