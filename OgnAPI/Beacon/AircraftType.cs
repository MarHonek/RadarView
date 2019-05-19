using System;

namespace OgnAPI.Beacon
{
    /// <summary>
    /// Třída reprezentuje typ letadla.
    /// </summary>
    public enum AircraftType
    {
        /// <summary>
        /// Neznámý.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Větroň.
        /// </summary>
        Glider = 1,

        /// <summary>
        /// Vlečné letadlo.
        /// </summary>
        TowPlane = 2,

        /// <summary>
        /// Helikoptéra.
        /// </summary>
        HelicopterRotorcraft = 3,

        /// <summary>
        /// Padák.
        /// </summary>
        Parachute = 4,

        /// <summary>
        /// Výsadkové letadlo.
        /// </summary>
        DropPlane = 5,

        /// <summary>
        /// Hang glider (závěsné létání).
        /// </summary>
        HangGlider = 6,

        /// <summary>
        /// Paraglider
        /// </summary>
        ParaGlider = 7,

        /// <summary>
        /// Motorové letadlo.
        /// </summary>
        PoweredAircraft = 8,

        /// <summary>
        /// Stíhačka.
        /// </summary>
        JetAircraft = 9,

        /// <summary>
        /// UFO.
        /// </summary>
        UFO = 10,

        /// <summary>
        /// Horkovzdušný balón.
        /// </summary>
        Balloon = 11,

        /// <summary>
        /// Vzducholoď.
        /// </summary>
        Airship = 12,

        /// <summary>
        /// Bezpilotní letadlo.
        /// </summary>
        UAV = 13,

        /// <summary>
        /// Statický objekt.
        /// </summary>
        StaticObject = 15

    }

    /// <summary>
    /// Rozšiřující třída pro typ letadla.
    /// </summary>
    public static class AircraftTypeExtension
    {
        /// <summary>
        /// Získá index na základě typu.
        /// </summary>
        /// <param name="value">Typ letadla.</param>
        /// <returns>Index typu letadla.</returns>
        public static int GetCode(this AircraftType value)
        {
            return (int)value;
        }

        /// <summary>
        /// Získá typ letadla podle indexu.
        /// </summary>
        /// <param name="value">index typu letadla.</param>
        /// <returns>Typ letadla.</returns>
        public static AircraftType ForValue(int value)
        {
            return Enum.IsDefined(typeof(AircraftType), value) ? (AircraftType)value : AircraftType.Unknown;
        }
    }
}
