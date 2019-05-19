using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region License
//   Copyright 2010 John Sheehan
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//	 http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License. 
#endregion
#region Acknowledgements
// Original JsonSerializer contributed by Daniel Crenna (@dimebrain)
#endregion

using System.IO;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Deserializers;
using RestSharp.Serializers;

namespace RadarView.Model.Service.Utils
{
	/// <summary>
	/// Default JSON serializer for request bodies
	/// Doesn't currently use the SerializeAs attribute, defers to Newtonsoft's attributes
	/// </summary>
	public class RestSharpJsonNetSerializer : ISerializer, IDeserializer
	{
		private Newtonsoft.Json.JsonSerializer serializer;

		public RestSharpJsonNetSerializer(Newtonsoft.Json.JsonSerializer serializer)
		{
			this.serializer = serializer;
		}

		public string ContentType
		{
			get { return "application/json"; } // Probably used for Serialization?
			set { }
		}

		public string DateFormat { get; set; }
		public string Namespace { get; set; }
		public string RootElement { get; set; }

		public string Serialize(object obj)
		{
			using (var stringWriter = new StringWriter()) {
				using (var jsonTextWriter = new JsonTextWriter(stringWriter)) {
					this.serializer.Serialize(jsonTextWriter, obj);
					return stringWriter.ToString();
				}
			}
		}

		public T Deserialize<T>(IRestResponse response)
		{
			var content = response.Content;
			using (var stringReader = new StringReader(content)) {
				using (var jsonTextReader = new JsonTextReader(stringReader)) {
					return this.serializer.Deserialize<T>(jsonTextReader);
				}
			}
		}

		public static RestSharpJsonNetSerializer Default
		{
			get
			{
				return new RestSharpJsonNetSerializer(new Newtonsoft.Json.JsonSerializer() {
					NullValueHandling = NullValueHandling.Ignore,
				});
			}
		}
	}
}