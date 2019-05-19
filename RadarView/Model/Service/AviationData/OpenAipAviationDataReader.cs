using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace RadarView.Model.Service.AviationData
{
    /// <summary>
    /// Abstraktni trida, ktera se stara o nacitani leteckych dat ze souboru.
    /// T - trida podle ktere budou data deserializovana.
    /// U - Data bez hlavicky.
    /// </summary>
    public abstract class OpenAipAviationDataReader<T, U> where T : class where U : class
    {

        /// <summary>
        /// Serializovana data.
        /// </summary>
        protected T aviationData; 

        /// <summary>
        /// Cesta k souboru,
        /// </summary>
        private static readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "AviationData");


        /// <summary>
        /// Nacte data ve formatu XML a provede deserializaci.
        /// </summary>
        /// <param name="filename">Nazev souboru.</param>
        protected void ReadAll(string filename)
        {
	        var serializer = new XmlSerializer(typeof(T));
	        T openAipAirspace;

	        using (var reader = XmlReader.Create(Path.Combine(path, filename))) {
		        openAipAirspace = (T) serializer.Deserialize(reader);
	        }

	        this.aviationData = openAipAirspace;
        }
    }
}
