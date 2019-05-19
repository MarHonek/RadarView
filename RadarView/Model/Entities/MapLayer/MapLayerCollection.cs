using System.Collections.Generic;
using Newtonsoft.Json;

namespace RadarView.Model.Entities.MapLayer
{
	/// <summary>
	/// Kolekce mapových podkladů.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class MapLayerCollection
	{
		/// <summary>
		/// Seznam mapových podkladů.
		/// </summary>
		[JsonProperty("mapLayers")]
		public List<MapLayer> Layers { get; set; }
	}
}
