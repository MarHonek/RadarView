using System;
using System.IO;
using System.Threading.Tasks;
using RadarView.Model.Service.ImageDownloadClient;

namespace RadarView.Model.DataService.Map.MapImageDataSource
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class MapImageDataService : IMapImageDataService
	{
		private readonly IImageDownloadClient _imageDownloadClient;

		/// <summary>
		/// Koncovka dočasného souboru.
		/// </summary>
		private const string TEMP_FILE_NAME_EXTENSION = "temp";


		public MapImageDataService(IImageDownloadClient imageDownloadClient)
		{
			this._imageDownloadClient = imageDownloadClient;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task DownloadMapAsync(string url, string mapName, int zoomLevel, string airportIcao)
		{
			await this._imageDownloadClient.DownloadImageAsync(url, this.GetMapPath(mapName, zoomLevel, airportIcao, TEMP_FILE_NAME_EXTENSION));
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void ReplaceTemporaryFile(string mapName, float zoomLevel, string airportIcao)
		{
			if (File.Exists(this.GetMapPath(mapName, zoomLevel, airportIcao))) {
				File.Delete(this.GetMapPath(mapName, zoomLevel, airportIcao));
			}

			File.Move(this.GetMapPath(mapName, zoomLevel, airportIcao, TEMP_FILE_NAME_EXTENSION), this.GetMapPath(mapName, zoomLevel, airportIcao));
		}


		/// <summary>
		/// Vrací kompletní cestu k mapovým obrázkům.
		/// </summary>
		/// <param name="zoomLevel">zoom level mapy</param>
		/// <param name="fileNameComment">poznámka uvedena v názvu</param>
		/// <param name="airportIcao">Icao letiště</param>
		/// <returns>cestu a název souboru</returns>
		private string GetMapPath(string mapName, float zoomLevel, string airportIcao, string fileNameComment = null)
		{
			var fileName = GetFileName(mapName, zoomLevel, airportIcao, fileNameComment);
			var fileNameWithExtension = fileName + ".jpeg";
			return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", fileNameWithExtension);
		}

		/// <summary>
		/// Vrací název souboru mapového podkladu.
		/// </summary>
		/// <param name="zoomLevel">zoom level mapy</param>
		/// <param name="airportIcao">Icao letiště</param>
		/// <param name="fileNameComment">poznámka uvedena v názvu</param>
		/// <param name="mapName">Název mapového podkladu.</param>
		/// <returns></returns>
		private static string GetFileName(string mapName, float zoomLevel, string airportIcao, string fileNameComment = null)
		{
			return mapName + "_" + airportIcao + "_" + zoomLevel + fileNameComment;
		}
	}
}
