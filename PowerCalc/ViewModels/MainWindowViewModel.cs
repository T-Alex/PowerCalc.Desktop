using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TAlex.Common.Environment;
using TAlex.MathCore.ExpressionEvaluation.Trees.Builders;


namespace TAlex.PowerCalc.ViewModels
{
    public class MainWindowViewModel
    {
        public readonly IExpressionTreeBuilder<Object> ExpressionTreeBuilder;


        public virtual string WindowTitle
        {
            get
            {
                string productTitle = ApplicationInfo.Title;

                if (Licensing.License.IsTrial)
                    return String.Format("{0} - Evaluation version (days left: {1})", productTitle, Licensing.License.TrialDaysLeft);
                else
                    return productTitle;
            }
        }

        public virtual string AboutMenuItemHeader
        {
            get
            {
                string productTitle = ApplicationInfo.Title;
                return "_About " + productTitle;
            }
        }


        public MainWindowViewModel(IExpressionTreeBuilder<Object> treeBuilder)
        {
            ExpressionTreeBuilder = treeBuilder;
        }
    }
}
