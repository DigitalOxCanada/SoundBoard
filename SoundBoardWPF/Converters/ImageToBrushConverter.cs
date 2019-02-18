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
    public class ImageToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            if (string.IsNullOrEmpty((string)value)) return null;
            App myApp = (App)Application.Current;
            if (!System.IO.File.Exists($"{myApp.SelectedThemePath}/{value}")) return null;

            ImageBrush brush = new ImageBrush();
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            string img = "nopic.png";
            img = $"{myApp.SelectedThemePath}/{value}";
            bitmap.UriSource = new Uri($"{img}", UriKind.Relative);
            bitmap.EndInit();
            brush.ImageSource = bitmap;
            return brush;

            //var c= (Color)ColorConverter.ConvertFromString((string)value);
            //System.Drawing.Color col = (System.Drawing.Color)value;
            //Color c = Color.FromArgb(col.A, col.R, col.G, col.B);
            //return new System.Windows.Media.SolidColorBrush(c);
            //            return new BrushConverter().ConvertFromString((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
