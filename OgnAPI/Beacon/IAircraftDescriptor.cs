namespace OgnAPI.Beacon
{
	/// <summary>
	/// Třída reprezentuje dodatečné informace o letadle.
	/// </summary>
    public interface IAircraftDescriptor
    {
        /// <summary>
        /// Registrační číslo.
        /// </summary>
        string RegNumber { get; }

        /// <summary>
        /// Soutěžní číslo nebo null.
        /// </summary>
        string CN { get; }

        /// <summary>
        /// Identifikátor modelu.
        /// </summary>
        string Model { get; }

        /// <summary>
        /// Atribut určující jestli je letadlo sledováno.
        /// </summary>
        bool Tracked { get; }

        /// <summary>
        /// Atribut určující zda je letadlo identifikováno.
        /// </summary>
        bool Identified { get; }
    }
}
