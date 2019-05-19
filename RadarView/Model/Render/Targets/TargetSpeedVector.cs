using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Abstract;

namespace RadarView.Model.Render.Targets
{
    /// <summary>
    /// Reprezentuje směr letu a predikovanou polohu letadla v následujích x sekundách
    /// </summary>
    public class TargetSpeedVector : GeographicRenderObject, IRenderable
    {
        /// <summary>
        /// Objekt reprezentující aktuální polohu
        /// </summary>
        private Model.Render.Targets.Target target;

        //Location definuje polohu Targetu za x sekund
        public TargetSpeedVector(IViewportProjection projection, Model.Render.Targets.Target target, Location location, Color color) : base(projection, location, color)
        {
            this.target = target;
        }

        public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {
            drawBatch.DrawPrimitiveLine(this.Pen, this.target.Point, this.Point);
        }
    }
}
