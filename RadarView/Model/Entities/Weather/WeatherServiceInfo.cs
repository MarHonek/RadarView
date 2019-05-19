using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RadarView.Model.Entities.Weather
{
	/// <summary>
	/// Reprezentuje pocasi.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class WeatherServiceInfo
	{
		/// <summary>
		/// ID stavu pocasi.
		/// </summary>
		[JsonProperty("id")]
		public int Id { get; set; }

		/// <summary>
		/// Skupina parametru stavu pocasi (Rain, Snow, Extreme atd).
		/// </summary>
		[JsonProperty("main")]
		public string Main { get; set; }

		/// <summary>
		/// Poznamka ke stavu pocasi.
		/// </summary>
		[JsonProperty("description")]
		public string Description { get; set; }

		/// <summary>
		/// ID ikony stavu pocasi.
		/// </summary>
		[JsonProperty("icon")]
		public string Icon { get; set; }
	}
}
