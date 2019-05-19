using System.Collections.Generic;
using Newtonsoft.Json;

namespace OsnAPI.Client
{
	/// <summary>
	/// JSON Atributy.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class JsonStateVectorObject
    {
		/// <summary>
		/// Globální unix časová známka (sekundy).
		/// </summary>
		[JsonProperty("time")]
		public long Time { get; set; }

        /// <summary>
        /// Seznam dat o letadlech.
        /// </summary>
        [JsonProperty("states")]
        public List<List<object>> States { get; set; }
    }
}
