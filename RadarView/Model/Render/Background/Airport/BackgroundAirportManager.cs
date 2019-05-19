using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LilyPath;
using Microsoft.Xna.Framework.Graphics;
using MoreLinq;
using RadarView.Model.DataService.AviationData.AirportDataService;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Service.SessionContext;
using RadarView.Properties;
using SharpDX.MediaFoundation.DirectX;
using Color = Microsoft.Xna.Framework.Color;
using Colors = RadarView.Model.Service.Config.Colors;

namespace RadarView.Model.Render.Background.Airport
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
    public class BackgroundAirportManager : IBackgroundAirportManager
    {
	    /// <summary>
	    /// Kolekce letišť.
	    /// </summary>
	    private List<BackgroundAirport> airports;

	    /// <summary>
	    /// Šířka runway (pixely).
	    /// </summary>
	    private readonly float AirportRunwayThickness = Settings.Default.AirportRunwayThickness;


		private ISessionContext _sessionContext;

	    private IAirportDataService _airportDataService;

	    private IViewportProjection _mapProjection;


	    public BackgroundAirportManager(ISessionContext sessionContext, IAirportDataService airportDataService, IViewportProjection mapProjection)
	    {
		    this._sessionContext = sessionContext;
		    this._airportDataService = airportDataService;
		    this._mapProjection = mapProjection;
	    }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task Initialize()
	    {
			this.airports = new List<BackgroundAirport>();
			var airportsInMonitoredArea = await this._airportDataService.GetNotClosedAirportsInBoundingBoxByCountryAsync(this._sessionContext.MonitoredArea, this._sessionContext.CountryCode);
			foreach (var airport in airportsInMonitoredArea) {
				this.airports.Add(new BackgroundAirport(this._mapProjection, airport, Colors.AirportColor, this.AirportRunwayThickness));
			}
	    }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void SetVisibilityExceptSelectedAirport(bool shouldBeVisible)
		{
			this.airports.Where(x => !x.Airport.Equals(this._sessionContext.CurrentAirport))
						 .ForEach(x => x.ChangeVisibilityOfAirportAndRunways(shouldBeVisible));
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
		{
			foreach (var backgroundAirport in this.airports) {
				backgroundAirport.Draw(spriteBatch, drawBatch);
			}
		}
    }
}
