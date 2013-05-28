using Ninject.Modules;
using System;
using TAlex.Common.Environment;


namespace TAlex.PowerCalc.Locators.Modules
{
    public class BaseServicesNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ApplicationInfo>().ToConstant(ApplicationInfo.Current);
        }
    }
}
