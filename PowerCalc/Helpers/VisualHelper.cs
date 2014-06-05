using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace TAlex.PowerCalc.Helpers
{
    public static class VisualHelper
    {
        public static IInputElement FindFirstFocusableElement(DependencyObject obj)
        {
            IInputElement firstFocusable = null;

            int count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count && null == firstFocusable; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                IInputElement inputElement = child as IInputElement;
                if (null != inputElement && inputElement.Focusable)
                {
                    firstFocusable = inputElement;
                }
                else
                {
                    firstFocusable = FindFirstFocusableElement(child);
                }
            }

            return firstFocusable;
        }

        public static T GetChild<T>(DependencyObject obj) where T : class
        {
            T target = default(T);

            int count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count && null == target; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                T inputElement = child as T;
                if (null != inputElement)
                {
                    target = inputElement;
                }
                else
                {
                    target = GetChild<T>(child);
                }
            }

            return target;
        }
    }
}
