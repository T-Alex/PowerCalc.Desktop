using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using TAlex.PowerCalc.ViewModels;


namespace TAlex.PowerCalc.Services
{
    public class HowToItemsProvider : IHowToItemsProvider
    {
        #region Fields

        private static readonly string ItemsNamespace = "TAlex.PowerCalc.Resources.Documents.HowTo.";

        private static readonly IDictionary<string, int> ItemGetegoryOrders = new Dictionary<string, int>
        {
            { "Worksheet", 1 },
            { "Matrices", 2 },
            { "Plot 2D", 3 },
            { "General", 4 },
        };

        #endregion

        #region IHowToItemsProvider Members

        public IList<HowToItem> GetItems()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var names = assembly.GetManifestResourceNames().Where(x => x.StartsWith(ItemsNamespace));

            return names.Select(x =>
            {
                var item = new HowToItem { Body = XamlReader.Load(assembly.GetManifestResourceStream(x)) };
                item.Caption = (string)((FlowDocument)item.Body).Tag;
                return item;
            })
            .OrderBy(x => CaptionSelector(x.Caption))
            .ToList();
        }

        #endregion

        #region Helpers

        private string CaptionSelector(string caption)
        {
            foreach (var categoryOrder in ItemGetegoryOrders)
            {
                if (caption.StartsWith(categoryOrder.Key))
                {
                    return ReplaceFirst(caption, categoryOrder.Key, categoryOrder.Value.ToString());
                }
            }
            return caption;
        }

        private static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0) return text;
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        #endregion
    }
}
