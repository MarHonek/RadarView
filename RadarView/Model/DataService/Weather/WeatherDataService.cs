using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Entities.Weather;
using RadarView.Model.Service.ApiRestClient;
using RadarView.Properties;

namespace RadarView.Model.DataService.Weather
{

	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class WeatherDataService : IWeatherDataService
	{
		/// <summary>
		/// Url sluzby, ktera poskytuje informace o pocasi.
		/// </summary>
		private readonly string Url = Settings.Default.MeteoDataUrl;

		/// <summary>
		/// Api klic pro sluzbu, ktera poskutuje informace o pocasi.
		/// </summary>
		private readonly string ApiKey = Settings.Default.MeteoDataApiKey;


		private IApiRestClient _restClient;


		public WeatherDataService(IApiRestClient restClient)
		{
			this._restClient = restClient;
		}


		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<WeatherData> GetCurrentWeatherAsync(Location location)
		{
			var urlWithParameters = string.Format(this.Url, location.Latitude, location.Longitude, this.ApiKey);
			return await this._restClient.MakeGerRequestAsync<WeatherData>(urlWithParameters, null, null, null);
		}
	}
}
