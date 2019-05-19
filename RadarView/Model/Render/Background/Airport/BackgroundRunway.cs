using System;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Abstract;

namespace RadarView.Model.Render.Background.Airport
{
    /// <summary>
    /// Reprezentuje vzletovou dráhu (runway) jako objekt vykreslení.
    /// </summary>
    public class BackgroundRunway : RenderObject, IRenderable
    {
        /// <summary>
        /// Bod začátku vzletové dráhy ve viewportu.
        /// </summary>
        private Vector2 startPoint;

        /// <summary>
        /// Bod konce vzletové dráhy ve viewportu.
        /// </summary>
        private Vector2 endPoint;

        /// <summary>
        /// Zeměpisná poloha začátku vzletové dráhy
        /// </summary>
        private Location startLocation;

        /// <summary>
        /// Zeměpisná poloha konce vzletové dráhy
        /// </summary>
        private Location endLocation;

        /// <summary>
        /// Mapová projekce
        /// </summary>
        private IViewportProjection mapProjection;


        public BackgroundRunway(IViewportProjection mapProjection, Location startLocation, Location endLocation, Color color, float strokeThickness) : base(color, strokeThickness)
        {
            this.mapProjection = mapProjection;
            this.mapProjection.ViewportChanged += this.Projector_ViewportChanged;

            this.startLocation = startLocation;
            this.endLocation = endLocation;
            this.UpdatePosition();
        }

        private void Projector_ViewportChanged(object sender, EventArgs e)
        {
            this.UpdatePosition();
        }

        /// <summary>
        /// Přepočítá zeměpisné souřadnice na bod ve viewportu.
        /// </summary>
        private void UpdatePosition()
        {
            this.startPoint = this.mapProjection.LocationToViewportPoint(this.endLocation);
            this.endPoint = this.mapProjection.LocationToViewportPoint(this.startLocation);
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {
            drawBatch.DrawLine(this.Pen, this.startPoint, this.endPoint);
        }
    }
}
