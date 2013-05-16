using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Documents;
using TAlex.PowerCalc.ViewModels;
using TAlex.WPF.Converters;


namespace TAlex.PowerCalc.Converters
{
    public class FunctionSignaturesToTooltipConverter : ConverterBase<FunctionSignaturesToTooltipConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IList<SignatureViewModel> signatures = value as IList<SignatureViewModel>;

            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.Add(new Bold(new Italic(new Run("Overload(s)"))));
            textBlock.Inlines.Add(new LineBreak());

            for (int i = 0; i < signatures.Count; i++)
            {
                SignatureViewModel signature = signatures[i];
                textBlock.Inlines.Add(String.Format("{0}(", signature.Name));

                for (int j = 0; j < signature.Arguments.Count; j++)
                {
                    KeyValuePair<string, string> arg = signature.Arguments[j];

                    textBlock.Inlines.Add(new Italic(new Run(arg.Key)));
                    textBlock.Inlines.Add(new Run(" " + arg.Value + ((j < signature.Arguments.Count - 1) ? ", " : String.Empty)));
                }

                textBlock.Inlines.Add(")");

                if (i < signatures.Count - 1)
                {
                    textBlock.Inlines.Add(new LineBreak());
                }
            }

            return new ToolTip { Content = textBlock };
        }
    }
}
