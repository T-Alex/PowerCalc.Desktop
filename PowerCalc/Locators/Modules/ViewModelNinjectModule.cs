using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.PowerCalc.ViewModels;
using TAlex.PowerCalc.ViewModels.Worksheet;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;
using TAlex.PowerCalc.ViewModels.Matrices;
using TAlex.PowerCalc.Services;
using TAlex.PowerCalc.Converters;


namespace TAlex.PowerCalc.Locators.Modules
{
    public class ViewModelNinjectModule : NinjectModule
    {
        public override void Load()
        {
            Bind<MainWindowViewModel>().ToSelf();
            Bind<PreferencesWindowViewModel>().ToSelf();
            Bind<ReferencesWindowViewModel>().ToSelf();
            Bind<AboutWindowViewModel>().ToSelf().InSingletonScope();
            Bind<RegistrationWindowViewModel>().ToSelf();
            Bind<WorksheetMatrixViewModel>().ToSelf();
            Bind<InsertFunctionContextMenuViewModel>().ToSelf();
            Bind<ConstantsContextMenuViewModel>().ToSelf();
            Bind<WorksheetModel>().ToSelf();

            Bind<DataTable>().ToConstructor(x => new DataTable(
                x.Inject<IExpressionTreeBuilder<Object>>(),
                x.Inject<IClipboardService>(),
                x.Inject<WorksheetMatrixCachedValueConverter>(),
                ((IAppSettings)x.Context.Kernel.GetService(typeof(IAppSettings))).MatricesWorksheetRows,
                ((IAppSettings)x.Context.Kernel.GetService(typeof(IAppSettings))).MatricesWorksheetColumns));
        }
    }
}
