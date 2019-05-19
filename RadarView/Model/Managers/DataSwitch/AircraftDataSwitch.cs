using System;
using System.Collections.Generic;
using RadarView.Model.Entities.Aviation;
using RadarView.Model.Entities.Aviation.Interface;
using RadarView.Model.Managers.LiveDataManager;
using RadarView.Model.Service.SessionContext;
using RadarView.Properties;

namespace RadarView.Model.Managers.DataSwitch
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
    public class AircraftDataSwitch : IAircraftDataSwitch
    {
        /// <summary>
        /// Aktivni zdroj dat.
        /// </summary>
        private IRenderingSample activeDataSource;


        private IAircraftLiveDataManager _aircraftLiveDataManager;

        private ISessionContext _sessionContext;


        public AircraftDataSwitch(ISessionContext sessionContext, IAircraftLiveDataManager aircraftLiveDataManager)
        {
	        this._sessionContext = sessionContext;
	        this._aircraftLiveDataManager = aircraftLiveDataManager;
	        this.activeDataSource = this._aircraftLiveDataManager;
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public bool IsLoggable
        {
	        get { return false; }
        }

        /// <summary>
		/// <inheritdoc/>
		/// </summary>
        public Dictionary<AircraftIdentifier, Aircraft> GetSample(DateTime currentFixDateTime, DateTime[] trailDotsDateTime)
        {
            return this.activeDataSource.GetSample(currentFixDateTime, trailDotsDateTime);
        }
    }
}