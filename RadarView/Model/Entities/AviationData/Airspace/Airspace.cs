using System.Xml.Serialization;

namespace RadarView.Model.Entities.AviationData.Airspace
{
    /// <summary>
    /// Trida reprezentuje vzdusny prostor.
    /// </summary>
    public class Airspace
    {
        /// <summary>
        /// Kategorie vzdusneho prostoru.
        /// </summary>
        [XmlAttribute("CATEGORY")]
        public AirspaceCategory Category { get; set; }

        /// <summary>
        /// Verze.
        /// </summary>
        [XmlElement("VERSION")]
        public string Version { get; set; }

        /// <summary>
        /// Identifikator vzdusneho prostoru.
        /// </summary>
        [XmlElement("ID")]
        public long Id { get; set; }

        /// <summary>
        /// Zeme ve ktere se vzdusny prostor nachazi.
        /// </summary>
        [XmlElement("COUNTRY")]
        public string Country { get; set; }

        /// <summary>
        /// Nazev.
        /// </summary>
        [XmlElement("NAME")]
        public string Name { get; set; }

        /// <summary>
        /// Maximalni nadmorska vyska (metry).
        /// </summary>
        [XmlElement("ALTLIMIT_TOP")]
        public AirspaceAltLimit AltLimitTop { get; set; }

        /// <summary>
        /// Minimalni nadmorska vyska (metry).
        /// </summary>
        [XmlElement("ALTLIMIT_BOTTOM")]
        public AirspaceAltLimit AltLimitBottom { get; set; }

        /// <summary>
        /// Informace o zemepisne poloze vzdusneho prostoru.
        /// </summary>
        [XmlElement("GEOMETRY")]
        public AirspaceGeometry Geometry { get; set; }


    }
}
