using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RadarView.Model.Entities.Exceptions;
using RadarView.Model.Entities.Weather;
using RadarView.Model.Service.Utils;
using RestSharp;

namespace RadarView.Model.Service.ApiRestClient
{
	public class ApiRestClient : IApiRestClient
	{
		/// <summary>
		/// Rest klient.
		/// </summary>
		private IRestClient restClient;

		/// <summary>
		/// Instance serializeru / deserializeru, ktery vyuziva Json.NET.
		/// </summary>
		private RestSharpJsonNetSerializer jsonSerializer;

		public ApiRestClient()
		{
			this.restClient = new RestClient();
			this.restClient.Timeout = 5000;

			this.jsonSerializer = RestSharpJsonNetSerializer.Default;
			this.restClient.AddHandler("application/json", this.jsonSerializer);
			this.restClient.AddHandler("text/json", this.jsonSerializer);
			this.restClient.AddHandler("text/x-json", this.jsonSerializer);
			this.restClient.AddHandler("text/javascript", this.jsonSerializer);
			this.restClient.AddHandler("application/javascript", this.jsonSerializer);
			this.restClient.AddHandler("*+json", this.jsonSerializer);
		}

		/// <summary>
		/// Vytvori request a nastavi hlavicky, ktere jsou spolecne pro vsechny requesty.
		/// </summary>
		/// <param name="resource">Adresa zdroju.</param>
		/// <param name="method">Metoda</param>
		/// <returns>Vygenerovany request.</returns>
		private RestRequest InitializeRequestForResource(string resource, Method method = Method.GET)
		{
			var request = new RestRequest(resource, method) {
				RequestFormat = DataFormat.Json,
				JsonSerializer = this.jsonSerializer,
			};
			return request;
		}


		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<TItem> MakeGerRequestAsync<TItem>(string url, string resource, string itemId, object body)
		{
			try {
				this.restClient.BaseUrl = new Uri(url);
				var request = this.InitializeRequestForResource(resource + (itemId == null ? "" : "/" + itemId));
				request.AddBody(body);
				var response = await this.restClient.ExecuteGetTaskAsync<TItem>(request);

				if (response.ErrorException != null || response.Data == null) {
					throw new RestApiClientException("Chyba pri GET requestu REST API");
				}
				return response.Data;
			}
			catch (Exception ex) {
				throw new RestApiClientException("Chyba pri GET requestu REST API");
			}
		}
	}
}
