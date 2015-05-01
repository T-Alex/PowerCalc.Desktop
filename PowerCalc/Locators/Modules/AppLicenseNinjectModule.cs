using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.PowerCalc.Licensing;
using TAlex.License;


namespace TAlex.PowerCalc.Locators.Modules
{
    public class AppLicenseNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ILicenseDataManager>().To<AppLicenseDataManager>().InSingletonScope();
            Bind<ITrialPeriodDataProvider>().To<AppTrialPeriodDataProvider>().InSingletonScope();
            Bind<LicenseBase>().To<AppLicense>().InSingletonScope();
        }
    }
}
