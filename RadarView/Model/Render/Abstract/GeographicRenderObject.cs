using System;
using Microsoft.Xna.Framework;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Managers.MapProjection;

namespace RadarView.Model.Render.Abstract
{
    /// <summary>
    /// Reprezentuje objekt vykreslení se souřadnicemi určenými zeměpisnou polohou.
    /// </summary>
    /// <remarks>
    /// Optimalizace výkonu!
    /// Třídá je navrhnuta tak aby probíhalo co nejméně výpočtů při vykreslování. tzn. Get metoda pouze vrací hodnoty.
    /// </remarks>
    public abstract class GeographicRenderObject : RenderObject
    {
        /// <summary>
        /// Atribut určující, zda došlo k uvolnění prostředků.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Mapová projekce
        /// </summary>
        protected IViewportProjection projection;

        /// <summary>
        /// Zeměpisná poloha objektu. Při auktualizaci polohy je automaticky přepočítána Vlastnost Point.
        /// </summary>
        private Location location;
        public Location Location
        {
            get
            {
                return this.location;
            }
            set
            {
                this.location = value;
                if (this.IsVisible) {
					//Optimalizace výkonu. Pokud je object schovaný nepřepočítává jeho polohu.
	                this.Point = this.projection.LocationToViewportPoint(this.location);
	                this.LocationChanged?.Invoke(this, new EventArgs());
               }
            }
        }

        /// <summary>
        /// Bod (pixel) obrazu odpovídající zeměpisné poloze vlastnosti Location.
        /// </summary>
        public Vector2 Point { get; set;}

		/// <summary>
		/// Událost vyvolána pokud byla změněna pozice objektu z venčí (ne uživatelem).
		/// </summary>
        public event EventHandler LocationChanged;

        /// <summary>
        /// Inicializuje instanci třídy GeographicRenderObject
        /// </summary>
        /// <param name="projection">mapová projekce</param>
        /// <param name="location">geografická poloha středu</param>
        /// <param name="color">barva</param>
        public GeographicRenderObject(IViewportProjection projection, Location location, Color color) : base(color)
        {
            this.projection = projection;
            this.projection.ViewportChanged += this.Projection_ViewportChanged;
            this.Location = location;
        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.projection.ViewportChanged -= this.Projection_ViewportChanged;
                this.projection = null;
                this.location = null;
            }

            this.disposed = true;

            base.Dispose(disposing);
        }

        private void Projection_ViewportChanged(object sender, EventArgs e)
        {
			//Přepočítá pozici bodu při změně viewportu.
			this.Point = this.projection.LocationToViewportPoint(this.location);
        }
    }
}
