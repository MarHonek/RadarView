// Převzato z XAML Map Control - https://github.com/ClemensFischer/XAML-Map-Control
// Autor: Clemens Fischer
// Upravil: Martin Honěk

using System;
using Microsoft.Xna.Framework;
using RadarView.Model.Entities.Geographic;
using BoundingBox = RadarView.Model.Entities.Geographic.BoundingBox;

namespace RadarView.Model.Managers.MapProjection
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
    public class ViewportProjection : IViewportProjection
    {                
        /// <summary>
        /// Středoý bod viewportu
        /// </summary>
        private Vector2 viewportCenter;

        /// <summary>
        /// Mapová projekce
        /// </summary>
        private MapProjection projection;

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public Vector2 RenderSize { get; set; }

        /// <summary>
        /// Zeměpisná poloha středu viewportu
        /// </summary>
        private Location center = new Location(0, 0);

        /// <summary>
        /// Aktuální zoom level
        /// </summary>
        private float zoomLevel = 9;

        /// <summary>
        /// Dočasný středový bod pro transformaci
        /// </summary>                       
        private Location transformCenter;

        /// <summary>
		/// <inheritdoc/>
		/// </summary>
        public event EventHandler ViewportChanged;

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public Location Center
        {
	        get { return this.center; }
	        set
	        {
		        this.center = value;
		        this.UpdateTransform();
	        }
        }

        /// <summary>
        /// Minimální zoom level
        /// </summary>
        private float minZoomLevel = 0;

        /// <summary>
        /// Maximální zoom level
        /// </summary>
        private float maxZoomLevel = 15;

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public float ZoomLevel
        {
	        get { return this.zoomLevel; }
	        set
	        {
		        this.zoomLevel = value;
		        this.UpdateTransform(true);
	        }
        }

		public ViewportProjection()
		{
			this.projection = new WebMercatorProjection();
		}

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Initialize(Location center)
        {
	        this.ResetTransformCenter();
	        this.Center = center;
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public Vector2 LocationToViewportPoint(Location location)
        {
            return this.projection.LocationToViewportPoint(location);
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public Location ViewportPointToLocation(Vector2 point)
        {
            return this.projection.ViewportPointToLocation(point);
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public BoundingBox ViewportToBoundingBox()
        {
            var leftCorner = this.ViewportPointToLocation(Vector2.Zero);
            var rightCorner = this.ViewportPointToLocation(this.RenderSize);
            return new BoundingBox(rightCorner.Latitude, leftCorner.Longitude, leftCorner.Latitude, rightCorner.Longitude);
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public void SetTransformCenter(Vector2 center)
        {
            this.transformCenter = this.projection.ViewportPointToLocation(center);
            this.viewportCenter = center;
        }

        /// <summary>
        /// Resetuje dočasný středový bod nastavený metodou SetTransformCenter
        /// </summary>
        public void ResetTransformCenter()
        {
            this.transformCenter = null;
            this.viewportCenter = new Vector2(this.RenderSize.X / 2, this.RenderSize.Y / 2);
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public void TranslateMap(Vector2 translation)
        {
	        if (this.transformCenter != null) {
		        this.ResetTransformCenter();
		        this.UpdateTransform();
	        }

	        if (Math.Abs(translation.X) > 0.00001f || Math.Abs(translation.Y) > 0.00001f) {

		        translation.X = -translation.X;
		        translation.Y = -translation.Y;

		        //nastaví nový středový bod na základě vektoru posunu
		        this.Center = this.projection.TranslateLocation(this.Center, translation);
	        }
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public void ZoomMap(Vector2 center, float zoomLevel)
        {
	        zoomLevel = Math.Min(Math.Max(zoomLevel, this.minZoomLevel), this.maxZoomLevel);

	        if (Math.Abs(this.ZoomLevel - zoomLevel) > 0.00001f) {
		        this.SetTransformCenter(center);
		        this.ZoomLevel = zoomLevel;
	        }
        }

        /// <summary>
        /// Metoda provádí transformaci zobrazení (viewportu). Po provedení vyvolá událost ViewportChanged.
        /// </summary>
        /// <param name="resetTransformCenter">hodnota určující zda se má resetovat středový bod pro transformaci</param>
        private void UpdateTransform(bool resetTransformCenter = false)
        {
	        var projection = this.projection;
	        var center = this.transformCenter ?? this.center;

	        projection.SetViewportTransform(this.center, center, this.viewportCenter, this.ZoomLevel);

	        if (this.transformCenter != null) {
		        center = projection.ViewportPointToLocation(new Vector2(this.RenderSize.X / 2, this.RenderSize.Y / 2));

		        this.center = center;

		        if (resetTransformCenter) {
			        this.ResetTransformCenter();
			        projection.SetViewportTransform(center, center, this.viewportCenter, this.ZoomLevel);
		        }
	        }

	        this.ViewportChanged?.Invoke(this, new EventArgs());
        }
    }
}
