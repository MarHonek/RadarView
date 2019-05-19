using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using RadarView.Model.Service.ImageDownloadClient;
using RadarView.Properties;

namespace RadarView.Model.DataService.Weather.PrecipitationRadarDataService
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class PrecipitationRadarDataService : IPrecipitationRadarDataService
	{
		/// <summary>
		/// Url pro získání času posledního aktuálního snímku.
		/// </summary>
		private readonly string DateTimeServer = Settings.Default.PrecipitationRadarDateTimeUrl;

		/// <summary>
		/// Url serveru ze kterého se stahují radarové snímky srážek.
		/// </summary>
		private readonly string ImageServer = Settings.Default.PrecipitationRadarImagesServerUrl;

		/// <summary>
		/// Formát času v URL
		/// </summary>
		private readonly string UrlTimeFormat = Settings.Default.PrecipitationRadarResourceTimestampFormat;

		/// <summary>
		/// Poslední aktualizace radarového snímku na serveru.
		/// </summary>
		private DateTime lastPrecipitationRadarImageUpdate;

		/// <summary>
		/// Cesta k snímku.
		/// </summary>
		private readonly string ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "PrecipitationRadar.png");

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public event EventHandler ImageWasDownloaded;


		private readonly IImageDownloadClient _imageDownloadClient;


		public PrecipitationRadarDataService(IImageDownloadClient imageDownloadClient)
		{
			this._imageDownloadClient = imageDownloadClient;
		}


		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<DateTime> GetLastPrecipitationRadarUpdateDateTimeAsync()
		{
			//Z hlavičky určí čas poslední aktualizace snímku.
			var url = string.Format(this.DateTimeServer, 1);
			var result = await this._imageDownloadClient.GetHeaderAsync(url);
			return DateTime.ParseExact(result, this.UrlTimeFormat, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<DateTime> DownloadCurrentPrecipitationRadarImageAsync()
		{
			var result = await this.GetLastPrecipitationRadarUpdateDateTimeAsync();
			if (this.lastPrecipitationRadarImageUpdate != result) {
				var url = string.Format(this.ImageServer, 1, result.ToString(this.UrlTimeFormat));
				await this._imageDownloadClient.DownloadImageAsync(url, this.ImagePath);
				this.ImageWasDownloaded?.Invoke(this, new EventArgs());

				this.lastPrecipitationRadarImageUpdate = result;
			}

			return result;
		}
	}
}
