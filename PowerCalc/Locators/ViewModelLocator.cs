using Ninject;
using TAlex.PowerCalc.Locators.Modules;
using TAlex.PowerCalc.ViewModels;
using TAlex.PowerCalc.ViewModels.Plot2D;


namespace TAlex.PowerCalc.Locators
{
    public class ViewModelLocator
    {
        #region Fields

        private IKernel _kernel;

        #endregion

        #region Properties

        public HowToViewModel HowToViewModel
        {
            get
            {
                return _kernel.Get<HowToViewModel>();
            }
        }

        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                return _kernel.Get<MainWindowViewModel>();
            }
        }

        public PreferencesWindowViewModel PreferencesWindowViewModel
        {
            get
            {
                return _kernel.Get<PreferencesWindowViewModel>();
            }
        }

        public ReferencesWindowViewModel ReferencesWindowViewModel
        {
            get
            {
                return _kernel.Get<ReferencesWindowViewModel>();
            }
        }

        public AboutWindowViewModel AboutWindowViewModel
        {
            get
            {
                return _kernel.Get<AboutWindowViewModel>();
            }
        }

        public WorksheetMatrixViewModel WorksheetMatrixViewModel
        {
            get
            {
                return _kernel.Get<WorksheetMatrixViewModel>();
            }
        }

        public Traces2DModel Traces2DViewModel
        {
            get
            {
                return _kernel.Get<Traces2DModel>();
            }
        }

        public InsertFunctionContextMenuViewModel InsertFunctionContextMenuViewModel
        {
            get
            {
                return _kernel.Get<InsertFunctionContextMenuViewModel>();
            }
        }

        public ConstantsContextMenuViewModel ConstantsContextMenuViewModel
        {
            get
            {
                return _kernel.Get<ConstantsContextMenuViewModel>();
            }
        }

        #endregion

        #region Constructors

        public ViewModelLocator()
        {
            _kernel = new StandardKernel(
                new BaseServicesNinjectModule(),
                new MathCoreNinjectModule(),
                new ViewModelNinjectModule());
        }

        #endregion

        #region Methods

        public T Get<T>()
        {
            return _kernel.Get<T>();
        }

        #endregion
    }
}
