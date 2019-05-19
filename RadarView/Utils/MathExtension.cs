using Microsoft.Xna.Framework;
using System;
using RadarView.Model.Entities.Geographic;

namespace RadarView.Utils
{
    /// <summary>
    /// Třída óbsahuje matematické metody.
    /// </summary>
    public static class MathExtension
    {

        /// <summary>
        /// Převede radiány na stupně
        /// </summary>
        /// <param name="radian">hodnota v radiánech</param>
        /// <returns>stupně odpovídající radiánům</returns>
        public static double RadianToDegree(double radian)
        {
            return radian * (180.0 / Math.PI);
        }

        /// <summary>
        /// Převede stupně na radiány
        /// </summary>
        /// <param name="angle">hodnota ve stupních</param>
        /// <returns>radiány odpovídající stupňům</returns>
        public static double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }

        /// <summary>
        /// Převede metry za sekundu na uzly.
        /// </summary>
        /// <param name="metersPerSecond">Metry za sekundu.</param>
        /// <returns>Uzly.</returns>
        public static float MetersPerSecondsToKnots(float metersPerSecond)
        {
            return metersPerSecond * 1.9438444924574f;
        }

        /// <summary>
        /// Převede uzly na kilometry za hodinu.
        /// </summary>
        /// <param name="knots">Uzly.</param>
        /// <returns>Kilometry za hodinu.</returns>
        public static float KnotsToKilometersPerHour(float knots)
        {
            return knots * 1.852f;
        }

        /// <summary>
        /// Převede metry na stopy.
        /// </summary>
        /// <param name="meters">Metry.</param>
        /// <returns>Stopy.</returns>
        public static float MetersToFeet(float meters)
        {
             return meters / 0.3048f;
        }

        /// <summary>
        /// Převede uzly na metry za sekundu.
        /// </summary>
        /// <param name="knots">Uzly.</param>
        /// <returns>Metry za sekundu.</returns>
        public static float KnotsToMetersPerSecond(float knots)
        {
            return knots * 0.514444444f;
        }

        /// <summary>
        /// Převede stopy za minutu na metry za sekundu.
        /// </summary>
        /// <param name="feetPerMinuteToMetersPerSecond">Stopy za minutu.</param>
        /// <returns>Metry za sekundu.</returns>
        public static float FeetPerMinuteToMetersPerSecond(float feetPerMinuteToMetersPerSecond)
        {
            return feetPerMinuteToMetersPerSecond * 0.0051f;
        }

        /// <summary>
        /// Převede stopy na metry.
        /// </summary>
        /// <param name="feet">Stopy.</param>
        /// <returns>Metry.</returns>
        public static float FeetToMeters(float feet)
        {
            return feet * 0.3048f;
        }

        /// <summary>
        /// Převede metry za sekundu na kilometry za hodinu.
        /// </summary>
        /// <param name="metersPerSecond">metry za sekundu.</param>
        /// <returns>Kilometry za hodinu.</returns>
        public static float MetersPerSecondToKilometersPerHour(float metersPerSecond)
        {
            return metersPerSecond * 3.6f;
        }

		/// <summary>
		/// Provede lineární interpolaci mezi časem 0 a 1.
		/// </summary>
		/// <param name="v0">hodnota v čase 0</param>
		/// <param name="v1">hodnota v čase 1</param>
		/// <param name="t">čas pro hledanou hodnotu.</param>
		/// <returns>Interpolovanou hodnotu.</returns>
		public static float LinearInterpolate(float v0, float v1, float t)
		{
			return (1 - t) * v0 + t * v1;
		}

		/// <summary>
		/// Provede lineární interpolaci mezi libovolnými časy.
		/// </summary>
		/// <param name="v0">hodnota v čase x</param>
		/// <param name="v1">hodnota v čase y</param>
		/// <param name="t0">čas x</param>
		/// <param name="t1">čas y</param>
		/// <param name="tx">čas pro hledanou hodnotu.</param>
		/// <returns>Interpolovanou hodnotu.</returns>
		public static float LinearInterpolateInBoundlessInterval(float v0, float v1, int t0, int t1, int tx)
		{
			return LinearInterpolate(v0, v1, (tx - t0) / ((float)(t1 - t0)));
		}


		/// <summary>
		/// Provede lineární interpolaci mezi libovolnými časy.
		/// </summary>
		/// <param name="v0">hodnota v čase x</param>
		/// <param name="v1">hodnota v čase y</param>
		/// <param name="t0">čas x</param>
		/// <param name="t1">čas y</param>
		/// <param name="tx">čas pro hledanou hodnotu.</param>
		/// <returns>Interpolovanou hodnotu.</returns>
		public static float LinearInterpolateInBoundlessInterval(float v0, float v1, DateTime t0, DateTime t1, DateTime tx)
		{
			return LinearInterpolate(v0, v1, (int)(tx - t0).TotalSeconds / (float)(t1 - t0).TotalSeconds);
		}


		/// <summary>
		/// Převede vektor na úhel.
		/// </summary>
		/// <param name="vector">vektor úhlu</param>
		/// <returns>úhel svírající <see cref="vector"/> a referenční vektor (°0)</returns>
		public static float VectorToAngle(Vector2 vector)
		{
			vector.Normalize();
			return (float)Math.Atan2(vector.X, -vector.Y);
		}


		/// <summary>
		/// Určí kurz ze dvou bodů.
		/// </summary>
		/// <param name="startLocation">počáteční poloha</param>
		/// <param name="endLocation">koncová poloha</param>
		/// <returns>Azimuth mezi body.</returns>
		public static double DetermineAzimuth(Location startLocation, Location endLocation)
		{
			var latStart = DegreeToRadian(startLocation.Latitude);
			var latEnd = DegreeToRadian(endLocation.Latitude);
			var lonDiff = DegreeToRadian(endLocation.Longitude) - DegreeToRadian(startLocation.Longitude);

			var y = Math.Sin(lonDiff) * Math.Cos(latEnd);
			var x = Math.Cos(latStart) * Math.Sin(latEnd) - Math.Sin(latStart) * Math.Cos(latEnd) * Math.Cos(lonDiff);
			var courseInRad = Math.Atan2(y, x);

			return (RadianToDegree(courseInRad) + 360) % 360;
		}
	}
}
