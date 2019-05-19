using System;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.DataService.Weather.PrecipitationRadarDataService;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Abstract;
using BoundingBox = RadarView.Model.Entities.Geographic.BoundingBox;

namespace RadarView.Model.Render.Background.PrecipitationRadar
{
    /// <summary>
    /// Třídá se stará o vykreslení radarového snímku srážek.
    /// </summary>
    public class BackgroundPrecipitationRadar : RenderObject, IBackgroundPrecipitationRadar
    {
        /// <summary>
        /// Hranice radarového snímku
        /// </summary>
        private readonly BoundingBox RadarBoundingBox = new BoundingBox(Properties.Settings.Default.PrecipitationRadarArea);

        /// <summary>
        /// Zeměpisná poloha levého horního rohu radarového snímku
        /// </summary>
        private readonly Location TopLeftCornerGeoLocation;

        /// <summary>
        /// Zeměpisná poloha spodního pravého rohu radarového snímku.
        /// </summary>
        private readonly Location RightBottomCornerGeoLocation;

        /// <summary>
        /// Bod (ve viewportu) leveho horního rohu snímku.
        /// </summary>
        private Vector2 leftCornerPoint;

        /// <summary>
        /// Bod (ve viewportu) pravého spodního rohu snímku.
        /// </summary>
        private Vector2 rightCornerPoint;

        /// <summary>
        /// Mapová projekce
        /// </summary>
        private IViewportProjection _mapProjection;

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public Texture2D RadarTexture { get; set; }


		public BackgroundPrecipitationRadar(IViewportProjection mapProjection)
        {
            this._mapProjection = mapProjection;                
            this._mapProjection.ViewportChanged += this.Projector_ViewportChanged;

            this.Color = Color.White;
            this.IsVisible = true;

            this.TopLeftCornerGeoLocation = new Location(this.RadarBoundingBox.North, this.RadarBoundingBox.West);
            this.RightBottomCornerGeoLocation = new Location(this.RadarBoundingBox.South, this.RadarBoundingBox.East);

            this.UpdateRadarImagePosition();
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public float ImageOpacity
		{
			get { return this.Opacity; }
			set { this.Opacity = value; }
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public bool ImageVisibility
		{
			get { return this.IsVisible; }
			set { this.IsVisible = value; }
		}	

		/// <summary>
		/// Přepočítá novou pozici levého horního rohu a pravého spodního rohu na základě změny viewportu
		/// </summary>
		private void UpdateRadarImagePosition()
        {
            this.leftCornerPoint = this._mapProjection.LocationToViewportPoint(this.TopLeftCornerGeoLocation);
            this.rightCornerPoint = this._mapProjection.LocationToViewportPoint(this.RightBottomCornerGeoLocation);
        }

        private void Projector_ViewportChanged(object sender, EventArgs e)
        {
            this.UpdateRadarImagePosition();
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {
            if (this.RadarTexture != null)
            {
                spriteBatch.Draw(this.RadarTexture, new Rectangle((int)this.leftCornerPoint.X, (int)this.leftCornerPoint.Y,
	                (int)(this.rightCornerPoint.X - this.leftCornerPoint.X),
	                (int)(this.rightCornerPoint.Y - this.leftCornerPoint.Y)), this.Color);
            }
        }
    }
}
