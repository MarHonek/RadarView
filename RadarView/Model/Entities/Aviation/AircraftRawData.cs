using System;
using System.Collections.Generic;
using System.Linq;
using RadarView.Model.Entities.AviationData;
using RadarView.Model.Entities.Geographic;
using RadarView.Properties;

namespace RadarView.Model.Entities.Aviation
{
    /// <summary>
    /// Reprezentuje surová data o letadle, získaná z datového zdroje
    /// </summary>
    public class AircraftRawData
    {
        /// <summary>
        /// Identifikace letadla.
        /// </summary>
        public AircraftIdentifier Id { get; }

        /// <summary>
        /// Dodatečné informace o letadle.
        /// </summary>
        public AircraftInfo Info { get; }

		/// <summary>
		/// Stavové vektory letadla. Hodnota je seznam stavových vektorů z důvodu, 
		/// že mohou z různých datových zdrojů přijit vektory pro stejný čas a my chceme udržovat všechny stavové vektory.
		/// </summary>
		private SortedList<DateTime, List<AircraftStateVector>> stateVectors;

		/// <summary>
		/// Seznam všech datových zdroju, ze kterých byly získané informace o letadle.
		/// </summary>
		private List<AircraftDataSourceEnum> participatingDataSources;

		/// <summary>
		/// Maximální stáří nejstaršího reálného fixu získaného z datového zdroje.
		/// </summary>
		private readonly int MaxRealFixTimestampAge = Settings.Default.MaxRealFixAgeSeconds;

		/// <summary>
		/// Nejmladší fix, poloha získaná z datového zdroje.
		/// </summary>
		public DateTime YoungestRealFixDateTime
        {
            get { return this.stateVectors.LastOrDefault().Key; }
        }

        /// <summary>
        /// Nejstarší fix, poloha získaná z datového zdroje.
        /// </summary>
        public DateTime OldestRealFixDateTime 
        {
            get { return this.stateVectors.FirstOrDefault().Key; }
        }

        public AircraftRawData(string address, string callsign, string competetionName, string squawk, string registration,
	        string iataFlightNumber, float? latitude, float? longitude, DateTime? dateTime, float? altitude, int? track,
	        float? groundSpeed, float? climbRate, bool? onGround, AircraftType aircraftType, string model, string crew, string _operator, AircraftManeuver maneuver, AircraftDataSourceEnum dataSource, string originCountry,
	        string destinationCountry, string originCity, string destinationCity)
        {
	        this.Id = new AircraftIdentifier(registration, address, callsign, iataFlightNumber, squawk, this, dataSource);

	        this.Info = new AircraftInfo(competetionName, aircraftType, model, crew, _operator, originCountry, destinationCountry, originCity, destinationCity);
	        this.participatingDataSources = new List<AircraftDataSourceEnum>() {dataSource};

	        this.stateVectors = new SortedList<DateTime, List<AircraftStateVector>>();
	        if (latitude.HasValue && longitude.HasValue && dateTime.HasValue) {

		        var firstFix = new AircraftFix(new Location(latitude.Value, longitude.Value), altitude ?? 0, dateTime.Value);
		        var stateVector = new AircraftStateVector(firstFix, groundSpeed, climbRate, onGround, track, maneuver, dataSource);
		        this.AddStateVector(stateVector);
	        }
        }

        /// <summary>
        /// Přidá data ke kolekci informací o letadle.
        /// </summary>
        /// <param name="data">surová data o letadle.</param>
        public void AddData(AircraftRawData data)
        {
	        var newStateVector = data.stateVectors.FirstOrDefault().Value.FirstOrDefault();
	        if (newStateVector != null) {
		        this.Info.Combine(data.Info);
		        this.Id.Combine(data.Id, newStateVector.DataSource);
		        this.AddStateVector(newStateVector);
	        }
        }

        /// <summary>
        /// Získá stavový vektor nejblížšího reálného fixu. Pokud nejsou hodnoty k dispozici převezme hodnoty z předchozích vektorů.
        /// </summary>
        /// <param name="dateTime">Čas hledaného vektoru</param>
        public AircraftStateVector GetStateVectorOrPrevious(DateTime dateTime)
        {
	        var stateVectors = this.stateVectors.ToList();
	        AircraftStateVector resultStateVector = null;
	        for (var i = stateVectors.Count - 1; i >= 0; i--) {
		        var stateVectorsListItem = stateVectors[i].Value.FirstOrDefault();
		        if (stateVectorsListItem.RealFix.DateTime <= dateTime) {
			        if (resultStateVector == null) {
				        resultStateVector = new AircraftStateVector(stateVectorsListItem.RealFix, stateVectorsListItem.DataSource);
			        } else {
				        this.CopyStateVector(resultStateVector, stateVectorsListItem);
			        }

			        if (resultStateVector.IsFullyFilled()) {
				        break;
			        }
		        }
	        }

	        return resultStateVector;
        }

