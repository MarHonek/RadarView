using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleJson;

namespace RadarView.Model.Entities.Weather
{
	/// <summary>
	/// Reprezentuje informace o stavu dne.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class WeatherDayInfo
	{
		/// <summary>
		/// Interni parameter.
		/// </summary>
		[JsonProperty("message")]
		public double Message { get; set; }

		/// <summary>
		/// Kod statu.
		/// </summary>
		[JsonProperty("country")]
		public string Country { get; set; }

		/// <summary>
		/// UTC casova znamka vychody slunce.
		/// </summary>
		[JsonProperty("sunrise")]
		public int Sunrise { get; set; }

		/// <summary>
		/// UTC casova znamka zapadu slunce.
		/// </summary>
		[JsonProperty("sunset")]
		public int Sunset { get; set; }
	}
}
