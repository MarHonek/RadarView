namespace RadarView.Model.Entities.Aviation
{
    /// <summary>
    /// Třída reprezentuje predikovaný stavový vektor získaný z prediktoru.
    /// </summary>
    public class AircraftPredictedStateVector : AircraftStateVector
    {
        /// <summary>
        /// Aktuální predikovaný fix.
        /// </summary>
        public AircraftFix CurrentPredictedFix { get; }

        /// <summary>
        /// Vytvoří novou instanci třídy AircraftPredictedStateVector.
        /// </summary>
        /// <param name="currentStateVector">Stavový vektor z datového zdroje.</param>
        /// <param name="currentPredictedFix">Aktuální predikovaný fix.</param>
        public AircraftPredictedStateVector(AircraftStateVector currentStateVector, AircraftFix currentPredictedFix) : base(currentStateVector.RealFix,  currentStateVector.GroundSpeed, currentStateVector.VerticalSpeed, currentStateVector.OnGround, currentStateVector.Track, currentStateVector.Maneuver, currentStateVector.DataSource)
        {
            this.CurrentPredictedFix = currentPredictedFix;
        }
    }
}
