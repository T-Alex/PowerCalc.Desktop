using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.MathCore.ExpressionEvaluation.Trees.Metadata;

namespace TAlex.PowerCalc.ViewModels
{
    public class ReferencesWindowViewModel
    {
        protected readonly IConstantsMetadataProvider ConstantsMetadataProvider;
        protected readonly IFunctionsMetadataProvider FunctionsMetadataProvider;


        public IList<FunctionMetadata> FunctionMetadata { get; set; }
        public IList<ConstantMetadata> ConstantMetadata { get; set; }


        public ReferencesWindowViewModel(IConstantsMetadataProvider constantsMetadataProvider, IFunctionsMetadataProvider functionsMetadataProvider)
        {
            ConstantsMetadataProvider = constantsMetadataProvider;
            FunctionsMetadataProvider = functionsMetadataProvider;

            ConstantMetadata = ConstantsMetadataProvider.GetMetadata().ToList();
            FunctionMetadata = FunctionsMetadataProvider.GetMetadata().ToList();
        }
    }
}
