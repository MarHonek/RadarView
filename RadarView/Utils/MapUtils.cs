using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarView.Utils
{
	/// <summary>
	/// Pomocná třída pro práci s mapovými podklady.
	/// </summary>
	public class MapUtils
	{
		/// <summary>
		/// Získá jméno souboru obsahující textury mapy.
		/// </summary>
		/// <param name="mapName">Název mapy.</param>
		/// <param name="icao">ICAO letiště, které se nachází ve středu mapy.</param>
		/// <param name="mapLevel">Zoom level.</param>
		/// <returns>Název souboru obsahující texuturu mapy.</returns>
		public static string GetMapTextureName(string mapName, string icao, int mapLevel)
		{
			return string.Format("{0}_{1}_{2}", mapName, icao, mapLevel);
		}
	}
}
