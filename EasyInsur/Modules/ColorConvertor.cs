using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using HandyControl.Tools.Extension;

namespace EasyInsur.Modules
{
    public class ColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value as string) == "") return DependencyProperty.UnsetValue;
            var input = (double)value;

            //custom condition is checked based on data.

            return input < 0 ? new SolidColorBrush(Colors.IndianRed) : DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
