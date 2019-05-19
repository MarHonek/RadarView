using System.Collections.Generic;
using System.Threading.Tasks;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.Entities.AviationData.Airspace;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Background.Airport;
using RadarView.Model.Render.Background.Airspace;
using RadarView.Model.Render.Background.Map;
using RadarView.Model.Render.Background.MonitoredArea;
using RadarView.Model.Render.Background.PrecipitationRadar;
using RadarView.Model.Service.Config;
using RadarView.Model.Service.SessionContext;
using RadarView.Properties;

namespace RadarView.Model.Render.Background.BackgroundManager
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
    public class BackgroundManager : IBackgroundManager
    {
		private readonly IBackgroundMapManager _mapManager; 

		private readonly IBackgroundPrecipitationRadar _precipitationRadar;

		private readonly IBackgroundAirportManager _airportManager;

		private readonly IBackgroundAirspaceManager _airspaceManager;

		/// <summary>
		/// Vykreslený čtverec zobrazující sledovanou oblast.
		/// </summary>
        private BackgroundArea monitoredAreaRectangle;

   
        public BackgroundManager(IViewportProjection mapProjection,
								 ISessionContext sessionContext,
								 IBackgroundAirportManager airportManager,
								 IBackgroundAirspaceManager airspaceManager,
								 IBackgroundPrecipitationRadar precipitationRadar,
								 IBackgroundMapManager mapManager)
        {

	        this._airportManager = airportManager;
	        this._airspaceManager = airspaceManager;
	        this._mapManager = mapManager;
			this._precipitationRadar = precipitationRadar;
			this.monitoredAreaRectangle = new BackgroundArea(mapProjection, sessionContext.MonitoredArea, Colors.MonitoredAreaRectColor);
		}

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public async Task Initialize()
        {
	        this._mapManager.Initialize();
	        this._airspaceManager.Initialize();
	        await this._airportManager.Initialize();
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {
			this._mapManager.Draw(spriteBatch, drawBatch);
			this._airspaceManager.Draw(spriteBatch, drawBatch);
			this._airportManager.Draw(spriteBatch, drawBatch);
			this.monitoredAreaRectangle.Draw(spriteBatch, drawBatch);
			this._precipitationRadar.Draw(spriteBatch, drawBatch);
        }
    }
}
