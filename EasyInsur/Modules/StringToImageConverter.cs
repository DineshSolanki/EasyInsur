using System;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using HandyControl.Tools.Extension;

namespace EasyInsur.Modules
{
    internal class StringToImageConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null) return null;
            var imageName = value as string;
            if (string.IsNullOrEmpty(imageName)) return null;
            var imagePath = Path.Join(Services.AppPathWithoutName, "images", imageName);
            if (!File.Exists(imagePath)) return null;
            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.UriSource = new Uri(imagePath);
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
