using System.Threading.Tasks;

namespace RadarView.Model.DataService.Map.MapImageDataSource
{
	/// <summary>
	/// DataService pro stahování mapových podkladů.
	/// </summary>
	public interface IMapImageDataService
	{
		/// <summary>
		/// Stáhne mapový podklad
		/// </summary>
		/// <param name="url">Url k serveru s mapovým podkladem.</param>
		/// <param name="mapName">Název mapového podkladu.</param>
		/// <param name="zoomLevel">Zoom level mapy.</param>
		/// <param name="airportIcao">ICAO letiště podle kterého se mapa centruje.</param>
		Task DownloadMapAsync(string url, string mapName, int zoomLevel, string airportIcao);

		/// <summary>
		/// Přejmenuje mapy označené jako dočasné na pernametní.
		/// </summary>
		/// <param name="mapName">Název mapového podkladu.</param>
		/// <param name="zoomLevel">zoom level mapy</param>
		/// <param name="airportIcao">Icao letiště</param>
		void ReplaceTemporaryFile(string mapName, float zoomLevel, string airportIcao);
	}
}
