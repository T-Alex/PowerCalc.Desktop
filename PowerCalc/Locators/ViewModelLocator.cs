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


        public InsertFunctionContextMenuViewModel InsertFunctionContextMenuViewModel
        {
            get
            {
                IList<FunctionMetadata> metadata = _kernel.Get<IFunctionsMetadataProvider>().GetMetadata().ToList();

                return new InsertFunctionContextMenuViewModel()
                {
                    Categories = metadata
                        .GroupBy(x => x.Category)
                        .Select(x => new FunctionCategoryViewModel()
                        {
                            CategoryName = x.Key,
                            Functions = x.Select(f => ConvertToFunctionViewModel(f)).ToList()
                        }).ToList()
                };
            }
        }

        private FunctionViewModel ConvertToFunctionViewModel(FunctionMetadata metadata)
        {
            FunctionSignature sign = metadata.Signatures.First();
            string s = String.Empty;
            for (int i = 0; i < sign.ArgumentCount - 1; i++)
            {
                s += ",";
            }

            return new FunctionViewModel
            {
                DisplayName = metadata.DisplayName,
                FunctionName = sign.Name,
                InsertValue = String.Format("{0}({1})", sign.Name, s),
                Signatures = metadata.Signatures.Select(x => new SignatureViewModel
                {
                    Name = x.Name,
                    Arguments = x.Arguments.Select(a => new KeyValuePair<string, string>(a.Type, a.Name)).ToList(),
                    ArgumentsCount = x.ArgumentCount
                }).ToList()
            };
        }

        public ConstantsContextMenuViewModel ConstantsContextMenuViewModel
        {
            get
            {
                IList<ConstantMetadata> metadata = _kernel.Get<IConstantsMetadataProvider>().GetMetadata().ToList();

                return new ConstantsContextMenuViewModel()
                {
                    Constants = metadata.Select(x => new ConstantViewModel
                    {
                        DisplayName = x.DisplayName,
                        ConstantName = x.Name,
                        Value = x.Value
                    }).ToList()
                };
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
