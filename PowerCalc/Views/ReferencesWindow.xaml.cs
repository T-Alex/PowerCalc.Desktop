using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TAlex.MathCore.ExpressionEvaluation.Trees.Metadata;
using TAlex.WPF.Helpers;

namespace TAlex.PowerCalc.Views
{
    /// <summary>
    /// Interaction logic for ReferencesWindow.xaml
    /// </summary>
    public partial class ReferencesWindow : Window
    {
        public ReferencesWindow()
        {
            InitializeComponent();
        }


        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FunctionsListView != null && e.AddedItems.Count > 0)
            {
                FunctionMetadata selectedItem = e.AddedItems[0] as FunctionMetadata;
                var group = FunctionsListView.Items.Groups.First(x => ((dynamic)x).Name == selectedItem.Category);

                DependencyObject itemContainer = FunctionsListView.ItemContainerGenerator.ContainerFromItem(group);
                Expander expander = TAlex.PowerCalc.Helpers.VisualHelper.GetChild<Expander>(itemContainer);

                if (expander != null)
                {
                    expander.IsExpanded = true;
                    FunctionsListView.UpdateLayout();

                    FunctionsListView.SelectedItem = selectedItem;
                    FunctionsListView.ScrollIntoView(selectedItem);
                }
            }
        }

        private void ContentPresenter_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ContentPresenter contentPresenter = (ContentPresenter)sender;
            FunctionsListView.SelectedItem = contentPresenter.Content;
        }
    }
}
