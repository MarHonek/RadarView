using System;
using Microsoft.Xna.Framework;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Managers.MapProjection;

namespace RadarView.Model.Render.Abstract
{
    /// <summary>
    /// Reprezentuje tvar ohraničený obdelníkem. Tvar je vykreslen podle pozice levého horního rohu obdelníku.
    /// </summary>
    /// <remarks>
    /// Optimalizace výkonu!
    /// Třídá je navrhnuta tak aby probíhalo co nejméně výpočtů při vykreslování. tzn. Get metoda pouze vrací hodnoty.
    /// </remarks>
    public class GeographicRenderShape : GeographicRenderObject
    {
        /// <summary>
        /// Atribut určující zda došlo k uvolnění prostředků.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Rozměr obdélníku ohraničující tvar.
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// Bod (pixel) levého horního horního rohu obdelníku.
        /// </summary>
        private Vector2 renderPoint;
        public Vector2 RenderPoint
        {
            get
            {
                return this.renderPoint;
            }
        }

        /// <summary>
        /// Inicializuje instanci třídy GeographicRenderShape
        /// </summary>
        /// <param name="projection">mapová projekce</param>
        /// <param name="location">geografická poloha středu</param>
        /// <param name="size">rozměry tvaru</param>
        /// <param name="color">barva</param>
        public GeographicRenderShape(IViewportProjection projection, Location location, Vector2 size, Color color) : base(projection, location, color)
        {
            this.Size = size;   
            this.LocationChanged += this.GeographicRenderShape_LocationChanged;
            projection.ViewportChanged += this.GeographicRenderShape_LocationChanged;
            this.GeographicRenderShape_LocationChanged(this, null);

        }

        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.LocationChanged -= this.GeographicRenderShape_LocationChanged;
                this.projection.ViewportChanged -= this.GeographicRenderShape_LocationChanged;
            }

            this.disposed = true;

            base.Dispose(disposing);
        }

        private void GeographicRenderShape_LocationChanged(object sender, EventArgs e)
        {
            this.renderPoint = this.Point - (this.Size / 2);
        }
    }
}
