using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using RadarView.Utils;

namespace RadarView.ViewModel.Converters
{
	/// <summary>
	/// Json Converter pro převod string hodnoty (#rrggbb).
	/// </summary>
	public class StringToColorJsonConverter : JsonConverter<Color>
	{
		public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
		{
			var colorInHexFormat = reader.Value as string;
			return ColorUtil.HexToColor(colorInHexFormat);
		}
	}
}
