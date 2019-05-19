// Převzato z XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// Autor: Clemens Fischer
// Upravil: Martin Honěk

using System;
using Microsoft.Xna.Framework;
using RadarView.Model.Entities.Geographic;

namespace RadarView.Model.Managers.MapProjection
{
    /// <summary>
    /// Třída definuje mapovou projekci mezi geografickými souřadnicemi, mapovými souřadnicemi (kartézské soustavy) a obrazovými body (pixely).
    /// </summary>
    /// 
    /// (Použití vektoru jako bodu poskytuje větší přesnost, efektivitu a jednoduší transformaci)
    public abstract class MapProjection
    {
        /// <summary>
        /// Size jedné tile.
        /// </summary>
        public const float TileSize = 256;

        /// <summary>
        /// Počet pixelů na jeden zeměpisný stupeň.
        /// </summary>
        public const float PixelPerDegree = TileSize / 360;

        /// <summary>
        /// Poloměr rovníku podle WSG84.
        /// </summary>
        public const float Wgs84EquatorialRadius = 6378137;

        /// <summary>
        /// Počet metrů na jeden zeměpisný stupeň.
        /// </summary>
        public const float MetersPerDegree = (float)(Wgs84EquatorialRadius * Math.PI / 180);

        /// <summary>
        /// Meřítko (scale factor) geografických souřadnic ku mapovým souřadnicím, 
        /// podobné reálnemu měřítku cylindrické projekce
        /// </summary>
        public float TrueScale { get; protected set; } = MetersPerDegree;

        /// <summary>
        /// Tranformační matici pro převod z mapových souřadnic na obrazové body. (pixely).
        /// </summary>
        public Matrix ViewportTransform { get; private set; }

        /// <summary>
        /// Měřítko (scaling factor) mapových souřadnic ku obrazovým bodům (pixelům).
        /// </summary>
        public float ViewportScale { get; protected set; }

        /// <summary>
        /// Získá mapové měřítko pro určenou polohu jako počet obrazových bodů na meter (px/m)
        /// </summary>
        /// <param name="location">zeměpisná poloha</param>
        /// <returns>vektor reprezentující měřítko</returns>
        public abstract Vector2 GetMapScale(Location location);

        /// <summary>
        /// Transformuje geografickou polohu na mapovou souřadnici.
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public abstract Vector2 LocationToPoint(Location location);

        /// <summary>
        /// Tranformuje mapové souřadnice na geografickou polohu.
        /// </summary>
        /// <param name="point">mapová souřadnice</param>
        /// <returns>geografickou polohu</returns>
        public abstract Location PointToLocation(Vector2 point);

        /// <summary>
        /// Posune geografickou polohu podle zadaného vektoru (v pixelech)
        /// </summary>
        /// <param name="location">zeměpisná poloha</param>
        /// <param name="translation">vektor posunu</param>
        /// <returns>nevou geografickou polohu</returns>
        public abstract Location TranslateLocation(Location location, Vector2 translation);

        /// <summary>
        /// Transformuje geografickou polohu na souřidnici pixelu
        /// </summary>
        /// <param name="location">zeměpisná poloha</param>
        /// <returns>souřadnice pixelu</returns>
        public Vector2 LocationToViewportPoint(Location location)
        {
            return Vector2.Transform(this.LocationToPoint(location), this.ViewportTransform);
        }

        /// <summary>
        /// Transformuje obrazový bod na geografickou polohu
        /// </summary>
        /// <param name="point">obrazový bod</param>
        /// <returns>geografickou polohu</returns>
        public Location ViewportPointToLocation(Vector2 point)
        {          
            var invertedMatrix = Matrix.Invert(this.ViewportTransform);
            return this.PointToLocation(Vector2.Transform(point, invertedMatrix));
        }

        /// <summary>
        /// Nastaví viewportScale a hodnoty ViewportTransform
        /// </summary>
        /// <param name="projectionCenter">střed projekce</param>
        /// <param name="mapCenter">střed mapy</param>
        /// <param name="viewportCenter">střed obrazu</param>
        /// <param name="zoomLevel">zoom level</param>
        public virtual void SetViewportTransform(Location projectionCenter, Location mapCenter, Vector2 viewportCenter, float zoomLevel)
        {
            this.ViewportScale = (float)Math.Pow(2d, zoomLevel) * PixelPerDegree / this.TrueScale;
            var center = this.LocationToPoint(mapCenter);
            this.ViewportTransform = this.TranslateScaleTranslate(
                -center.X, -center.Y, this.ViewportScale, -this.ViewportScale, viewportCenter.X, viewportCenter.Y);
        }

        /// <summary>
        /// Vytvoří transformační matici pro zoom v určeném bodů
        /// </summary>
        private Matrix TranslateScaleTranslate(
         float translation1X, float translation1Y,
         float scaleX, float scaleY,
         float translation2X, float translation2Y)
        {
            return Matrix.CreateTranslation(new Vector3(translation1X, translation1Y, 0)) *
                Matrix.CreateScale(new Vector3(scaleX, scaleY, 1)) *
                Matrix.CreateTranslation(new Vector3(translation2X, translation2Y, 0));
        }
    }
}
