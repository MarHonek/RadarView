using System.Collections.Generic;
using System.Threading.Tasks;
using RadarView.Model.Entities.AviationData.Airports;
using RadarView.Model.Entities.Geographic;

namespace RadarView.Model.DataService.AviationData.AirportDataService
{
	/// <summary>
	/// DataService pro přístup k informacím o letištích.
	/// </summary>
	public interface IAirportDataService
	{
		/// <summary>
		/// Vrátí letiště nacházející se v zadané oblasti.
		/// </summary>
		/// <param name="boundingBox">Hranice zvoleného prostoru.</param>
		/// <param name="countryCode">Zkratka státu.</param>
		/// <returns>Vyfiltrovane letiste.</returns>
		Task<List<Airport>> GetNotClosedAirportsInBoundingBoxByCountryAsync(BoundingBox boundingBox, string countryCode);

		/// <summary>
		/// Vrátí všechny letiště.
		/// </summary>
		/// <returns>Seznam letišť.</returns>
		Task<List<Airport>> GetAllAirportsAsync();

		/// <summary>
		/// Vrátí všechny neuzavřené letiště, které se nacházejí v zadané zemi a mají alespoň jednu vyplněnou dráhu.
		/// </summary>
		/// <param name="country">zkratka země.</param>
		/// <returns>Seznam vyfiltrovaných letišť.</returns>
		Task<List<Airport>> GetAllNonClosedAirportsWithLeastOneRunwayInCountryAsync(string country);

		/// <summary>
		/// Vrátí letiště podle zadaného ICAO kódu.
		/// </summary>
		/// <param name="icao">ICAO identifikátor letiště.</param>
		/// <returns>Letiště.</returns>
		Task<Airport> GetAirportByIcaoAsync(string icao);
	}
}
