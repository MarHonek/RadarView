using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using OsnAPI.Utils;
using UnitsNet;

namespace OsnAPI.Aircraft
{
    /// <summary>
    /// Třída definuje objekt pbsahující aktuální data o poloze, výšce, rychlosti atd.
    /// </summary>
    public class OsnAircraftStateVector : ICloneable
    {
        #region properties
        /// <summary>
        /// Vrací unikátní 24-bitová ICAO adresa transpondéru v hexadecimálním tvaru.
        /// </summary>
        [JsonProperty("modeSCode")]
        public string ModeSCode { get; }

		/// <summary>
		/// Vrací volací znak (8 znaků). Může být null, jestliže volací znak nebyl přijat.
		/// </summary>
		[JsonProperty("callsign")]
		public string Callsign { get; }

		/// <summary>
		/// Vrací název státu odvozený z ICAO adresy.
		/// </summary>
		[JsonProperty("originCountry")]
		public string OriginCountry { get; }

		/// <summary>
		/// Vrací unix časovou známku (sekundy) poslední aktualizace polohy. Může být null pokud OpenSky nepřijal hlášení o poloze za posledních 15s.
		/// </summary>
		[JsonProperty("timePosition")]
		public long? TimePosition { get; }

		/// <summary>
		/// Vrací unix časovou známku (sekundy) poslední aktualizace obecně. 
		/// </summary>
		[JsonProperty("lastContact")]
		public long LastContact { get; internal set; }

		/// <summary>
		/// Vrací zeměpisnou délku ve stupních (desítková soustava). Může být null.
		/// </summary>
		[JsonProperty("longitude")]
		public float? Longitude { get; }

		/// <summary>
		/// Vrací zeměpisnou šířku ve stupních (desítková soustava). Může být null.
		/// </summary>
		[JsonProperty("latitude")]
		public float? Latitude { get; }

		/// <summary>
		/// Vrací bool hodnotu, určující zda je letadlo na zemi.
		/// </summary>
		[JsonProperty("onGround")]
		public bool OnGround { get; }

		/// <summary>
		/// Vrací kurz ve stupních (podle hodinových ručiček) od severu (0°). Může být null.
		/// </summary>
		[JsonProperty("heading")]
		public int? Heading { get; }

		/// <summary>
		/// Vrací kód odpovídače A/C (squawk). Může být null.
		/// </summary>
		[JsonProperty("squawk")]
		public string Squawk { get; }

		/// <summary>
		/// Vrací true pokud 'flight status' značí zvláštní účel.
		/// </summary>
		[JsonProperty("spi")]
		public bool Spi { get; }

        /// <summary>
        /// Vrací zdroj polohových dat: 0 = ADS-B, 1 = ASTERIX, 2 = MLAT
        /// </summary>
        [JsonProperty("positionSource")]
		public PositionSource PositionSource { get; }

		/// <summary>
		/// Vrací rychlost vůči zemi (m/s). Může být null.
		/// </summary>
		[JsonProperty("velocity")]
		public float? Velocity { get; }

		/// <summary>
		/// Vrací rychlost stoupání/klesání (m/s). Kladná hodnota značí stoupání, záporná klesání. Může být null.
		/// </summary>
		[JsonProperty("verticalRate")]
		public float? VerticalRate { get; }

		/// <summary>
		/// Vrací nadmořskou výšku získanou z barometrického výškoměru (metry). Může být null.
		/// </summary>
		[JsonProperty("baroAltitude")]
		public float? BaroAltitude { get; }

		/// <summary>
		/// Vrací nadmořskou výšku získanou z GPS (metry). Může být null.
		/// </summary>
		[JsonProperty("geoAltitude")]
		public float? GeoAltitude { get; }

        #endregion

        [JsonConstructor]
        public OsnAircraftStateVector(string modeSCode, string callsign, string originCountry, long? timePosition, long lastContact, float? longitude, float? latitude, float? geoAltitude, 
            bool onGround, float? velocity, int? heading, float? verticalRate, float? baroAltitude, string squawk, bool spi, PositionSource positionSource)
        {
            this.ModeSCode = modeSCode;
            this.Callsign = callsign;
            this.OriginCountry = originCountry;
            this.TimePosition = timePosition;
            this.LastContact = lastContact;
            this.Longitude = longitude;
            this.Latitude = latitude;
            this.GeoAltitude = geoAltitude;
            this.OnGround = onGround;
            this.Velocity = velocity;
            this.Heading = heading;
            this.VerticalRate = verticalRate;
            this.BaroAltitude = baroAltitude;
            this.Squawk = squawk;
            this.Spi = spi;
            this.PositionSource = positionSource;
        }

        /// <summary>
        /// Konstruktor inicializuje instanci třídy OsnAircrft. Otypuje data v zadaném seznamu.
        /// </summary>
        /// <param name="stateVector">data o letadlech</param>
        public OsnAircraftStateVector(List<object> stateVector)
        {


            this.ModeSCode = stateVector[0].ToString();
            this.Callsign = ((string)stateVector[1]).NullIfWhiteSpace();
            this.OriginCountry = ((string)stateVector[2]).NullIfWhiteSpace();
            this.TimePosition = stateVector[3].ParseToLongOrNull();
            this.LastContact = (long)stateVector[4];
            this.Longitude = stateVector[5].ParseToFloatOrNull();
            this.Latitude = stateVector[6].ParseToFloatOrNull();
            this.GeoAltitude = stateVector[7].ParseToFloatOrNull();
            this.OnGround = (bool)stateVector[8];
            this.Velocity = stateVector[9].ParseToFloatOrNull();
            this.Heading = stateVector[10].ParseToIntOrNull();
            this.VerticalRate = stateVector[11].ParseToFloatOrNull();
            this.BaroAltitude = stateVector[13].ParseToFloatOrNull();
            this.Squawk = ((string)stateVector[14]).NullIfWhiteSpace();
            this.Spi = (bool)stateVector[15];
            this.PositionSource = (PositionSource)(Convert.ToInt32(stateVector[16]));
        }

        public override bool Equals(object obj)
        {
	        if (obj == null) {
		        return false;
	        }

	        var stateVector = obj as OsnAircraftStateVector;
	        return stateVector != null && this.ModeSCode == stateVector.ModeSCode;
        }

        public override int GetHashCode()
        {
            return this.ModeSCode.GetHashCode();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
	        var aircraftString = new StringBuilder();
	        foreach (var propertyInfo in this.GetType().GetProperties()) {
		        aircraftString.Append(propertyInfo.Name + ": " + propertyInfo.GetValue(this) + "; ");
	        }

	        return aircraftString.ToString();
        }
    }
}
