using System;
using System.Device.Location;
using Microsoft.Xna.Framework;
using RadarView.Utils;

namespace RadarView.Model.Entities.Geographic
{
    /// <summary>
    /// Reprezentuje zemepisnou polohu danou zemepisnou sirkou a delkou ve stupnich.
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Polomer zeme (metr).
        /// </summary>
        private const int EarthRadiusInMeters = 6378000;

        /// <summary>
        /// Zemepisna sirka (stupně [desitkove]).
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// Zemepisna delka (stupně [desitkove]).
        /// </summary>
        public float Longitude { get; set; }

        public Location()
        { }

        /// <summary>
        /// Konstruktor inicializuje polohu zadanou zemepisnou delkou a sirkou.
        /// </summary>
        /// <param name="latitude">zeměpisná šířka</param>
        /// <param name="longitude">zeměpisná délka</param>
        public Location(float latitude, float longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// Vypocita vzdalenost mezi dvema zemepisnymi polohami.
        /// </summary>
        /// <param name="location">zemepisna poloha.</param>
        /// <returns>vzdalenost v metrech.</returns>
        public double GetDistanceTo(Location location)
        {
            var startCoordinate = new GeoCoordinate(this.Latitude, this.Longitude);
            var endCoordinate = new GeoCoordinate(location.Latitude, location.Longitude);
            return startCoordinate.GetDistanceTo(endCoordinate);
        }

        /// <summary>
        /// Vypocita geografickou polohu na zaklade picatecni polohy a smeroveho vektoru.
        /// </summary>
        /// <param name="location">pocatecni poloha.</param>
        /// <param name="distance">vzdalenost (smerovy vektor) v metrech.</param>
        /// <returns>Vypoctena zemepisna poloha.</returns>
        public static Location TranslateLocation(Location location, Vector2 distance)
        {
            var latitude = (float)(location.Latitude + MathExtension.RadianToDegree((distance.Y / EarthRadiusInMeters)));
            var longitude = (float)(location.Longitude + MathExtension.RadianToDegree((distance.X / EarthRadiusInMeters)) /
                Math.Cos(MathExtension.DegreeToRadian(location.Latitude)));
            return new Location(latitude, longitude);
        }

        public bool Equals(Location location)
        {
            return location != null
                && Math.Abs(location.Latitude - this.Latitude) < 1e-9
                && Math.Abs(location.Longitude - this.Longitude) < 1e-9;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Location);
        }

        public override int GetHashCode()
        {
            return this.Latitude.GetHashCode() ^ this.Longitude.GetHashCode();
        }

		public override string ToString()
		{
			return "Latitude: " + this.Latitude.ToString() + ", Longitude: " + this.Longitude.ToString(); 
		}
	}
}
