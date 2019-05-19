using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarView.Model.Service.ImageDownloadClient
{
	/// <summary>
	/// Klient pro stahování obrázku.
	/// </summary>
	public interface IImageDownloadClient
	{
		/// <summary>
		/// Získá hlavičku z odpovědi na http dotaz.
		/// </summary>
		/// <param name="url">Url serveru.</param>
		/// <returns>Hlavička odpovědi.</returns>
		Task<string> GetHeaderAsync(string url);

		/// <summary>
		/// Stáhne obrázek ze serveru.
		/// </summary>
		/// <param name="url">Url serveru.</param>
		/// <param name="imagePath">Místo na disku, kde bude obrázek uložen.</param>
		Task DownloadImageAsync(string url, string imagePath);
	}
}
