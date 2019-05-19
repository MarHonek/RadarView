using System;
using RadarView.Model.Entities.AviationData;

namespace RadarView.Model.Entities.Aviation
{
    /// <summary>
    /// Třída reprezentuje stavový vektor letadla.
    /// Obsahuje informace, které se časem mění tj. poloha, rychlost, kurz.
    /// </summary>
    public class AircraftStateVector
    {
        /// <summary>
        /// Reálný fix získaný z datového zdroje.
        /// </summary>
        public AircraftFix RealFix { get; }

        /// <summary>
        /// Traťová rychlost letadla (metry).
        /// </summary>
        public float? GroundSpeed { get; set; }

        /// <summary>
        /// Vertikální rychlost - stoupání/klesání (metry za sekundu). 
        /// </summary>
        public float? VerticalSpeed { get; set; }

        /// <summary>
        /// Manévr letadla. Stoupání/klesání/horizont.
        /// </summary>
        public AircraftManeuver Maneuver { get; set; }

		/// <summary>
		/// Atribut určující zda je letadlo na zemi
		/// </summary>
		public bool? OnGround { get; set; }

        /// <summary>
        /// Směr letu (stupně).
        /// </summary>
        public int? Track { get; set; }

        /// <summary>
        /// Datový zdroj.
        /// </summary>
        public AircraftDataSourceEnum DataSource { get; set; }

        /// <summary>
        /// Vytvoří novou instanci třídy AircraftStateVector.
        /// </summary>
        public AircraftStateVector( AircraftFix realFix, AircraftDataSourceEnum dataSource)
        {
            this.RealFix = realFix;
            this.DataSource = dataSource;
        }

        /// <summary>
        /// Vytvoří novou instanci třídy AircraftStateVector.
        /// </summary>
        public AircraftStateVector(AircraftFix realFix, float? groundSpeed, float? verticalSpeed, bool? onGround, int? track, AircraftManeuver maneuver)
        {
            this.RealFix = realFix;
            this.GroundSpeed = groundSpeed;
            this.VerticalSpeed = verticalSpeed;
            this.OnGround = onGround;
            this.Track = track;
            this.Maneuver = maneuver;
        }

        /// <summary>
        /// Vytvoří novou instanci třídy AircraftStateVector.
        /// </summary>
        public AircraftStateVector(AircraftFix realFix, float? groundSpeed, float? verticalSpeed, bool? onGround, int? track, AircraftManeuver maneuver, AircraftDataSourceEnum dataSource) : this(realFix, groundSpeed, verticalSpeed, onGround, track, maneuver)
        {
			this.DataSource = dataSource;
        }

        /// <summary>
        /// Určí zda jsou všechny atributy stavového vektoru vyplněny.
        /// POZOR: OnGround nemusí být vyplněn neboť je tato informace problematická a bývá často null i když je letadlo na zemi.
        /// </summary>
        /// <returns>true pokud je vektor plně vyplněn (Mimo OnGround), jinak false.</returns>
        public bool IsFullyFilled()
        {
            return this.GroundSpeed.HasValue && this.VerticalSpeed.HasValue && this.Track.HasValue;
        }
    }
}
