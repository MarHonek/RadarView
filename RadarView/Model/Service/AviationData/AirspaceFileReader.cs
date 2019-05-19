using System.Collections.Generic;
using System.Threading.Tasks;
using RadarView.Model.Entities.AviationData.Airspace;

namespace RadarView.Model.Service.AviationData
{
	/// <summary>
	/// <inheritdoc cref="IAirspaceFileReader"/>
	/// </summary>
	public class AirspaceFileReader : OpenAipAviationDataReader<XmlOpenAipAirspace, Airspace>, IAirspaceFileReader
    {
        /// <summary>
        /// Nazev souboru.
        /// </summary>
        private const string FileName = "openaip_airspace_czech_republic_cz.aip";

		/// <summary>
		/// Vsechny vzdusne prostory.
		/// </summary>
        private List<Airspace> allAirspace;

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public List<Airspace> ListAll()
        {
			if (this.allAirspace == null) {
				this.ReadAll(FileName);
			}

			return this.allAirspace;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task ReadAllAsync()
		{
			await Task.Run(() => {
				this.ReadAll(FileName);
				this.allAirspace = this.aviationData.AllAirspace;
			});
		}
    }
}
