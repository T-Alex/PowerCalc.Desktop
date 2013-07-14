using System;
using System.Collections.Generic;
using System.Linq;
using TAlex.MathCore.ExpressionEvaluation.Trees.Metadata;


namespace TAlex.PowerCalc.ViewModels
{
    public class ConstantsContextMenuViewModel
    {
        public List<ConstantViewModel> Constants { get; set; }


        public ConstantsContextMenuViewModel(IConstantsMetadataProvider constantsMetadataProvider)
        {
            Init(constantsMetadataProvider);
        }


        private void Init(IConstantsMetadataProvider constantsMetadataProvider)
        {
            IList<ConstantMetadata> metadata = constantsMetadataProvider.GetMetadata().ToList();

            Constants = metadata.Select(x => new ConstantViewModel
                    {
                        DisplayName = x.DisplayName,
                        ConstantName = x.Name,
                        Value = x.Value
                    }).ToList();
        }
    }

    public class ConstantViewModel
    {
        public string DisplayName { get; set; }

        public string ConstantName { get; set; }

        public string Value { get; set; }
    }
}
