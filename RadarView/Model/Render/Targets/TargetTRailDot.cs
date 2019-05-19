using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Abstract;

namespace RadarView.Model.Render.Targets
{
    /// <summary>
    /// Reprezentuje jednu z přechozích poloh letadla
    /// </summary>
    public class TargetTRailDot : GeographicRenderShape, IRenderable
    {
        /// <summary>
        /// Textura trail dotu
        /// </summary>
        private Texture2D texture;

        public TargetTRailDot(IViewportProjection projection, Location location, Vector2 size, Color color) 
            : base(projection, location, size, color)
        {
            this.texture = Renderer.FilledRectangle;
        }

        public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {     
            spriteBatch.Draw(this.texture, new Rectangle(this.RenderPoint.ToPoint(), this.Size.ToPoint()), this.Color);
        }
    }
}
