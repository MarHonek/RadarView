using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.DataService.AviationData.AirspaceDataService;
using RadarView.Model.DataService.Network;
using RadarView.Model.Entities.AviationData.Airspace;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Service.Config;
using RadarView.Model.Service.SessionContext;

namespace RadarView.Model.Render.Background.Airspace
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
    public class BackgroundAirspaceManager : IBackgroundAirspaceManager
	{
		/// <summary>
		/// Vzdušné prostory
		/// </summary>
		public List<BackgroundAirspace> listOfAirspace;

		/// <summary>
		/// Lock pro synchronizaci vláken s AfisStrip.
		/// </summary>
		private static readonly object afisStripLock = new object();


		private readonly ISessionContext _sessionContext;

		private readonly IAirspaceDataService _airspaceDataService;

		private readonly IViewportProjection _mapProjection;

		private readonly IAfisStripDataService _afisStripDataService;


		public BackgroundAirspaceManager(ISessionContext sessionContext, 
										 IAirspaceDataService airspaceDataService,
										 IViewportProjection mapProjection,
										 IAfisStripDataService afisStripDataService)
		{
			this._sessionContext = sessionContext;
			this._airspaceDataService = airspaceDataService;
			this._mapProjection = mapProjection;
			this._afisStripDataService = afisStripDataService;
			this._afisStripDataService.ActiveAirspaceReceived += this._afisStripDataService_ActiveAirspaceReceived;
		}

		/// <summary>
		/// Zvýrazní vzdušné prostory označené jako aktivní (z afistripu).
		/// </summary>
		private void _afisStripDataService_ActiveAirspaceReceived(object sender, AfisStripDataService.ActiveAirspaceEventArgs e)
		{
			lock (afisStripLock) {

				var names = e.AirspaceNames;

				var regexes = names.Select(name =>
						new Regex(name + @"(\s|$)", RegexOptions.Compiled | RegexOptions.IgnoreCase))
					.ToList();

				foreach (var bgAirspace in this.listOfAirspace) {
					if (regexes.Any(rgx => rgx.IsMatch(bgAirspace.Airspace.Name))) {
						bgAirspace.Color = Colors.AirspaceActiveColor;
					}
					else {
						bgAirspace.Color = this.GetAirspaceColor(bgAirspace.Airspace);
					}
				}
			}
		}

		/// <summary>
		/// Vrací barvu vzdušného prostoru na základě jeho atributů
		/// </summary>
		/// <param name="airspace">vzdušný prostor</param>
		/// <returns>barvu vzdušného prostoru</returns>
		private Color GetAirspaceColor(Entities.AviationData.Airspace.Airspace airspace)
		{
			var airspaceCategoriesColors = Colors.AirspaceCategoriesColors;
			var airspaceCategory = airspace.Category;
			if (airspaceCategoriesColors.ContainsKey(airspaceCategory)) {
				return airspaceCategoriesColors[airspaceCategory];
			}

			return airspaceCategoriesColors[AirspaceCategory.Other];
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void AirspaceCategoryVisibility(AirspaceCategory category, bool visible)
		{
			foreach (var airspace in this.listOfAirspace) {
				if (airspace.Airspace.Category == category) {
					airspace.IsVisible = visible;
				}
			}
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Initialize()
	    {
			this.listOfAirspace = new List<BackgroundAirspace>();
			var airspaceInMonitoredArea = this._airspaceDataService.GetAllAirspaceInBoundingBox(this._sessionContext.MonitoredArea);
			foreach (var airspace in airspaceInMonitoredArea) {
				this.listOfAirspace.Add(new BackgroundAirspace(airspace, this._mapProjection, this.GetAirspaceColor(airspace)));
			}
	    }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
		{
			foreach (var backgroundAirspace in this.listOfAirspace) {
				backgroundAirspace.Draw(spriteBatch, drawBatch);
			}
		}
	}
}
