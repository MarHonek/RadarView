using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RadarView.Model.Entities.Weather
{
	/// <summary>
	/// Zemepisna poloha mista, pro kter jsou data platna.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class WeatherCoordinates
	{
		/// <summary>
		/// Zemepisna delka
		/// </summary>
		[JsonProperty("lon")]
		public double Longitude { get; set; }

		/// <summary>
		/// Zemepisn sirka
		/// </summary>
		[JsonProperty("lat")]
		public double Latitude { get; set; }
	}
}
