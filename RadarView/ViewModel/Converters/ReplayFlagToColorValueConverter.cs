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
	/// Konverter, který mění příznak určující zda je provádeno zpětné přehrávání logů na barvu textu.
	/// </summary>
	public class ReplayFlagToColorValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var isReplaying = (bool) value;
			if (isReplaying) {
				return System.Windows.Media.Brushes.Red;
			} else {
				return System.Windows.Media.Brushes.Black;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
