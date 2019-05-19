using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using RadarView.Model.Entities.AviationData.Airports;

namespace RadarView.ViewModel.Converters
{
	/// <summary>
	/// Koverter pro zobrazení určité property třídy airport v listboxu.
	/// </summary>
	public class AirportToListBoxValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			//V listboxu zobrazí názvy letišť.
			var airport = (Airport) value;
			return airport.Name;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
