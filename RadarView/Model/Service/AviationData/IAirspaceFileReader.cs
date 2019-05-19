using System.Collections.Generic;
using System.Threading.Tasks;
using RadarView.Model.Entities.AviationData.Airspace;

namespace RadarView.Model.Service.AviationData
{
	/// <summary>
	/// Trida se stara o nacitani vzdusnych prostoru ze souboru.
	/// Data jsou ziskavana ze sluzby OpenAip.
	/// </summary>
	public interface IAirspaceFileReader
	{
		/// <summary>
		/// Vrati vsechny vzdusne prostory nactene ze souboru.
		/// </summary>
		/// <returns>Vsechny deserializovane vzdusne prostory.</returns>
		List<Airspace> ListAll();


		/// <summary>
		/// Načte celý soubor asynchroně.
		/// </summary>
		Task ReadAllAsync();
	}
}
