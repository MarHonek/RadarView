using System.Collections.Generic;
using RadarView.Model.Entities.AviationData;

namespace RadarView.Model.Entities.Aviation
{
    /// <summary>
    /// Reprezentuje letadlo
    /// </summary>
    public class Aircraft
    {
        /// <summary>
        /// Identifikátor letadla.
        /// </summary>
        public AircraftIdentifier Id { get; }

        /// <summary>
        /// Seznam historických fixu obsahující 3D polohy a čas.
        /// </summary>
        public List<AircraftFix> TrailDotsFixes { get; }

        /// <summary>
        /// Fix obsahující polohu a čas konce predikované polohy.
        /// </summary>
        public AircraftFix SpeedVectorEndFix { get; }

        /// <summary>
        /// Seznam surových polohových dat z datových zdrojů.
        /// </summary>
        public List<AircraftFix> RealFixes { get; }

        /// <summary>
        /// Informace o letadle.
        /// </summary>
        public AircraftInfo Info { get; }

        /// <summary>
        /// Aktuální stavový vektor obsahující informace o poloze, výšce, rychlosti atd.
        /// </summary>
        public AircraftPredictedStateVector CurrentStateVector { get; }

		/// <summary>
		/// Seznam datových zdrojů ze kterých přišla informace o letadle.
		/// </summary>
		public List<AircraftDataSourceEnum> ParticipatingDataSources { get; }


        /// <summary>
        /// Vytvoří novou instanci třídy Aircraft.
        /// </summary>
        /// <param name="id">Identifikátor letadla.</param>
        /// <param name="currentFix">Aktuální poloha letadla.</param>
        /// <param name="trailDotsFixes">Seznam historických poloh.</param>
        /// <param name="speedVectorEndFix">Predikovaná poloha.</param>
        /// <param name="KnownFixes">Seznam surových polohových dat získaných z datového zdroje.</param>
        /// <param name="info">Informace o letadlech.</param>
        public Aircraft(AircraftIdentifier id,  AircraftPredictedStateVector currentStateVector, List<AircraftFix> trailDotsFixes, AircraftFix speedVectorEndFix, List<AircraftFix> lastRealFix, AircraftInfo info, List<AircraftDataSourceEnum> participatingDataSources)
        {
            this.Id = id;
            this.CurrentStateVector = currentStateVector;
            this.TrailDotsFixes = trailDotsFixes;
            this.SpeedVectorEndFix = speedVectorEndFix;
            this.RealFixes = lastRealFix;
            this.Info = info;
			this.ParticipatingDataSources = participatingDataSources;
        }

        public override bool Equals(object obj)
        {
	        if (obj == null) {
		        return false;
	        }

	        if (!(obj is Aircraft)) {
		        return false;
	        }

	        var aircraft = obj as Aircraft;
	        return this.Id.Equals(aircraft.Id);
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
