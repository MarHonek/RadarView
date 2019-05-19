using LilyPath;
using Microsoft.Xna.Framework.Graphics;

namespace RadarView.Model.Render.Abstract
{
    /// <summary>
    /// Rozhraní pro vykreslování objektů.
    /// </summary>
    public interface IRenderable
    {
          /// <summary>
          /// Metoda pro vykreslení
          /// </summary>
          /// <param name="spriteBatch">instance třídy SpriteBatch pro vykreslování textur</param>
          /// <param name="drawBatch">instance třídy SpriteBatch pro vykrelování 2D primitiv</param>
          void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch);
    }
}
