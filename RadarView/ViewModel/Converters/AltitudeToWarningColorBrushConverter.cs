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
	/// Konverter pro převod výšky na barvu. Pokud je výška menší nez neomezená nastaví barvu na červenou.
	/// </summary>
	public class AltitudeToWarningColorBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var altitude = (int) value;
			if (altitude == int.MaxValue) {
				return System.Windows.Media.Brushes.Black;
			}
			else {
				return System.Windows.Media.Brushes.Red;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
