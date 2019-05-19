using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Abstract;

namespace RadarView.Model.Render.Targets
{
    /// <summary>
    /// Reprezentuje objekt, který označuje poslední známou polohu získanou z datového zdroje
    /// </summary>
    public class TargetRealFix : GeographicRenderShape, IRenderable
    {

        /// <summary>
        /// Textura pro vykreslení poslední známé polohy.
        /// </summary>
        private Texture2D RealFixTexture;

        public TargetRealFix(IViewportProjection projection, Location location, Vector2 size, Color color) : base(projection, location, size, color)
        {
            this.RealFixTexture = Renderer.Cross;
        }

        public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {
            spriteBatch.Draw(this.RealFixTexture, new Rectangle(this.RenderPoint.ToPoint(), this.Size.ToPoint()), this.Color);
        }
    }
}
