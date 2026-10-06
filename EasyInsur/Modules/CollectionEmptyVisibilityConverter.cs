using System;
using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EasyInsur.Modules
{
    public class CollectionEmptyVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool isEmpty = true;

            if (value is IEnumerable enumerable)
            {
                var enumerator = enumerable.GetEnumerator();
                isEmpty = !enumerator.MoveNext();
            }
            else if (value is int count)
            {
                isEmpty = count == 0;
            }
            else if (value is long longCount)
            {
                isEmpty = longCount == 0;
            }

            bool invert = parameter?.ToString()?.Equals("Inverse", StringComparison.OrdinalIgnoreCase) == true;
            if (invert)
            {
                return isEmpty ? Visibility.Collapsed : Visibility.Visible;
            }
            return isEmpty ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
            throw new NotImplementedException();
    }
}
