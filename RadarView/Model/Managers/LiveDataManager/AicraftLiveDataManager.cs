using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RadarView.Model.Entities.Aviation;
using RadarView.Model.Entities.Aviation.Interface;
using RadarView.Model.Entities.AviationData;
using RadarView.Model.Render;
using RadarView.Model.Service.AircraftDataSourceService;
using RadarView.Model.Service.Predictor;
using RadarView.Model.Service.SessionContext;
using BoundingBox = RadarView.Model.Entities.Geographic.BoundingBox;

namespace RadarView.Model.Managers.LiveDataManager
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
    public class AircraftLiveDataManager : IAircraftLiveDataManager
    {
        /// <summary>
        /// Kolekce obsahující surová data z datových zdrojů.
        /// </summary>
        private AircraftRawDataCollection collection;

        /// <summary>
        /// Lock pro synchronizaci přístupu k atributům z více vláken.
        /// </summary>
        private static readonly object CollectionLock = new object();

        /// <summary>
        /// Určí zda je možné informace z tohoto zdroje zalogovat.
        /// </summary>
        public bool IsLoggable
        {
	        get { return true; }
        }

		/// <summary>
		/// Prediktor určující polohu letadla a další informace o letadle (např. ryclost klesání/stoupání).
		/// </summary>
		private Predictor predictor;


        private ISessionContext _sessionContext;

        private IAircraftDataSourceClient _dataSourceClient;


		public AircraftLiveDataManager(ISessionContext sessionContext, IAircraftDataSourceClient dataSourceClient)
		{
			this._dataSourceClient = dataSourceClient;
	        this._sessionContext = sessionContext;
            this.collection = new AircraftRawDataCollection();
            this.predictor = new Predictor(this._sessionContext.MonitoredArea);
            this._dataSourceClient.AircraftReceived += this.DataSourceClientAircraftReceived;
        }

		/// <summary>
		/// Handler shromažduje polohová data a vhodně je uloží do kolekce.
		/// </summary>
		private void DataSourceClientAircraftReceived(object sender, AircraftDataSourceEventArgs e)
        {
	        lock (CollectionLock) {
		        this.collection[e.RawData.Id] = e.RawData;
	        }
        }

        /// <summary>
        /// Získa predikovaná seznam letadel (polohových dat) pro zadané časy.
        /// </summary>
        /// <param name="currentFixDateTime">časová známka aktuální polohy letadla.</param>
        /// <param name="trailDotsDateTime">pole časových známek historických poloh letadel.</param>
        /// <returns>Seznam letadel k zobrazení.</returns>
        public Dictionary<AircraftIdentifier, Aircraft> GetSample(DateTime currentFixDateTime, DateTime[] trailDotsDateTime)
        {
			var predictedAircraftCollection = new Dictionary<AircraftIdentifier, Aircraft>();
			lock (CollectionLock) {
				this.collection.MergeDuplicates();
				for (var i = this.collection.Count - 1; i >= 0; i--) {

					var predictorResult = this.predictor.Predict(currentFixDateTime, trailDotsDateTime, this.collection[i], this._sessionContext.AirportArea);

					var resultEnum = predictorResult.Item1;
					var predictedAircraft = predictorResult.Item2;

					if (resultEnum == PredictorResultEnum.Remove) {
						//Letadlo nemá být již zobrazováno (je mimo oblast, odhlásilo se).
						this.collection.Remove(this.collection[i]);
						continue;
					}

					if (resultEnum == PredictorResultEnum.Hide) {
						//Letadlo má být skryto.
						continue;
					}

					predictedAircraftCollection.Add(predictedAircraft.Id, predictedAircraft);
				}
			}

			return predictedAircraftCollection;
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public void RemoveAircraftFromDataSource(AircraftDataSourceEnum dataSource)
        {
	        lock (CollectionLock) {
		        this.collection.RemoveAircraftFromSource(dataSource);
	        }
        }
    }
}
