// Převzato z XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// Autor: Clemens Fischer
// Upravil: Martin Honěk

using System;
using Microsoft.Xna.Framework;
using RadarView.Model.Entities.Geographic;

namespace RadarView.Model.Managers.MapProjection
{
	/// <summary>
	/// Třída reprezentuje web-mercatorovu projeci.
	/// </summary>
	public class WebMercatorProjection : MapProjection
    {
        /// <inheritdoc/>
        public override Vector2 GetMapScale(Location location)
        {
            var scale = (float)(this.ViewportScale / Math.Cos(location.Latitude * Math.PI / 180));
            return new Vector2(scale, scale);
        }

        /// <inheritdoc/>
        public override Vector2 LocationToPoint(Location location)
        {
            return new Vector2(
                this.TrueScale * location.Longitude,
                this.TrueScale * LatitudeToY(location.Latitude));
        }

        /// <inheritdoc/>
        public override Location PointToLocation(Vector2 point)
        {
            return new Location(
                YToLatitude(point.Y / this.TrueScale),
                point.X / this.TrueScale);
        }

        /// <inheritdoc/>
        public override Location TranslateLocation(Location location, Vector2 translation)
        {
            var scaleX = this.TrueScale * this.ViewportScale;
            var scaleY = (float)(scaleX / Math.Cos(location.Latitude * Math.PI / 180));

            return new Location(location.Latitude - translation.Y / scaleY,
                location.Longitude + translation.X / scaleX);
        }

        /// <summary>
        /// Transformuje zeměpisnou šířku (latitude) na Y mapovou souřadnici
        /// </summary>
        /// <param name="latitude">zeměpisná šířka</param>
        /// <returns>souřadnice Y mapy</returns>
        public static float LatitudeToY(float latitude)
        {
            return (float)(Math.Log(Math.Tan((latitude + 90) * Math.PI / 360)) / Math.PI * 180);
        }

        /// <summary>
        /// Transformuje Y mapovou souřadnici na zeměpisnou šířku (latitude)
        /// </summary>
        /// <param name="y">souřadnice Y mapy</param>
        /// <returns>zeměpisnou šířku</returns>
        public static float YToLatitude(double y)
        {
            return (float)(90 - Math.Atan(Math.Exp(-y * Math.PI / 180)) / Math.PI * 360);
        }
    }
}
