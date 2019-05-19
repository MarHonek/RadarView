using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using MoreLinq;
using RadarView.Model.Entities.AviationData.Airports;
using RadarView.Model.Entities.Geographic;

namespace RadarView.Model.Service.AviationData
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
    public class AirportFileReader : IAirportFileReader
    {
		/// <summary>
		/// Cesta k souboru,
		/// </summary>
		private static readonly string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "AviationData");

		/// <summary>
		/// Nazev souboru s informacemi o letistich.
		/// </summary>
		private const string AirportFileName = "airports.csv";

		/// <summary>
		/// Nazev souboru s informacemi o vzletovych drahahch.
		/// </summary>
		private const string RunwaysFileName = "runways.csv";


		/// <summary>
		/// Vsechny letiste.
		/// </summary>
		private Dictionary<string, Airport> airports;


		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<Dictionary<string, Airport>> ListAllAsync()
        {
	        if (this.airports == null) {
		        await this.ReadAllAsync();
	        }

	        return this.airports;
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task ReadAllAsync()
		{
			await Task.Run(() =>
			{
				this.airports = new Dictionary<string, Airport>();
				this.airports = this.ReadAirports();
				this.ReadRunwaysAsync();
			});
		}

		/// <summary>
		/// Nacte letiste ze souboru
		/// </summary>
		/// <returns>Slovnik obsahujici letiste. Klic je ICAO identifikator letiste.</returns>
		private Dictionary<string, Airport> ReadAirports()
		{
			var airports = new Dictionary<string, Airport>();

			using (var streamReader = new StreamReader(Path.Combine(path, AirportFileName))) {
				streamReader.ReadLine();
				while (!streamReader.EndOfStream) {
					var line = streamReader.ReadLine();
					var airport = this.DeserializeAirport(line);
					if (airport != null) {
						airports.Add(airport.IcaoIdent, airport);
					}
				}
			}

			return airports;
		}

		/// <summary>
		/// Nacte vzletove drahy ze souboru
		/// </summary>
		/// <returns>Slovnik obsahujici vzletove drahy. Klic je ICAO identifikator letiste.</returns>
		private void ReadRunwaysAsync()
		{
			using (var streamReader = new StreamReader(Path.Combine(path, RunwaysFileName))) {

				streamReader.ReadLine();
				while (!streamReader.EndOfStream) {

					var line = streamReader.ReadLine();
					var runway = this.DeserializeRunway(line);

					if (runway != null) {
						if (this.airports.ContainsKey(runway.AirportIcaoIdent)) {
							var runways = this.airports[runway.AirportIcaoIdent].Runway;
							runways.Add(runway);
						}
					}
				}
			}
		}

		/// <summary>
		/// Deserializuje informace o letisti.
		/// </summary>
		/// <param name="line">serializovane informace o letisti.</param>
		/// <returns>Letiste.</returns>
		private Airport DeserializeAirport(string line)
		{

			var splitLine = line.Split(',');
			for (var i = 0; i < splitLine.Length; i++) {
				splitLine[i] = splitLine[i].Replace("\"", "");
			}

			if (splitLine.Length != 18) {
				return null;
			}

			var cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
			cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";

			var airport = new Airport() {
				Id = Convert.ToInt32(splitLine[0]),
				IcaoIdent = splitLine[1],
				AirportType = AirportTypeHelper.DeserializeAirportType(splitLine[2]),
				Name = splitLine[3],
				Location = new Location(float.Parse(splitLine[4], NumberStyles.Any, cultureInfo), float.Parse(splitLine[5], NumberStyles.Any, cultureInfo)),
				Altitude = string.IsNullOrEmpty(splitLine[6]) ? 0 : Convert.ToInt32(splitLine[6]),
				Continent = splitLine[7],
				Country = splitLine[8],
				Region = splitLine[9],
				City = splitLine[10],
				GpsCode = splitLine[12],
				IataCode = splitLine[13],
				LocalCode = splitLine[14]
			};
			return airport;


		}

		/// <summary>
		/// Deserializuje informace o vzletove draze.
		/// </summary>
		/// <param name="line">serializovane informace o vzletove draze.</param>
		/// <returns>Vzletova draha.</returns>
		private Runway DeserializeRunway(string line)
		{
			var splitLine = line.Split(',');
			for (var i = 0; i < splitLine.Length; i++) {
				splitLine[i] = splitLine[i].Replace("\"", "");
			}

			if (splitLine.Length != 20) {
				return null;
			}

			var cultureInfo = (CultureInfo)CultureInfo.CurrentCulture.Clone();
			cultureInfo.NumberFormat.CurrencyDecimalSeparator = ".";

			var runway = new Runway() {
				Id = Convert.ToInt32(splitLine[0]),
				AirportIdReference = Convert.ToInt32(splitLine[1]),
				AirportIcaoIdent = splitLine[2],
				Length = string.IsNullOrEmpty(splitLine[3]) ? 0 : Convert.ToInt32(splitLine[3]),
				Width = string.IsNullOrEmpty(splitLine[4]) ? 0 : Convert.ToInt32(splitLine[4]),
				Surface = splitLine[5],
				Lighted = Convert.ToInt32(splitLine[6]) != 0,
				Closed = Convert.ToInt32(splitLine[7]) != 0,
				StartName = splitLine[8],
				StartLocation = string.IsNullOrEmpty(splitLine[9]) || string.IsNullOrEmpty(splitLine[10]) ? null : new Location(float.Parse(splitLine[9], NumberStyles.Any, cultureInfo), float.Parse(splitLine[10], NumberStyles.Any, cultureInfo)),
				StartAltitude = string.IsNullOrEmpty(splitLine[11]) ? 0 : Convert.ToInt32(splitLine[11]),
				StartHeading = string.IsNullOrEmpty(splitLine[12]) ? 0 : float.Parse(splitLine[12], NumberStyles.Any, cultureInfo),
				EndName = splitLine[14],
				EndLocation = string.IsNullOrEmpty(splitLine[15]) || string.IsNullOrEmpty(splitLine[16]) ? null : new Location(float.Parse(splitLine[15], NumberStyles.Any, cultureInfo), float.Parse(splitLine[16], NumberStyles.Any, cultureInfo)),
				EndAltitude = string.IsNullOrEmpty(splitLine[17]) ? 0 : Convert.ToInt32(splitLine[17]),
				EndHeading = string.IsNullOrEmpty(splitLine[18]) ? 0 : float.Parse(splitLine[18], NumberStyles.Any, cultureInfo),
			};
			return runway;
		}
    }
}