        /// <summary>
		/// Zkombinuje data z duplikátů.
		/// Identifikátory nejsou zkombinovány z důvodu, že jsou chybné, jejimž následkem bylo vytvoření duplikátů.
		/// </summary>
		/// <param name="data">data z duplikátu.</param>
		public void MergeDuplicates(AircraftRawData data) 
		{
			this.Info.Combine(data.Info);

			foreach (var stateVector in data.ListAllStateVectors()) {
				this.AddStateVector(stateVector);
			}
		}


        /// <summary>
        /// Přidá polohu z datového zdroje ke kolekci informací o letadle.
        /// <param name="stateVector">Stavový vektor, který bude přídán ke kolekci informací o letadle.</param>
        /// </summary>
        private void AddStateVector(AircraftStateVector stateVector)
		{
			//Zjistí jestli už existuje stavový vektor s touto časovou známkou
			//Pokud ano => přidá stavový vektor do kolekce.
			//Pokud ne => vytvoří nový seznam pro danou časovou známku a přidá do ní stavový vektor.
			List<AircraftStateVector> stateVectors = null;
			this.stateVectors.TryGetValue(stateVector.RealFix.DateTime, out stateVectors);

			if (stateVectors == null) {
				stateVectors = new List<AircraftStateVector>();
				this.stateVectors.Add(stateVector.RealFix.DateTime, stateVectors);
			}

			//Podmínka zabraňuje ukládání duplikátních informací z jednoho datového zdroje.
			if (!stateVectors.Exists(x => x.DataSource == stateVector.DataSource)) {
				stateVectors.Add(stateVector);
			}

			//Pokud přijde vektor z nového zdroje, přidá zdroj do kolekce.
			if (!this.participatingDataSources.Contains(stateVector.DataSource)) {
				this.participatingDataSources.Add(stateVector.DataSource);
			}

			while ((this.stateVectors.Last().Key - DateTime.UtcNow).TotalSeconds > this.MaxRealFixTimestampAge) {
				//smaže stavové vektory, jejichž stáří překračuje konstantu.
				this.stateVectors.RemoveAt(0);
			}
		}

		/// <summary>
		/// Odstraní datový zdroj z kolekce.
		/// </summary>
		/// <param name="dataSourceToRemove"></param>
		public void RemoveDataSource(AircraftDataSourceEnum dataSourceToRemove) 
		{
			this.participatingDataSources.RemoveAll(x => x == dataSourceToRemove);
		}

		private void CopyStateVector(AircraftStateVector target, AircraftStateVector source)
		{
			var t = typeof(AircraftStateVector);
			var properties = t.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);
			foreach (var prop in properties) {
				var value = prop.GetValue(source);
				if (value != null) {
					if (prop.GetValue(target) == null) {
						prop.SetValue(target, value);
					}
				}
			}
		}

		/// <summary>
		/// Vrátí seznam všech datových zdroju, ze kterých byly získané informace o letadle.
		/// </summary>
		/// <returns>seznam zainteresovaných datových zdrojů.</returns>
		public List<AircraftDataSourceEnum> ListParticipatingDataSources() 
		{
			return this.participatingDataSources.ToList();
		}

		/// <summary>
		/// Vylistuje všechny timestampy
		/// </summary>
		/// <returns></returns>
		public List<AircraftStateVector> ListAllStateVectors() 
		{
			return this.stateVectors
					.SelectMany(x => x.Value)
					.ToList();
		}

		/// <summary>
		/// Určí zda se dva objekty typu AircraftRawData rovnají.
		/// Porovnání probíhá na základě jejich identifikátorů.
		/// </summary>
		/// <param name="obj">objekt.</param>
		/// <returns>true pokud se rovnají, jinak false.</returns>
        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }

            var rawData = obj as AircraftRawData;
            return rawData != null && rawData.Id.Equals(this.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }		
	}
}
