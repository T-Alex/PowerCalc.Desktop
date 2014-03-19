using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace TAlex.PowerCalc.Controls
{
    public class DataGridTextColumnEx : DataGridColumn
    {
        private BindingBase _binding;
        private BindingBase _editingBinding;


        public virtual BindingBase Binding
        {
            get
            {
                return _binding;
            }

            set
            {
                if (_binding != value)
                {
                    _binding = value;
                    base.NotifyPropertyChanged("Binding");
                }
            }
        }

        public virtual BindingBase EditingBinding
        {
            get
            {
                return _editingBinding;
            }

            set
            {
                if (_editingBinding != value)
                {
                    _editingBinding = value;
                    base.NotifyPropertyChanged("EditingBinding");
                }
            }
        }


        protected override FrameworkElement GenerateEditingElement(DataGridCell cell, object dataItem)
        {
            TextBox textBox = new TextBox();
            if (EditingBinding == null)
                BindingOperations.ClearBinding(textBox, TextBox.TextProperty);
            else
                BindingOperations.SetBinding(textBox, TextBox.TextProperty, EditingBinding);

            return textBox;
        }

        protected override FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            TextBlock textBlock = new TextBlock();
            if (Binding == null)
                BindingOperations.ClearBinding(textBlock, TextBlock.TextProperty);
            else
                BindingOperations.SetBinding(textBlock, TextBlock.TextProperty, Binding);

            return textBlock;
        }

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            TextBox textBox = editingElement as TextBox;
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
                s = string.Empty;
            }
            return s;
        }
    }
}
