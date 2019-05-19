using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using RadarView.Model.Entities.Geographic;

namespace RadarView.Model.Entities.AviationData.Airspace
{
    /// <summary>
    /// Trida obsahuje informace o zemepisne poloze vzdusneho prostoru.
    /// </summary>
    public class AirspaceGeometry
    {
        /// <summary>
        /// Atribut obsahujici souradnice polygonu - vzdusneho prostoru v textove podobe.
        /// </summary>
        private string polygonString;

        /// <summary>
        /// Textovy retezec obsahuje souradnice bodu polygonu (vzdusneho prostoru)
        /// </summary>
        [XmlElement("POLYGON")]
        public string PolygonString 
        {
            set 
            {
                this.polygonString = value;
                this.Coordinates = this.ParseCoordinates(this.polygonString);
            }
            get
            {
                return this.polygonString;
            }
        }

        /// <summary>
        /// Seznam souradnic bodu tvorici vzdusny prostor.
        /// </summary>
        public List<Location> Coordinates { get; set; }

        /// <summary>
        /// Seznam rozparsovanych souradnic vzdusneho prostoru.
        /// </summary>
        /// <param name="polygonString">Textovy retezec obsahujici souradnice vzdusneho prostoru.</param>
        /// <returns>Seznam rozparsovanych vzdusnych prostoru.</returns>
        private List<Location> ParseCoordinates(string polygonString)
        {
            return polygonString.Split(',')
                .Select(x =>
                {
                    x = x.Trim();
                    var coordinateString = x.Split(' ');
                    var location = new Location {
	                    Longitude = float.Parse(coordinateString[0].Trim(), CultureInfo.InvariantCulture),
	                    Latitude = float.Parse(coordinateString[1].Trim(), CultureInfo.InvariantCulture)
                    };
                    return location;
                }).ToList();
        }
    }
}
