//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Microsoft.Windows.Controls
{
    /// <summary>
    /// Converts Boolean to SelectiveScrollingOrientation based on the given parameter.
    /// </summary> 
    [Localizability(LocalizationCategory.NeverLocalize)]
    internal sealed class BooleanToSelectiveScrollingOrientationConverter : IValueConverter
    {
        /// <summary>
        ///     Convert Boolean to SelectiveScrollingOrientation
        /// </summary>
        /// <param name="value">Boolean</param>
        /// <param name="targetType">SelectiveScrollingOrientation</param>
        /// <param name="parameter">SelectiveScrollingOrientation that should be used when the Boolean is true</param>
        /// <param name="culture">null</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && parameter is SelectiveScrollingOrientation)
            {
                var valueAsBool = (bool)value;
                var parameterSelectiveScrollingOrientation = (SelectiveScrollingOrientation)parameter;

                if (valueAsBool)
                {
                    return parameterSelectiveScrollingOrientation;
                }
            }

            return SelectiveScrollingOrientation.Both;
        }

        /// <summary>
        ///     Not implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// Converts DataGridHeadersVisibility to Visibility based on the given parameter.
    /// </summary> 
    [Localizability(LocalizationCategory.NeverLocalize)]
    internal sealed class DataGridHeadersVisibilityToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Convert DataGridHeadersVisibility to Visibility
        /// </summary>
        /// <param name="value">DataGridHeadersVisibility</param>
        /// <param name="targetType">Visibility</param>
        /// <param name="parameter">DataGridHeadersVisibility that represents the minimum DataGridHeadersVisibility that is needed for a Visibility of Visible</param>
        /// <param name="culture">null</param>
        /// <returns>Visible or Collapsed based on the value & converter mode</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visible = false;

            if (value is DataGridHeadersVisibility && parameter is DataGridHeadersVisibility)
            {
                var valueAsDataGridHeadersVisibility = (DataGridHeadersVisibility)value;
                var parameterAsDataGridHeadersVisibility = (DataGridHeadersVisibility)parameter;

                switch (valueAsDataGridHeadersVisibility)
                {
                    case DataGridHeadersVisibility.All:
                        visible = true;
                        break;

                    case DataGridHeadersVisibility.Column:
                        visible = parameterAsDataGridHeadersVisibility == DataGridHeadersVisibility.Column ||
                                    parameterAsDataGridHeadersVisibility == DataGridHeadersVisibility.None;
                        break;

                    case DataGridHeadersVisibility.Row:
                        visible = parameterAsDataGridHeadersVisibility == DataGridHeadersVisibility.Row ||
                                    parameterAsDataGridHeadersVisibility == DataGridHeadersVisibility.None;
                        break;
                }
            }

            if (targetType == typeof(Visibility))
            {
                return visible ? Visibility.Visible : Visibility.Collapsed;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}