using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace SoundBoardWPF.Converters
{
    public class ColorStringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null; // return new BrushConverter().ConvertFromString("#121212");
            //var c= (Color)ColorConverter.ConvertFromString((string)value);
            //System.Drawing.Color col = (System.Drawing.Color)value;
            //Color c = Color.FromArgb(col.A, col.R, col.G, col.B);
            //return new System.Windows.Media.SolidColorBrush(c);
            return new BrushConverter().ConvertFromString((string)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //TODO needs fixing to match Convert above using string, not argb vals
            SolidColorBrush c = (SolidColorBrush)value;
            System.Drawing.Color col = System.Drawing.Color.FromArgb(c.Color.A, c.Color.R, c.Color.G, c.Color.B);
            return col;
        }
    }
}
