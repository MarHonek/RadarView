using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Air;

namespace RadarView.Render.Target
{
    /// <summary>
    /// Reprezentuje objekt, který označuje poslední známou polohu získanou z datového zdroje
    /// </summary>
    public class TargetKnowFix : GeographicRenderShape, IRenderable
    {

        /// <summary>
        /// Textura pro vykreslení poslední známé polohy.
        /// </summary>
        private Texture2D lastKnowFixTexture;

        public TargetKnowFix(ViewportProjection projection, Location location, Vector2 size, Color color) : base(projection, location, size, color)
        {
            this.lastKnowFixTexture = Renderer.Cross;
        }

        public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {
            spriteBatch.Draw(lastKnowFixTexture, new Rectangle(RenderPoint.ToPoint(), Size.ToPoint()), Color);
        }
    }
}
