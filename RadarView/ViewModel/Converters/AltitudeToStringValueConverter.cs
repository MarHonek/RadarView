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
	/// Konverter pro převod výšky na string.
	/// </summary>
	public class AltitudeToStringValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var altitude = (int) value;
			if (altitude == int.MaxValue) {
				return "Neomezeno";
			}

			if (altitude > 4000) {
				return "FL " + (altitude / 100).ToString();
			}

			return altitude.ToString() + " ft";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
