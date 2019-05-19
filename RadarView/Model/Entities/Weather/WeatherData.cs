using System.Collections.Generic;
using Newtonsoft.Json;

namespace RadarView.Model.Entities.Weather
{
    /// <summary>
    /// Reprezentuje meteorologicke informace.
    /// </summary>
    public class WeatherData
    {
		/// <summary>
		/// Interni parametr.
		/// </summary
		[JsonProperty("@base")]
		public string Base { get; set; }

		/// <summary>
		/// Hlavni meteo informace.
		/// </summary>
		[JsonProperty("main")]
		public Weather Weather { get; set; }

        /// <summary>
        /// Viditelnost (metr).
        /// </summary>
        [JsonProperty("visibility")]
        public int Visibility { get; set; }

		/// <summary>
		/// Vitr.
		/// </summary>
		[JsonProperty("wind")]
		public Wind Wind { get; set; }

		/// <summary>
		/// Oblacnost.
		/// </summary>
		[JsonProperty("clouds")]
		public Clouds Clouds { get; set; }

        /// <summary>
        /// Zemepisna poloha mista, pro ktere jsou data platna.
        /// </summary>
        [JsonProperty("coord")]
		public WeatherCoordinates Coordinates { get; set; }

		/// <summary>
		/// UTC casova znaka stavu pocasi.
		/// </summary>
		[JsonProperty("dt")]
		public int Time { get; set; }

		/// <summary>
		/// ID mesta.
		/// </summary>
		[JsonProperty("id")]
		public int CityId { get; set; }

		/// <summary>
		/// Nazev mesta ve kter se nachazi meteo stanice.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }

		/// <summary>
		/// Interni parametr.
		/// </summary>
		[JsonProperty("cod")]
		public int Cod { get; set; }

		/// <summary>
		/// Kody stavu pocasi.
		/// </summary>
		[JsonProperty("weather")]
		public WeatherServiceInfo[] WeatherCode { get; set; }

		/// <summary>
		/// Systemove parametry.
		/// </summary>
		[JsonProperty("sys")]
		public WeatherDayInfo System { get; set; }
    }
}
