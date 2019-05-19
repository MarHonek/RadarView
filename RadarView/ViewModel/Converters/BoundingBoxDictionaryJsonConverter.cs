using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RadarView.Model.Entities.Geographic;

namespace RadarView.ViewModel.Converters
{
	/// <summary>
	/// Konverter, který převede kolekci boundingBoxu do JSON a zpět.
	/// </summary>
	public class BoundingBoxDictionaryJsonConverter : JsonConverter<Dictionary<int, BoundingBox>>
	{
		public override void WriteJson(JsonWriter writer, Dictionary<int, BoundingBox> value, JsonSerializer serializer)
		{
			var dictWithString = new Dictionary<int, string>();
			foreach (var zoomLevel in value) {
				dictWithString[zoomLevel.Key] = zoomLevel.Value.ToString();
			}

			writer.WriteValue(JsonConvert.SerializeObject(dictWithString));
		}

		public override Dictionary<int, BoundingBox> ReadJson(JsonReader reader, Type objectType,
			Dictionary<int, BoundingBox> existingValue, bool hasExistingValue,
			JsonSerializer serializer)
		{
			var obj = JObject.Load(reader);
			var properties = obj.Properties();

			var result = new Dictionary<int, BoundingBox>();
			foreach (var property in properties) {
				result[Int32.Parse(property.Name)] = new BoundingBox(property.Value.ToString());
			}

			return result;
		}
	}
}
