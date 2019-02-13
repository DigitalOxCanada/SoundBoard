using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SoundBoardWPF.Converters
{
    public class ImageSourceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            App myApp = (App)Application.Current;
            if (!System.IO.File.Exists($"{myApp.SelectedThemePath}/{value}")) return null;
            var fn = $"{myApp.SelectedThemePath}/{value}";

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
