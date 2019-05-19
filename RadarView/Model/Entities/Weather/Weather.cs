using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RadarView.Model.Entities.Weather
{
	/// <summary>
	/// Zakladni informace o pocasi.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class Weather
	{
		/// <summary>
		/// Prumerna teplota (C°).
		/// </summary>
		[JsonProperty("tempCelsius")]
		public double TemperatureCelsius {
			get { return this.Temperature - 273.15; }
		}

		/// <summary>
		/// Průměrná teplota (Kelvin)
		/// </summary>
		[JsonProperty("temp")]
		public double Temperature { get; set; }

		/// <summary>
		/// Atmosfericky tlak (hPa) v urovni hladiny more, pokud je dostupna. Jinak tlak z urovne zeme.
		/// </summary>
		[JsonProperty("pressure")]
		public double Pressure { get; set; }

		/// <summary>
		/// Vlhkost (%)
		/// </summary>
		[JsonProperty("humidity")]
		public int Humidity { get; set; }

		/// <summary>
		/// Minimalni teplota (Kelvin).
		/// </summary>
		[JsonProperty("temp_min")]
		public double MinTemperature { get; set; }

		/// <summary>
		/// Maximalni teplota (Kelvin)
		/// </summary>
		[JsonProperty("temp_max")]
		public double MaxTemperature { get; set; }

		/// <summary>
		/// Atmosfericky tlak (hPa) v urnovni hladiny more.
		/// </summary>
		[JsonProperty("sea_level")]
		public double SeaLevelPressure { get; set; }

		/// <summary>
		/// Atmosfericky tlak (hPa) v urovni zeme.
		/// </summary>
		[JsonProperty("grnd_level")]
		public double GroundLevelPressure { get; set; }
	}
}
