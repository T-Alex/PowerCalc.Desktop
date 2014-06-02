using System;
using System.Collections.Generic;
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
        private static SolidColorBrush SignatureNameBrush = new SolidColorBrush(Color.FromRgb(220, 50, 47));
        private static SolidColorBrush PuncBrush = new SolidColorBrush(Color.FromRgb(147, 161, 161));
        private static SolidColorBrush TypeBrush = new SolidColorBrush(Colors.Teal);
        private static SolidColorBrush ArgNameBrush = new SolidColorBrush(Color.FromRgb(25, 95, 145));


        public override object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            FunctionSignature signature = value as FunctionSignature;

            TextBlock textBlock = new TextBlock();
            textBlock.Inlines.Add(new Run(signature.Name) { Foreground = SignatureNameBrush });
            textBlock.Inlines.Add(new Run("(") { Foreground = PuncBrush });

            textBlock.Inlines.AddRange(signature.Arguments.SelectMany(a => new List<Inline> { new Run(a.Type) { Foreground = TypeBrush }, new Run(" "), new Run(a.Name) { Foreground = ArgNameBrush } }));

            //for (int i = 0; i < signatures.Count; i++)
            //{
            //    SignatureViewModel signature = signatures[i];
            //    textBlock.Inlines.Add(String.Format("{0}(", signature.Name));

            //    for (int j = 0; j < signature.Arguments.Count; j++)
            //    {
            //        KeyValuePair<string, string> arg = signature.Arguments[j];

            //        textBlock.Inlines.Add(new Italic(new Run(arg.Key)));
            //        textBlock.Inlines.Add(new Run(" " + arg.Value + ((j < signature.Arguments.Count - 1) ? ", " : String.Empty)));
            //    }

            //    textBlock.Inlines.Add(")");

            //    if (i < signatures.Count - 1)
            //    {
            //        textBlock.Inlines.Add(new LineBreak());
            //    }
            //}

            textBlock.Inlines.Add(new Run(")") { Foreground = PuncBrush });

            return textBlock;
        }
    }
}
