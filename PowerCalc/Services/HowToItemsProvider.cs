using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Markup;
using TAlex.PowerCalc.ViewModels;


namespace TAlex.PowerCalc.Services
{
    public class HowToItemsProvider : IHowToItemsProvider
    {
        private static readonly string ItemsNamespace = "TAlex.PowerCalc.Resources.Documents.HowTo.";

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
            }).ToList();
        }

        #endregion
    }
}
