using System;
using System.Globalization;
using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.Converters
{
    public class BoolToTextColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value == false)
            {
                // Choose light gray color for disabled pollen
                var color = new Color(Color.Default.R, Color.Default.G, Color.Default.B);
                color = color.WithLuminosity(0.8);
                return color;
            }

            return Color.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
