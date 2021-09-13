using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace HandyControlWpfCoreApp1.Modules
{
    class CommissionTypeToVisibilityConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = (ComboBoxItem)value;
            return s.Content.ToString().ToLowerInvariant() == "percentage" ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = (Visibility)value;
            return visibility == Visibility.Visible ? "Percentage" : "Amount";
        }
    }
}
