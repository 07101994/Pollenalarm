using System;
using System.Globalization;
using Xamarin.Forms;

namespace Pollenalarm.Frontend.Forms.Converters
{
	class IntToPollutionIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (!(value is int))
				return value;

			switch ((int)value)
			{
				default:
					return "AlertIcon.png";
				case 0:
					return "CheckIcon.png";

			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
