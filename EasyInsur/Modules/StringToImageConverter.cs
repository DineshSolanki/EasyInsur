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
            var str = value as string;
            if (string.IsNullOrWhiteSpace(str)) return null;

            string? finalPath = null;
            if (File.Exists(str))
            {
                finalPath = str;
            }
            else
            {
                var localAppImage = Path.Combine(Services.AppPathWithoutName, "images", str);
                if (File.Exists(localAppImage))
                {
                    finalPath = localAppImage;
                }
            }

            if (finalPath == null) return null;

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.UriSource = new Uri(finalPath, UriKind.Absolute);
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
