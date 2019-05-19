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
	/// Konverter, který přidá jednotku veličiny do zobrazení.
	/// </summary>
	public class ValueToStringWithUnitValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var unit = (string) parameter;
			if (value == null) {
				return null;
			}
			return value.ToString() + unit;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
