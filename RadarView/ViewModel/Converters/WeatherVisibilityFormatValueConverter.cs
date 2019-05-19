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
	/// Konverter pro převod formátu viditelnosti v rámci počasí.
	/// Jestliže je viditelnost > 1000m pak jednotka je kilometry jinak metry.
	/// </summary>
	public class WeatherVisibilityFormatValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) {
				return "N/A";
			}

			var visibility = (int) value;

			if (visibility >= 1000) {

				return (visibility/1000) + " Km";
			}

			return visibility + " m";

		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
