using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RadarView.Model.Entities.Weather
{
	/// <summary>
	/// Reprezentuje informace o vetru.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class Wind
	{
		/// <summary>
		/// Rychlost vetru (m/s).
		/// </summary>
		[JsonProperty("speed")]
		public double Speed { get; set; }

		/// <summary>
		/// Směr větru (úhlové stupně)
		/// </summary>
		[JsonProperty("deg")]
		public double Degree { get; set; }
	}
}
