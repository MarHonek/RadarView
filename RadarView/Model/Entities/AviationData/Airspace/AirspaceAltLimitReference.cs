using System.Xml.Serialization;

namespace RadarView.Model.Entities.AviationData.Airspace
{
    /// <summary>
    /// Trida obsahuje informaci odkud se vypocitava nadmorska vyska.
    /// </summary>
    public enum AirspaceAltLimitReference
    {
        /// <summary>
        /// Zeme.
        /// </summary>
        [XmlEnum("GND")] Ground,

        /// <summary>
        /// More.
        /// </summary>
        [XmlEnum("MSL")] MainSeaLevel,

        /// <summary>
        /// Atmosfera.
        /// </summary>
        [XmlEnum("STD")] StandartAtmosphere
    }
}
