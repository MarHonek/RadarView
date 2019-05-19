using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Microsoft.SqlServer.Server;
using RadarView.Properties;

namespace RadarView.ViewModel.Converters
{
	/// <summary>
	/// Konverter pro převod času na text obsahující čas a časové pásmo.
	/// </summary>
	public class DateTimeToStringValueConverter : IValueConverter
	{
		/// <summary>
		/// Příznak určující zda ma být zobrazen čas v lokalním pásmu (jinak UTC).
		/// </summary>
		private readonly bool localTimeZone = Settings.Default.LocalTimeZone;


		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null) {
				return "N/A";
			}

			var format = (string) parameter;
			var dateTime = (DateTime) value;

			if (this.localTimeZone) {
				var localDateTime = dateTime.ToLocalTime();
				return localDateTime.ToString(format) + " LOC";
			}

			return dateTime.ToString(format) + " UTC";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var stringValue = (string) value;
			var stringParameter = (string) parameter;

			return DateTime.ParseExact(stringValue, stringParameter, CultureInfo.InvariantCulture);
		}
	}
}
