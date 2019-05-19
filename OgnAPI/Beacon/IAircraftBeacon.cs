namespace OgnAPI.Beacon
{
	/// <summary>
	/// Třída reprezentuje základní informace o letadle.
	/// </summary>
    public interface IAircraftBeacon
    {
        /// <summary>
        /// Volací znak. Může obsahovat addresu zařízení.
        /// </summary>
        string CallSign { get; }

        /// <summary>
        /// Časová známka informace (Unix epocha).
        /// </summary>
        long Timestamp { get; set; }

        /// <summary>
        /// Zeměpisná šířka. (Stupně jako desetinné číslo).
        /// </summary>
        double Latitude { get; }

        /// <summary>
        /// Zěmepisná délka (Stupně jako desetinné číslo).
        /// </summary>
        double Longitude { get; }

        /// <summary>
        /// Nadmořská výška (stopy).
        /// </summary>
        float Altitude { get; }

        /// <summary>
        /// Kurz (Stupně) nebo null.
        /// </summary>
        int? Track { get; }

        /// <summary>
        /// Rychlost (Uzly) nebo null.
        /// </summary>
        float? GroundSpeed { get; }

        /// <summary>
        /// Textový řetězec nedekódovaného packetu = nezpracovaná data.
        /// </summary>
        string RawPacket { get; }

        /// <summary>
        /// Jméno přijímače.
        /// </summary>
        string ReceiverName { get; }

        /// <summary>
        /// Typ adresy.
        /// </summary>
        AddressType AddressType { get; }

        /// <summary>
        /// Adresa (ICAO/FLARM/OGN) tracker ID
        /// </summary>
        string Address { get; }

        /// <summary>
        /// Originální (FLARM) adresa.
        /// </summary>
        string OriginalAddress { get; }

        /// <summary>
        /// Typ letadla.
        /// </summary>
        AircraftType AircraftType { get; }

        /// <summary>
        /// Atribut určuje zda je stealth mode aktivován.
        /// </summary>
        bool Stealth { get; }

        /// <summary>
        /// Atribut určuje zda je no-tracking mode aktivován.
        /// </summary>
        bool NoTracking { get; }

        /// <summary>
        /// Rychlost klesání/stoupání. Uzly za minutu nebo null.
        /// </summary>
        float? ClimbRate { get; }

        /// <summary>
        /// Míra obratu (půlka otáčky za minutu) nebo null.
        /// </summary>
        float? TurnRate { get; }

        /// <summary>
        /// Sílá signálu (dB) nebo null.
        /// </summary>
        float? SignalStrength { get; }

        /// <summary>
        /// Frekvenční posun (kHz).
        /// </summary>
        float? FrequencyOffset { get; }

        /// <summary>
        /// GPS status (GPS přenost v metrech, vertikální a horizontální) nebo null.
        /// </summary>
        string GpsStatus { get; }

        /// <summary>
        /// Počet chyb opravených přijímačem nebo null.
        /// </summary>
        int? ErrorCount { get; }

        /// <summary>
        /// ID letadel přijmuto tímto letadlem.
        /// </summary>
        string[] HeardAircraftIds { get; }

        /// <summary>
        /// Verze firmware nebo null.
        /// </summary>
        float? FirmwareVersion { get; }

        /// <summary>
        /// Verze hardware nebo null.
        /// </summary>
        int? HardwareVersion { get; }

        /// <summary>
        /// Odhadovaný účinný vyzařovaný výkon vysílače nebo null.
        /// </summary>
        float? ERP { get; }

        /// <summary>
        /// Letová hladina (FL).
        /// Nadmořská výška z barometru (jestliže zařízení barometr obsahuje) nebo null.
        /// </summary>
        float? FlightLevel { get; }
    }
}
