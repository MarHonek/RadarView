using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace RadarView.ViewModel.Converters
{
	/// <summary>
	/// Konverter pro převod bool hodnoty na viditelnost GUI objektu.
	/// </summary>
	public class BoolToVisibilityValueConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool) value ? Visibility.Visible : Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (Visibility) value == Visibility.Visible;
		}
	}
}
