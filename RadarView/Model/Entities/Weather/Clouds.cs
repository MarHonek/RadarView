using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RadarView.Model.Entities.Weather
{
	/// <summary>
	/// Reprezentuje informace o oblacnosti.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class Clouds
	{
		/// <summary>
		/// Oblacnost (%).
		/// </summary>
		[JsonProperty("all")]
		public int CloudPercent { get; set; }
	}
}
