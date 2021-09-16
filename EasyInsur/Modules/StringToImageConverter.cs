using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using HandyControl.Tools.Extension;

namespace EasyInsur.Modules
{
    internal class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            var imageName = (value as string)!;
            if (imageName.IsNullOrEmpty()) return null;
            var bitmap = new BitmapImage(new Uri(imageName));
            return bitmap;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}