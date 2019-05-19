using System.Collections.Generic;
using System.Windows.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using RadarView.Model.Entities.Geographic;
using RadarView.ViewModel.Converters;

namespace RadarView.Model.Entities.MapLayer
{
	/// <summary>
	/// Třída reprezentuje mapový podklad
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class MapLayer
	{
		/// <summary>
		/// Název mapy.
		/// </summary>
		[JsonProperty("name")]
		public string Name { get; set; }


		/// <summary>
		/// Kolekce nazvu textur pro úrovně zoomu.
		/// </summary>
		[JsonProperty("imageName")]
		public string ImageName { get; set; }

		/// <summary>
		/// Hranice obrázku pro úrovně zoomu .
		/// </summary>
		[JsonProperty("boundingBoxes")]
		[JsonConverter(typeof(BoundingBoxDictionaryJsonConverter))]
		public Dictionary<int, BoundingBox> BoundingBoxes { get; set; }

		/// <summary>
		/// Příznak určující zda je možné mapu vykreslit.
		/// </summary>
		public bool Renderable { get; set; } = true;


		/// <summary>
		/// Příznak určující zda je mapa zobrazena.
		/// </summary>
		[JsonProperty("isVisible")]
		public bool IsVisible { get; set; }

		/// <summary>
		/// Podkladové obrázky.
		/// </summary>
		public Dictionary<int, Texture2D> Textures { get; set; }
	}
}
