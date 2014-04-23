using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using TAlex.PowerCalc.Helpers;


namespace TAlex.PowerCalc.Controls
{
    public class DataGridTemplateColumnEx : DataGridColumn
    {
        private string _bindingPath;
        private DataTemplate _cellTemplate;
        private DataTemplate _cellEditingTemplate;


        public virtual string BindingPath
        {
            get
            {
                return _bindingPath;
            }

            set
            {
                if (_bindingPath != value)
                {
                    _bindingPath = value;
                    base.NotifyPropertyChanged("BindingPath");
                }
            }
        }

        public DataTemplate CellTemplate
        {
            get
            {
                return _cellTemplate;
            }

            set
            {
                if (_cellTemplate != value)
                {
                    _cellTemplate = value;
                    base.NotifyPropertyChanged("CellTemplate");
                }
            }
        }

        public DataTemplate CellEditingTemplate
        {
            get
            {
                return _cellEditingTemplate;
            }

            set
            {
                if (_cellEditingTemplate != value)
                {
                    _cellEditingTemplate = value;
                    base.NotifyPropertyChanged("CellEditingTemplate");
                }
            }
        }


        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            ContentPresenter contentPresenter = new ContentPresenter { ContentTemplate = CellEditingTemplate };
            BindingOperations.SetBinding(contentPresenter, ContentPresenter.ContentProperty, new Binding(_bindingPath));

            return contentPresenter;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            ContentPresenter contentPresenter = new ContentPresenter { ContentTemplate = CellTemplate };
            BindingOperations.SetBinding(contentPresenter, ContentPresenter.ContentProperty, new Binding(_bindingPath));

            return contentPresenter;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            TextBox textBox = VisualHelper.FindFirstFocusableElement(editingElement) as TextBox;
            //TextBox textBox = editingElement as TextBox;
            if (textBox == null)
            {
                return null;
            }
            textBox.Focus();
            string text = textBox.Text;
            TextCompositionEventArgs textCompositionEventArg = editingEventArgs as TextCompositionEventArgs;
            if (textCompositionEventArg != null)
            {
                string str = ConvertTextForEdit(textCompositionEventArg.Text);
                textBox.Text = str;
                textBox.Select(str.Length, 0);
            }
            else if (!(editingEventArgs is MouseButtonEventArgs) || !PlaceCaretOnTextBox(textBox, Mouse.GetPosition(textBox)))
            {
                textBox.SelectAll();
            }
            return text;
        }
 

        private static bool PlaceCaretOnTextBox(TextBox textBox, Point position)
        {
            int characterIndexFromPoint = textBox.GetCharacterIndexFromPoint(position, false);
            if (characterIndexFromPoint < 0)
            {
                return false;
            }
            textBox.Select(characterIndexFromPoint, 0);
            return true;
        }

        private string ConvertTextForEdit(string s)
        {
            if (s == "\b")
            {
                s = String.Empty;
            }
            return s;
        }
    }
}
