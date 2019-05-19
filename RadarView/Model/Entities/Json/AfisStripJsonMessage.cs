using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SimpleJson;

namespace RadarView.Model.Entities.Json
{
	/// <summary>
	/// Objekt de/serializace zpráv pro komunikaci s aplikacemi 3. stran pomocí UDP protokolu.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class AfisStripJsonMessage
	{
		/// <summary>
		/// Seznam identifikátorů aktivních prostorů.
		/// </summary>
		[JsonProperty("activeAirspaces")]
		public List<string> ActiveAirspace { get; set; }

		/// <summary>
		/// Kolekce obsahující identifikátory letadel a jejich zvýraznění.
		/// Klíč: Identifikátor letadel.
		/// Hodnota: Typ zvýraznění.
		/// </summary>
		[JsonProperty("highlightedAircrafts")]
		public Dictionary<string, TargetHighlight> HighlightedAircraft { get; set; }
	}
}
