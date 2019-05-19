using System;
using System.Collections.Generic;
using RadarView.Model.Entities.AviationData;
using RadarView.Model.Entities.AviationData.Airports;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Entities.MapLayer;
using RadarView.Properties;

namespace RadarView.Model.Service.SessionContext
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class SessionContext : ISessionContext
	{

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public Airport CurrentAirport { get; set; }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public Location DefaultCenter
		{
			get { return this.CurrentAirport.Location; }
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public MapLayerCollection MapLayerCollection { get; set; }

		/// <summary>
		/// <inheritdoc />
		/// </summary>
		public BoundingBox MonitoredArea { get; set; }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public AircraftDataSourceWrapper AircraftDataSourceWrapper { get; set; }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public Tuple<int, BoundingBox> AirportArea { get; set; }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public Dictionary<AircraftDataSourceEnum, string> DataSourceReplayFiles { get; set; }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public string CountryCode { get; set; } = "CZ";

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void SaveToConfig()
		{
			Settings.Default.AirportSelected = this.CurrentAirport.IcaoIdent;
			Settings.Default.MonitoredArea = this.MonitoredArea.ToString();

			Settings.Default.Save();
		}
	}
}
