using System.Collections.Generic;
using TAlex.PowerCalc.ViewModels;


namespace TAlex.PowerCalc.Services
{
    public interface IHowToItemsProvider
    {
        IList<HowToItem> GetItems();
    }
}
