using Ninject;
using Ninject.Modules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TAlex.Common.Environment;
using TAlex.MathCore.ExpressionEvaluation.Trees.Metadata;
using TAlex.PowerCalc.Locators.Modules;
using TAlex.PowerCalc.ViewModels;


namespace TAlex.PowerCalc.Locators
{
    public class ViewModelLocator
    {
        #region Fields

        private IKernel _kernel;

        #endregion

        #region Properties

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

        public RegistrationWindowViewModel RegistrationWindowViewModel
        {
            get
            {
                return _kernel.Get<RegistrationWindowViewModel>();
            }
        }


        public WorksheetMatrixViewModel WorksheetMatrixViewModel
        {
            get
            {
                return _kernel.Get<WorksheetMatrixViewModel>();
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
                new AppLicenseNinjectModule(),
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
