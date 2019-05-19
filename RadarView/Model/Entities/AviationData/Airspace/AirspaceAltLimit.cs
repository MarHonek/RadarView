using System.Xml.Serialization;

namespace RadarView.Model.Entities.AviationData.Airspace
{
    /// <summary>
    /// Trida obsahuje informace o vysce vzdusneho prostoru. 
    /// </summary>
    public class AirspaceAltLimit
    {
        /// <summary>
        /// Informace typ, odkud se vypocitava nadmorska vyska (referencni vyska).
        /// </summary>
        [XmlAttribute("REFERENCE")]
        public AirspaceAltLimitReference Reference { get; set; }

        /// <summary>
        /// Nadmorska vyska.
        /// </summary>
        [XmlElement("ALT")]
        public int Altitude { get; set; }

        /// <summary>
        /// Jednotka v jake je uvedena nadmorska vyska.
        /// </summary>
        [XmlAttribute("UNIT")]
        public AirspaceAltitudeUnit Unit { get; set; }

    }

    /// <summary>
    /// Vycet obsahuje jednotky v jakych muze byt nadmorska vyska uvedena.
    /// </summary>
    public enum AirspaceAltitudeUnit
    {
        /// <summary>
        /// Letova hladina.
        /// </summary>
        [XmlEnum("FL")] FlightLevel,

        /// <summary>
        /// Stopy.
        /// </summary>
        [XmlEnum("F")] Feet,
    }
}
