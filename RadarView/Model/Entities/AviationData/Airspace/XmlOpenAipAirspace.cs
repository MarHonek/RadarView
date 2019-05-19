using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace RadarView.Model.Entities.AviationData.Airspace
{
    /// <summary>
    /// Trida obsahuje informace o dokumentu se vzdusnymi prostory.
    /// </summary>
    [Serializable, XmlRoot("OPENAIP")]
    public class XmlOpenAipAirspace
    {
        /// <summary>
        /// Verze dokumentu.
        /// </summary>
        [XmlAttribute("VERSION")]
        public string Version { get; set; }

        /// <summary>
        /// Datovy format.
        /// </summary>
        [XmlAttribute("DATAFORMAT")]
        public string DataFormat { get; set; }

        /// <summary>
        /// Seznam vzdusnych prostoru.
        /// </summary>
        [XmlArray("AIRSPACES")]
        [XmlArrayItem("ASP")]
        public List<Airspace> AllAirspace { get; set; }
    }
}
