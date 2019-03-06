using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DigitalOx.SoundBoard.Converters
{
    public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            App myApp = (App)Application.Current;
            if (!System.IO.File.Exists($"{myApp.SelectedProfilePath}/{value}")) return null;
            var fn = $"{myApp.SelectedProfilePath}/{value}";

            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(fn, UriKind.Relative);
            image.CacheOption = BitmapCacheOption.OnLoad;
            image.EndInit();

            return (ImageSource)image;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
