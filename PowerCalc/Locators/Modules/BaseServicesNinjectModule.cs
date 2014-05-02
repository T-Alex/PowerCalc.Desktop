using Ninject.Modules;
using System;
using TAlex.Common.Environment;
using TAlex.PowerCalc.Services;


namespace TAlex.PowerCalc.Locators.Modules
{
    public class BaseServicesNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ApplicationInfo>().ToConstant(ApplicationInfo.Current);
            Bind<IAppSettings>().ToMethod(x => Properties.Settings.Default).InSingletonScope();
        }
    }
}
