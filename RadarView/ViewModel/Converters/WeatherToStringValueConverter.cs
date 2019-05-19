using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace RadarView.ViewModel.Converters
{
	/// <summary>
	/// Konverter pro převod prázdného stringu na hodnotu N/A.
	/// </summary>
	public class WeatherToStringValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) {
				return "N/A";
			}

			var stringValue = value.ToString();
			if (value is string) {
				if (string.IsNullOrEmpty(stringValue)) {
					return "N/A";
				}

			}

			if (parameter != null) {
				var unit = (string) parameter;
				return stringValue + " " + unit;
			}

			return stringValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
