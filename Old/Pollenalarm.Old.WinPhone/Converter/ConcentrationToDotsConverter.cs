using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Pollenalarm.Old.WinPhone.Converter
{
    public class ConcentrationToDotsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {

                switch ((string)value)
                {
                    case "Keine":
                        return "";
                    case "Schwach":
                        return "•";
                    case "Mäßig":
                        return "• •";
                    case "Stark":
                        return "• • •";
                    default:
                        return "";
                }
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
