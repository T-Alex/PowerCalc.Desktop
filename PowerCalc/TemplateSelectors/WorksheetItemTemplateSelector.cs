using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TAlex.PowerCalc.ViewModels.Worksheet;


namespace TAlex.PowerCalc.TemplateSelectors
{
    public class WorksheetItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate NewItemTemplate { get; set; }
        public DataTemplate HistoryItemTemplate { get; set; }
        public DataTemplate CommentItemTemplate { get; set; }


        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            WorksheetItem worksheetItem = item as WorksheetItem;

            if (worksheetItem.Result == null) return NewItemTemplate;
            if (worksheetItem.Result is String) return CommentItemTemplate;
            return HistoryItemTemplate;
        }
    }
}
