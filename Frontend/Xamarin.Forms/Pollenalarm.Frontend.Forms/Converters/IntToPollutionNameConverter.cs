using System;
using System.Globalization;
using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.Converters
{
	public class IntToPollutionNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is int))
				return value;

			switch ((int)value)
			{
				default:
				case 0:
					return Resources.Strings.PollutionNameNone;
				case 1:
					return Resources.Strings.PollutionNameLow;
				case 2:
					return Resources.Strings.PollutionNameMedium;
				case 3:
					return Resources.Strings.PollutionNameStrong;
				case 4:
					return Resources.Strings.PollutionNameVeryStrong;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
