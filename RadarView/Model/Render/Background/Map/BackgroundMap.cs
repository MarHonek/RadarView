using System;
using System.Collections.Generic;
using System.Linq;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Abstract;
using BoundingBox = RadarView.Model.Entities.Geographic.BoundingBox;

namespace RadarView.Model.Render.Background.Map
{
    /// <summary>
    /// Třídá starající se o vykreslení mapového podkladu.
    /// </summary>
    public class BackgroundMap : RenderObject, IRenderable
    {
		/// <summary>
		/// Aktuálně zobrazovaná textura mapy.
		/// </summary>
	    private Texture2D currentMapTexture;

        /// <summary>
        /// Bod (ve viportu) levého horního rohu mapy.
        /// </summary>
        private Vector2 topLeftCornerPoint;

        /// <summary>
        /// Bod (ve viewportu) pravého dolního rohu mapy.
        /// </summary>
        private Vector2 rightBottomCornerPoint;

        /// <summary>
        /// Mapová projekce.
        /// </summary>
        private IViewportProjection _mapProjection;

		/// <summary>
		/// Hranice mapy pro mapove levely
		/// </summary>
		private Dictionary<int, BoundingBox> boundingBoxesForMapLevels;

		/// <summary>
		/// Hranice mapy pro mapový levely
		/// </summary>
		private Dictionary<int, Texture2D> texturesForMapLevels;

		/// <summary>
		/// Seřazené vrstvy podle hodnoty zoom levelu.
		/// </summary>
		private List<int> orderedMapLevels;

		/// <summary>
		/// Aktuální zoom level.
		/// </summary>
		int minZoomLevel;


        /// <summary>
        /// Inicializuje instanci třídy BackgroundMap
        /// </summary>
        /// <param name="mapProjection">mapová projekce</param>
        /// <param name="color">barva mapy</param>
        public BackgroundMap(IViewportProjection mapProjection, Color color, Dictionary<int, BoundingBox> boundingBoxesForMapLevels, Dictionary<int, Texture2D> texturesForMapLevels) : base(color)
        {
            this._mapProjection = mapProjection;
			this.boundingBoxesForMapLevels = boundingBoxesForMapLevels;
			this.texturesForMapLevels = texturesForMapLevels;
            this.ChangeMapLevelTexture(mapProjection.ZoomLevel);
            mapProjection.ViewportChanged += this.Projector_ViewportChanged;

			this.orderedMapLevels = this.boundingBoxesForMapLevels.Keys.OrderBy(x => x).ToList();
			this.minZoomLevel = this.orderedMapLevels.Min();
        }

        private void Projector_ViewportChanged(object sender, EventArgs e)
        {
            this.ChangeMapLevelTexture(this._mapProjection.ZoomLevel);
        }

        /// <summary>
        /// Změní texturu mapy na základě hodnoty zoom.
        /// </summary>
        /// <param name="zoomLevel">zoom level viewportu</param>
        private void ChangeMapLevelTexture(float zoomLevel)
        {
			var zoomLevelInt = (int)zoomLevel;

			if (this.CanMapLevelBeDisplayed(zoomLevelInt)) {
				this.DetermineCorners(this.boundingBoxesForMapLevels[zoomLevelInt]);
				this.currentMapTexture = this.texturesForMapLevels[zoomLevelInt];
			} else {
				var nearestSmallerMapLevel = this.orderedMapLevels.FindLast(x => x < zoomLevel);

				if (nearestSmallerMapLevel <= this.minZoomLevel) {
					nearestSmallerMapLevel = this.minZoomLevel;
				}

				this.currentMapTexture = this.texturesForMapLevels[nearestSmallerMapLevel];
				this.DetermineCorners(this.boundingBoxesForMapLevels[nearestSmallerMapLevel]);
			}
        }

		/// <summary>
		/// Určí zda jsou k dispozici všechny data pro zobrazení mapy pro zadaný map level.
		/// </summary>
		/// <param name="mapLevel">Map level</param>
		/// <returns>true pokud je možné mapu zobrazit, jinak false.</returns>
		private bool CanMapLevelBeDisplayed(int mapLevel) 
		{
			return this.boundingBoxesForMapLevels.ContainsKey(mapLevel) && this.texturesForMapLevels.ContainsKey(mapLevel);
		}

        /// <summary>
        /// Nastaví rohy mapy podle zadaného boundingBoxu
        /// </summary>
        /// <param name="boundingBox">hranice prostoru</param>
        private void DetermineCorners(BoundingBox boundingBox)
        {
            this.topLeftCornerPoint = this._mapProjection.LocationToViewportPoint(new Location(boundingBox.North, boundingBox.West));
            this.rightBottomCornerPoint = this._mapProjection.LocationToViewportPoint(new Location(boundingBox.South, boundingBox.East));
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {
	        if (this.currentMapTexture != null) {
		        spriteBatch.Draw(this.currentMapTexture, new Rectangle((int) this.topLeftCornerPoint.X,
			        (int) this.topLeftCornerPoint.Y,
			        (int) this.rightBottomCornerPoint.X - (int) this.topLeftCornerPoint.X,
			        (int) this.rightBottomCornerPoint.Y - (int) this.topLeftCornerPoint.Y), this.Color);
	        }
        }
    }
}
