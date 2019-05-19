using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.Entities.Aviation;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Abstract;

namespace RadarView.Model.Render.Targets
{
    /// <summary>
    /// Reprezentuje aktuální polohu letadla
    /// </summary>
    public class Target : GeographicRenderShape, IRenderable
    {
        /// <summary>
        /// Textura targetu
        /// </summary>
        private Texture2D targetTexture;


        public Target(IViewportProjection projection, Location location, Vector2 size, AircraftType aircraftType, Color color) : base(projection ,location, size ,color)
        {
            this.targetTexture = this.GetTargetTexture(aircraftType);
        }

        /// <summary>
        /// Vrací texturu podle typu letadla
        /// </summary>
        /// <param name="aircraftType">typ letadla</param>
        /// <returns>textura</returns>
        private Texture2D GetTargetTexture(AircraftType aircraftType)
        {
            switch(aircraftType)
            {
                case AircraftType.Glider:
                    return Renderer.Triangle;
                default:
                    return Renderer.Rectangle;
            }
        }
        
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {
            spriteBatch.Draw(this.targetTexture, new Rectangle(this.RenderPoint.ToPoint(), this.Size.ToPoint()), this.Color);        
        }
    }
}
