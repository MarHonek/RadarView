using System.Collections.Generic;
using System.Threading.Tasks;
using RadarView.Model.Entities.AviationData.Airports;

namespace RadarView.Model.Service.AviationData
{
	/// <summary>
	/// Třída se stará o načítání letišť ze souboru.
	/// Zdroj dat je služba ourairports.com
	/// </summary>
	public interface IAirportFileReader
	{
		/// <summary>
		/// Vrátí všechny letiště načtené ze souboru.
		/// Klíč je ICAO identifikátor letiště jako reference.
		/// </summary>
		/// <returns>Všechny deserializovane letiště.</returns>
		Task<Dictionary<string, Airport>> ListAllAsync();

		/// <summary>
		/// Načte vsechny informace ze souboru.
		/// </summary>
		Task ReadAllAsync();
	}
}
