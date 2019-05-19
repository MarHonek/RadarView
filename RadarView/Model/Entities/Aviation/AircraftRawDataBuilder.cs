using System;
using RadarView.Model.Entities.AviationData;

namespace RadarView.Model.Entities.Aviation
{
    /// <summary>
    /// Builder třídy AircraftRawData.
    /// </summary>
    public class AircraftRawDataBuilder
    {
		/// <summary>
		/// Adresa zařízení přijímací vysílající informace o letadle.
		/// Může byt kód módu S, identifikator zařízení FLARM nebo OGN Tracker.
		/// </summary>
		private string address;

        /// <summary>
        /// Volací znak letadla.
        /// </summary>
        private string callsign;

        /// <summary>
        /// Soutěžní název letadla.
        /// </summary>
        private string competitionName;

        /// <summary>
        /// Kód odpovídače letadla.
        /// </summary>
        private string squawk;

		/// <summary>
		/// Registrační kód letadla.
		/// </summary>
		private string registration;

		/// <summary>
		/// IATA číslo letu.
		/// </summary>
		private string iataFlightNumber;

        /// <summary>
        /// Zeměpisná šířka polohy.
        /// </summary>
        private float? latitude;

        /// <summary>
        /// Zeměpisná délka polohy.
        /// </summary>
        private float? longitude;

        /// <summary>
        /// Časová známka.
        /// </summary>
        private long? timestamp;

        /// <summary>
        /// Výška letadla.
        /// </summary>
        private float? altitude;

        /// <summary>
        /// Směr letu.
        /// </summary>
        private int? track;

        /// <summary>
        /// Rychlost letadla.
        /// </summary>
        private float? groundSpeed;

        /// <summary>
        /// Rychlost stoupání/klesání.
        /// </summary>
        private float? verticalSpeed;

        /// <summary>
        /// Atribut určující zda je letadlo na zemi.
        /// </summary>
        private bool? onGround;

		/// <summary>
		/// Manévr letadla.
		/// </summary>
        private AircraftManeuver maneuver;

        /// <summary>
        /// Typ letadla.
        /// </summary>
        private AircraftType aircraftType;

        /// <summary>
        /// Označení datového zdroje.
        /// </summary>
        private AircraftDataSourceEnum dataSource;

        /// <summary>
        /// Model letadla.
        /// </summary>
        private string model;

        /// <summary>
        /// Jméno posádky letadla.
        /// </summary>
        private string crew;

        /// <summary>
        /// Provozovatel letadla.
        /// </summary>
        private string _operator;

        /// <summary>
        /// Zdrojová země, odkud letadlo letí.
        /// </summary>
        private string originCountry;

        /// <summary>
        /// Mesto odletu.
        /// </summary>
        private string originCity;

        /// <summary>
        /// Cilove mesto letu.
        /// </summary>
        private string destinationCity;

        /// <summary>
        /// Cílová země, kam letadlo letí.
        /// </summary>
        private string destinationCountry;


        public AircraftRawDataBuilder Address(string address)
        {
            this.address = address;
            return this;
        }

        public AircraftRawDataBuilder CallSign(string callSign)
        {
            this.callsign = callSign;
            return this;
        }

        public AircraftRawDataBuilder Registration(string registration)
        {
            this.registration = registration;
            return this;
        }

		public AircraftRawDataBuilder IataFlightNumber(string iataFlightNumber)
		{
			this.iataFlightNumber = iataFlightNumber;
			return this;
		}

		public AircraftRawDataBuilder CompetitionName(string competitionName)
        {
            this.competitionName = competitionName;
            return this;
        }

        public AircraftRawDataBuilder Squawk(string squawk)
        {
            this.squawk = squawk;
            return this;
        }

        public AircraftRawDataBuilder Latitude(float? latitude)
        {
            this.latitude = latitude;
            return this;
        }

        public AircraftRawDataBuilder Longitude(float? longitude)
        {
            this.longitude = longitude;
            return this;
        }

        public AircraftRawDataBuilder Timestamp(long? timestamp)
        {
            this.timestamp = timestamp;
            return this;
        }

        public AircraftRawDataBuilder Altitude(float? altitude)
        {
            this.altitude = altitude;
            return this;
        }

        public AircraftRawDataBuilder Track(int? track)
        {
            this.track = track;
            return this;
        }

        public AircraftRawDataBuilder GroundSpeed(float? groundSpeed)
        {
            this.groundSpeed = groundSpeed;
            return this;
        }

        public AircraftRawDataBuilder VerticalSpeed(float? verticalSpeed)
        {
            this.verticalSpeed = verticalSpeed;
            return this;
        }

        public AircraftRawDataBuilder OnGround(bool? onGround)
        {
            this.onGround = onGround;
            return this;
        }

        public AircraftRawDataBuilder Maneuver(AircraftManeuver maneuver)
        {
	        this.maneuver = maneuver;
	        return this;
        }

		public AircraftRawDataBuilder AircraftType(AircraftType aircraftType)
        {
            this.aircraftType = aircraftType;
            return this;
        }

        public AircraftRawDataBuilder DataSource(AircraftDataSourceEnum dataSource)
        {
			this.dataSource = dataSource;
            return this;
        }


        public AircraftRawDataBuilder Model(string model)
        {
            this.model = model;
            return this;
        }


        public AircraftRawDataBuilder Crew(string crew)
        {
            this.crew = crew;
            return this;
        }

        public AircraftRawDataBuilder Operator(string _operator)
        {
            this._operator = _operator;
            return this;
        }

        public AircraftRawDataBuilder OriginCountry(string originCountry)
        {
            this.originCountry = originCountry;
            return this;
        }

        public AircraftRawDataBuilder DestinationCountry(string destinationCountry)
        {
            this.destinationCountry = destinationCountry; 
            return this;
        }

        public AircraftRawDataBuilder OriginCity(string originCity)
        {
            this.originCity = originCity;
            return this;
        }

        public AircraftRawDataBuilder DestinationCity(string destinationCity)
        {
            this.destinationCity = destinationCity;
            return this;
        }

        public AircraftRawData Build()
        {
	        DateTime? dateTime = null;
	        if (this.timestamp.HasValue) {
		        dateTime = DateTimeOffset.FromUnixTimeSeconds(this.timestamp.Value).DateTime;
	        }
            return new AircraftRawData(this.address, this.callsign, this.competitionName, this.squawk, this.registration, this.iataFlightNumber, this.latitude, this.longitude, dateTime, this.altitude,
                this.track, this.groundSpeed, this.verticalSpeed, this.onGround, this.aircraftType, this.model, this.crew, this._operator, this.maneuver, this.dataSource, this.originCountry, this.destinationCountry, this.originCity, this.destinationCity);
        }
    }
}
