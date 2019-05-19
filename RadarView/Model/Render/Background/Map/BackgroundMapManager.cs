using System.Collections.Generic;
using LilyPath;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.Entities.MapLayer;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Service.SessionContext;

namespace RadarView.Model.Render.Background.Map
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class BackgroundMapManager : IBackgroundMapManager
	{
		/// <summary>
		/// Kolekce mapových podkladů.
		/// </summary>
		private MapLayerCollection mapLayerCollection;

		/// <summary>
		/// Kolekce podkladových map.
		/// Klíč: Název mapy.
		/// Hodnota: Mapa (vykreslovací objekt).
		/// </summary>
		private Dictionary<string, BackgroundMap> _backgroundMaps;
		public Dictionary<string, BackgroundMap> BackgroundMaps
		{
			get { return this._backgroundMaps; }
		}


		private readonly ISessionContext _sessionContext;

		private readonly IViewportProjection _mapProjection;


		public BackgroundMapManager(ISessionContext sessionContext,
									IViewportProjection mapProjection)
		{
			this._sessionContext = sessionContext;
			this._mapProjection = mapProjection;

			this.mapLayerCollection = this._sessionContext.MapLayerCollection;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Initialize()
		{
			var mapColor = Service.Config.Colors.MapColor;
			this._backgroundMaps = new Dictionary<string, BackgroundMap>();
			foreach (var layer in this.mapLayerCollection.Layers) {
				if (!layer.Renderable) {
					continue;
				}

				this._backgroundMaps.Add(layer.Name, new BackgroundMap(this._mapProjection, mapColor, layer.BoundingBoxes, layer.Textures));
			}
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void ChangeVisibility(string mapName, bool isVisible)
		{
			if (this._backgroundMaps.ContainsKey(mapName)) {
				this._backgroundMaps[mapName].IsVisible = isVisible;
			}
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
		{
			foreach (var map in this.BackgroundMaps) {
				map.Value.Draw(spriteBatch, drawBatch);
			}
		}
	}
}
