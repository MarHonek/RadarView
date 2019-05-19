using System;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Abstract;
using BoundingBox = RadarView.Model.Entities.Geographic.BoundingBox;

namespace RadarView.Model.Render.Background.MonitoredArea
{
	/// <summary>
	/// Třída starající se o vykreslení čtverce reprezentující sledovanou oblast.
	/// </summary>
    public class BackgroundArea : RenderObject, IRenderable
    {
		/// <summary>
		/// Příznak určující zda byly uvolněny prostředky.
		/// </summary>
        private bool disposed;

		/// <summary>
		/// Mapová projekce.
		/// </summary>
        protected IViewportProjection mapProjection;

		/// <summary>
		/// Sledovaná oblast.
		/// </summary>
        private BoundingBox monitoredAreaBoundingBox;

		/// <summary>
		/// Pozice levého horního rohu.
		/// </summary>
		private Vector2 topLeftCorner;

		/// <summary>
		/// Pozice pravého spodního rohu.
		/// </summary>
        private Vector2 rightBottomCorner;

		/// <summary>
		/// Velikost obedlníku.
		/// </summary>
		private Vector2 size;


		public BackgroundArea(IViewportProjection mapProjection, BoundingBox monitoredArea, Color color) : base(color)
        {
            this.mapProjection = mapProjection;
            this.mapProjection.ViewportChanged += this.MapProjectionViewportChanged;
            this.monitoredAreaBoundingBox = monitoredArea;
            this.MapProjectionViewportChanged(this, null);
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        private void MapProjectionViewportChanged(object sender, EventArgs e)
        {
            this.topLeftCorner = this.mapProjection.LocationToViewportPoint(new Location(this.monitoredAreaBoundingBox.North, this.monitoredAreaBoundingBox.West));
            this.rightBottomCorner = this.mapProjection.LocationToViewportPoint(new Location(this.monitoredAreaBoundingBox.South, this.monitoredAreaBoundingBox.East));
			this.size = this.rightBottomCorner - this.topLeftCorner;
		}

		/// <summary>
		/// Uvolní prostředky.
		/// </summary>
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.mapProjection.ViewportChanged -= this.MapProjectionViewportChanged;
            }

            this.disposed = true;

            base.Dispose(disposing);
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
		{
			drawBatch.DrawRectangle(this.Pen, new Rectangle((int)this.topLeftCorner.X,
				(int)this.topLeftCorner.Y, (int)this.size.X, (int)this.size.Y));
		}
    }
}
