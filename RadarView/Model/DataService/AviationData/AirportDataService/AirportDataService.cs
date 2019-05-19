using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RadarView.Model.Entities.AviationData.Airports;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Service.AviationData;

namespace RadarView.Model.DataService.AviationData.AirportDataService
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class AirportDataService : IAirportDataService
	{
		private IAirportFileReader _airportFileReader;


		public AirportDataService(IAirportFileReader airportFileReader)
		{
			this._airportFileReader = airportFileReader;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<List<Airport>> GetNotClosedAirportsInBoundingBoxByCountryAsync(BoundingBox boundingBox, string countryCode)
		{
			var airports = await this._airportFileReader.ListAllAsync();
			//ICAO length = 4 -> vybere pouze registrované letiště.
			return airports.Where(x => boundingBox.Contains(x.Value.Location) && x.Value.IcaoIdent.Length == 4 && x.Value.Country == countryCode && x.Value.AirportType != AirportType.Closed).Select(x => x.Value).ToList();
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<List<Airport>> GetAllAirportsAsync()
		{
			var airports = await this._airportFileReader.ListAllAsync();
			return airports.Select(x => x.Value).ToList();
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<List<Airport>> GetAllNonClosedAirportsWithLeastOneRunwayInCountryAsync(string country)
		{
			var airports = await this._airportFileReader.ListAllAsync();
			var airportsInCountry = airports.Where(x => x.Value.Country == country && x.Value.Runway.Count > 0 && x.Value.IcaoIdent.Length == 4).Select(x => x.Value).ToList();
			foreach (var airport in airportsInCountry) {
				airport.Runway.RemoveAll(r => r.Closed);
			}

			return airportsInCountry;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<Airport> GetAirportByIcaoAsync(string icao)
		{
			var airports = await this._airportFileReader.ListAllAsync();
			return airports.ContainsKey(icao) ? airports[icao] : null;
		}
	}
}
