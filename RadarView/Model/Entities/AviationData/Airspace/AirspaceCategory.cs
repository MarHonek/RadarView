using System.Xml.Serialization;

namespace RadarView.Model.Entities.AviationData.Airspace
{
    /// <summary>
    /// Vycet obsahuje typu vzdusnych prostoru.
    /// </summary>
    public enum AirspaceCategory
    {
        [XmlEnum("A")] A,
        [XmlEnum("B")] B,
        [XmlEnum("C")] C,
        [XmlEnum("CTR")] CTR,
        [XmlEnum("D")] D,
        [XmlEnum("DANGER")] Danger,
        [XmlEnum("E")] E,
        [XmlEnum("F")] F,
        [XmlEnum("G")] G,
        [XmlEnum("GLIDING")] Gliding,
        [XmlEnum("OTH")] OTH,
        [XmlEnum("RESTRICTED")] Restricted,
        [XmlEnum("TMA")] TMA,
        [XmlEnum("TMZ")] TMZ,
        [XmlEnum("WAVE")] Wave,
        [XmlEnum("PROHIBITED")] Prohibited,
        [XmlEnum("FIR")] FIR,
        [XmlEnum("UIR")] UIR,
        [XmlEnum("RMZ")] RMZ,
        [XmlEnum("OTHER")] Other
	}
}
