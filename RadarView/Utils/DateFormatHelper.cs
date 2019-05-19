using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarView.Utils
{
	/// <summary>
	/// Pomocná třída starající se o formátování času.
	/// </summary>
	public static class DateFormatHelper
	{
		/// <summary>
		/// Vrátí stringový řetezec pro zadaný čas a formát.
		/// </summary>
		/// <param name="datetime">čas.</param>
		/// <param name="format">formát času.</param>
		/// <param name="inLocalTime">příznak určující zda má být čas lokální nebo UTC.</param>
		/// <returns>stringový řetezec zobrazující čas.</returns>
		public static string GetTimeLabelInFormat(DateTime datetime, string format, bool inLocalTime) 
		{
			if(inLocalTime) 
			{
				return datetime.ToLocalTime().ToString(format) + " LOC";
			}

			return datetime.ToString(format) + " UTC";
		}

		/// <summary>
		/// Vrátí stringový řetezec pro zadaný čas a formát.
		/// </summary>
		/// <param name="timestamp">časová známka UTC.</param>
		/// <param name="format">formát času.</param>
		/// <param name="inLocalTime">příznak určující zda má být čas lokální nebo UTC.</param>
		/// <returns>stringový řetezec zobrazující čas.</returns>
		public static string GetTimeLabelInFormat(long timestamp, string format, bool inLocalTime) 
		{
			return GetTimeLabelInFormat(DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime, format, inLocalTime);
		}
	}
}
