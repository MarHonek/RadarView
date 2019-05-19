using System.Linq;
using Newtonsoft.Json;

namespace RadarView.Model.Entities.Aviation
{
    /// <summary>
    /// Reprezentujici dodatečné informace o letadlech.
    /// </summary>
    public class AircraftInfo
    {
        /// <summary>
        /// Soutěžní název letala
        /// </summary>
        public string CompetitionName { get; set; }

        /// <summary>
        /// Typ letadla
        /// </summary>
        public AircraftType AircraftType { get; set; }

        /// <summary>
        /// Model letadla
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// Jméno posádky letadla
        /// </summary>
        public string Crew { get; set; }

        /// <summary>
        /// Provozovatel letadla
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Kód letiště odkud letadlo odlétá.
        /// </summary>
        public string OriginAirportCode { get; set; }

        /// <summary>
        /// Kód letiště kam letadlo přilétá.
        /// </summary>
        public string DestinationAirportCode { get; set; }

		/// <summary>
		/// Vytvoří novou instanci třídy <see cref="AircraftInfo"/>
		/// </summary>
		/// <param name="competitionName">Soutěžní název letadla.</param>
		/// <param name="aircraftType">Typ letadla.</param>
		/// <param name="model">Model letadla.</param>
		/// <param name="crew">Posádka letadla.</param>
		/// <param name="_operator">Provozovatel letadla.</param>
		/// <param name="originCountry">Země odletu.</param>
		/// <param name="destinationCountry">Země příletu.</param>
		/// <param name="originAirportCode">Kód letiště odletu.</param>
		/// <param name="destinationAirportCode">Kód letiště příletu.</param>
        [JsonConstructor]
        public AircraftInfo(string competitionName, AircraftType aircraftType,
            string model, string crew, string _operator, string originCountry, string destinationCountry, string originAirportCode, string destinationAirportCode)
        {
            this.CompetitionName = competitionName;
            this.AircraftType = aircraftType;
            this.Model = model;
            this.Crew = crew;
            this.Operator = _operator;
            this.OriginAirportCode = originAirportCode;
            this.DestinationAirportCode = destinationAirportCode;
        }

		/// <summary>
		/// Zkombinuje (doplní prázdné) informace o letadle příchozí z datového zdroje. 
		/// </summary>
		/// <param name="newValues">Množina nových informací.</param>
		public void Combine(AircraftInfo newValues)
		{
			var t = typeof(AircraftInfo);
			var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);
			foreach (var prop in properties) {
				var value = prop.GetValue(newValues);
				if (value != null) {
					if (prop.GetValue(this) == null) {
						prop.SetValue(this, value);
					}
				}
			}
		}
	}
}
