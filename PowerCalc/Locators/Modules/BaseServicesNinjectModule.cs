using Ninject.Modules;
using System;
using TAlex.PowerCalc.Services;
using TAlex.PowerCalc.Converters;
using TAlex.WPF.Theming;
using TAlex.Common.Models;
using System.Reflection;
using TAlex.Mvvm.Services;


namespace TAlex.PowerCalc.Locators.Modules
{
    public class BaseServicesNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<AssemblyInfo>().ToConstant(new AssemblyInfo(Assembly.GetExecutingAssembly())).InSingletonScope();
            Bind<IAppSettings>().ToMethod(x => Properties.Settings.Default).InSingletonScope();
            Bind<IClipboardService>().To<ClipboardService>();
            Bind<IMessageService>().To<MessageService>();
            Bind<WorksheetMatrixCachedValueConverter>().ToSelf();
            Bind<IHowToItemsProvider>().To<HowToItemsProvider>();

            TAlex.WPF.Theming.ThemeLocator.SetManager(TAlex.WPFThemes.Twilight.TwilightThemeManager.Instance);
            Bind<IThemeManager>().ToConstant(TAlex.WPF.Theming.ThemeLocator.Manager);
        }
    }
}
