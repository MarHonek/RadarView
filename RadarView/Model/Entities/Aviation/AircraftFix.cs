using System;
using RadarView.Model.Entities.Geographic;

namespace RadarView.Model.Entities.Aviation
{
    /// <summary>
    /// Reprezentuje polohu letadla v čase jako bod 4D prostoru.
    /// </summary>
    public struct AircraftFix
    {
        //Poloha letadla
        public Location Location { get; }

        //Výška letadla (metry)
        public float Altitude { get; }

        //Čas.
        public DateTime DateTime { get; }


        /// <summary>
        /// Inicializuje instanci třídy AircraftFix
        /// </summary>
        /// <param name="location">geografická poloha</param>
        /// <param name="altitude">výška</param>
        /// <param name="dateTime">Čas.</param>
        public AircraftFix(Location location, float altitude, DateTime dateTime)
        {
            this.Location = location;
            this.Altitude = altitude;
            this.DateTime = dateTime;
        }

		public override string ToString()
		{
			return this.Location + ", altitude: " + this.Altitude + ", timestamp: " + this.DateTime;
		}
	}
}
