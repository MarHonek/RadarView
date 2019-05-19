using System;

namespace OsnAPI.Aircraft
{
    /// <summary>
    /// Třída reprezentuje informace o letadle získané ze služby OpenSkyNetwork.
    /// V budoucnu může přibýt databáze OSN.
    /// </summary>
    public class OsnAircraft : ICloneable
    {
        /// <summary>
        /// Aktuální informace o poloze, výšce, rychlost atd.
        /// </summary>
        public OsnAircraftStateVector StateVector { get; }

        /// <summary>
        /// Inicializuje instanci třídy OsnAircraft.
        /// </summary>
        /// <param name="stateVector">Aktuální informace o letadle.</param>
        public OsnAircraft(OsnAircraftStateVector stateVector)
        {
            this.StateVector = stateVector;
        }

        public override bool Equals(object obj)
        {

	        if (obj == null) {
		        return false;
	        }

	        if (!(obj is OsnAircraft)) {
		        return false;
	        }

	        var aircraft = obj as OsnAircraft;
	        return this.StateVector.Equals(aircraft.StateVector);
        }

        public override int GetHashCode()
        {
            return this.StateVector.GetHashCode();
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public override string ToString()
        {
            return nameof(this.StateVector) + ": " + this.StateVector.ToString();
        }
    }
}
