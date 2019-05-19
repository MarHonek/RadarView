using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using RadarView.Model.Entities.AviationData.Airspace;
using RadarView.Properties;
using RadarView.Utils;
using RadarView.ViewModel.Converters;

namespace RadarView.Model.Service.Config
{
	/// <summary>
	/// Třída udržující barvy uložené v konfigu.
	/// </summary>
	public static class Colors
	{
		/// <summary>
		/// Barva pozadí.
		/// </summary>
		public static readonly Color ViewBackgroundColor = ColorUtil.HexToColor(Settings.Default.BackgroundColor);

		/// <summary>
		/// Barva aktivních vzdušných prostorů
		/// </summary>
		public static readonly Color AirspaceActiveColor = ColorUtil.HexToColor(Settings.Default.ActiveAirspaceColor);

		/// <summary>
		/// Barva vzletových drah
		/// </summary>
		public static readonly Color AirportColor = ColorUtil.HexToColor(Settings.Default.AirportColor);

		/// <summary>
		/// Barva obdélníku ohraničující sledovanou oblast
		/// </summary>
		public static readonly Color MonitoredAreaRectColor = ColorUtil.HexToColor(Settings.Default.MonitoredAreaRectColor);

		/// <summary>
		/// Barva podkladové mapy.
		/// </summary>
		public static readonly Color MapColor = ColorUtil.HexToColor(Settings.Default.MapColor);

		/// <summary>
		/// Barva letadla na zemi.
		/// </summary>
		public static readonly Color AircraftOnGroundColor = ColorUtil.HexToColor(Settings.Default.AircraftOnGroundColor);

		/// <summary>
		/// Barva letadla ve vzduchu
		/// </summary>
		public static readonly Color AircraftInAirColor = ColorUtil.HexToColor(Settings.Default.AircraftInAirColor);

		/// <summary>
		/// Barva letadla s výstrahout
		/// </summary>
		public static readonly Color AircraftAlertColor = ColorUtil.HexToColor(Settings.Default.AircraftAlert1Color);

		/// <summary>
		/// Barva letadla s upozorněním č.1
		/// </summary>
		public static readonly Color AircraftNotice1Color = ColorUtil.HexToColor(Settings.Default.AircraftNotice1Color);

		/// <summary>
		/// Barva letadla s upozorněním č.2
		/// </summary>
		public static readonly Color AircraftNotice2Color = ColorUtil.HexToColor(Settings.Default.AircraftNotice2Color);

		/// <summary>
		/// Barva vybraného letadla
		/// </summary>
		public static readonly Color SelectedAircraftColor = ColorUtil.HexToColor(Settings.Default.AircraftSelectedColor);

		/// <summary>
		/// Kolekce obsahující barvy k jednotlivým kategoriím vzdušných prostorů.
		/// </summary>
		public static readonly Dictionary<AirspaceCategory, Color> AirspaceCategoriesColors;


		static Colors()
		{
			var serializedAirspaceCategoriesColors = Settings.Default.AirspaceCategoriesColors;
			AirspaceCategoriesColors = JsonConvert.DeserializeObject<Dictionary<AirspaceCategory, Color>>(serializedAirspaceCategoriesColors, new StringToColorJsonConverter());
		}
	}
}
