using System;
using System.Collections.Generic;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Abstract;

namespace RadarView.Model.Render.Background.Airspace
{
    /// <summary>
    /// Reprentuje vzdušný prostor jako objekt vykreslení
    /// </summary>
    public class BackgroundAirspace : RenderObject, IRenderable
    {
        /// <summary>
        /// Vzdušný prostor
        /// </summary>
        public Entities.AviationData.Airspace.Airspace Airspace { get; }

        /// <summary>
        /// Body (ve viewportu) polygonu určijící tvar a polohu vzdušného prostoru
        /// </summary>
        private List<Vector2> verticesPosition;

        /// <summary>
        /// Mapová projekce
        /// </summary>
        private IViewportProjection projection;


        public BackgroundAirspace(Entities.AviationData.Airspace.Airspace airspace, IViewportProjection projection, Color color) : base(color)
        {
            this.Airspace = airspace;
            this.projection = projection;
            this.projection.ViewportChanged += this.Projector_ViewportChanged;
            this.UpdatePosition();         
        }

        /// <summary>
        /// Přepočítá body polygonu reprezentující vzdušný prostor
        /// </summary>
        private void UpdatePosition()
        {
            var verticesPosition = new List<Vector2>();
            this.Airspace.Geometry.Coordinates.ForEach(location => verticesPosition.Add(this.projection.LocationToViewportPoint(location)));
            this.verticesPosition = verticesPosition;
        }

        private void Projector_ViewportChanged(object sender, EventArgs e)
        {
            //Aktualizuje pozice vzdušného prostoru při změně viewportu.
            this.UpdatePosition();
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {
            drawBatch.DrawPrimitivePath(this.Pen, this.verticesPosition);
        }
    }
}
