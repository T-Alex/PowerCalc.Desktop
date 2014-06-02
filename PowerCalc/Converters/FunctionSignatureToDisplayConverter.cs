using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using TAlex.MathCore.ExpressionEvaluation.Trees.Metadata;
using TAlex.PowerCalc.ViewModels;
using TAlex.WPF.Converters;

namespace TAlex.PowerCalc.Converters
{
    public class FunctionSignatureToDisplayConverter : ConverterBase<FunctionSignatureToDisplayConverter>
    {
        private static readonly SolidColorBrush SignatureNameBrush = new SolidColorBrush(Color.FromRgb(220, 50, 47));
        private static readonly SolidColorBrush PuncBrush = new SolidColorBrush(Color.FromRgb(147, 161, 161));
        private static readonly SolidColorBrush TypeBrush = new SolidColorBrush(Colors.Teal);
        private static readonly SolidColorBrush ArgNameBrush = new SolidColorBrush(Color.FromRgb(25, 95, 145));


        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            FunctionSignature signature = value as FunctionSignature;

            TextBlock textBlock = new TextBlock();
            InlineCollection inlines = textBlock.Inlines;

            inlines.Add(new Run(signature.Name) { Foreground = SignatureNameBrush });
            inlines.Add(new Run("(") { Foreground = PuncBrush });

            for (int i = 0; i < signature.Arguments.Count; i++)
            {
                var arg = signature.Arguments[i];

                inlines.Add(new Run(arg.Type) { Foreground = TypeBrush });
                inlines.Add(new Run(" "));
                inlines.Add(new Run(arg.Name) { Foreground = ArgNameBrush });

                if (i < signature.Arguments.Count - 1)
                {
                    inlines.Add(new Run(", ") { Foreground = PuncBrush });
                }
            }

            inlines.Add(new Run(")") { Foreground = PuncBrush });

            return textBlock;
        }
    }
}
