using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAlex.MathCore.ExpressionEvaluation.Trees.Metadata;

namespace TAlex.PowerCalc.ViewModels
{
    public class InsertFunctionContextMenuViewModel
    {
        public List<FunctionCategoryViewModel> Categories { get; set; }


        public InsertFunctionContextMenuViewModel(IFunctionsMetadataProvider functionsMetadataProvider)
        {
            Init(functionsMetadataProvider);
        }

        private void Init(IFunctionsMetadataProvider functionsMetadataProvider)
        {
            IList<FunctionMetadata> metadata = functionsMetadataProvider.GetMetadata().ToList();

            Categories = metadata
                .GroupBy(x => x.Category)
                .Select(x => new FunctionCategoryViewModel()
                {
                    CategoryName = x.Key,
                    Functions = x.Select(f => ConvertToFunctionViewModel(f)).ToList()
                }).ToList();
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
    }

    public class FunctionCategoryViewModel
    {
        public string CategoryName { get; set; }

        public List<FunctionViewModel> Functions { get; set; }
    }

    public class FunctionViewModel
    {
        public string DisplayName { get; set; }

        public string FunctionName { get; set; }

        public string InsertValue { get; set; }

        public IList<SignatureViewModel> Signatures { get; set; }
    }

    public class SignatureViewModel
    {
        public string Name { get; set; }
        public IList<KeyValuePair<string, string>> Arguments { get; set; }
        public int ArgumentsCount { get; set; }
    }
}
