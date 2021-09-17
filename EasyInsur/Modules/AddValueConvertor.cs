using System;
using System.Windows.Data;

namespace EasyInsur.Modules
{
    class AddValueConvertor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType,
                object parameter, System.Globalization.CultureInfo culture)
        {
            var result =
                (long)values[0] + (long)values[1];
            return $"Total Registered: {result}";
        }
        public object[] ConvertBack(object value, Type[] targetTypes,
            object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }
}
