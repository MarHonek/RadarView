using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using RadarView.Model.Entities.Exceptions;

namespace RadarView.Model.Service.ImageDownloadClient
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class ImageDownloadClient : IImageDownloadClient
	{
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task<string> GetHeaderAsync(string url)
		{
			try {
				var request = (HttpWebRequest) WebRequest.Create(url);
				request.Timeout = 10000;
				var response = await request.GetResponseAsync();
				var header = response.Headers.Get("X-Frame-Date");
				return header;

			} catch (WebException webException) {
				if (webException.Status == WebExceptionStatus.Timeout) {
					var errorMessage = "Vyprsel cas pro stazeni hlavicky ze serveru.";
					Debug.WriteLine(errorMessage);
					throw new ImageDownloadClientException(errorMessage, webException);
				}
				else {
					var errorMessage = "Chyba pri zjistovani data a casu aktualniho radaroveho snimku srazek." +
					                   webException.StackTrace;
					Debug.WriteLine(errorMessage);
					throw new ImageDownloadClientException(errorMessage, webException);
				}
			} catch (Exception exception) {
				var errorMessage = "Neznama chyba v ImageDownloadClient";
				Debug.WriteLine(errorMessage);
				throw new ImageDownloadClientException(errorMessage, exception.InnerException);
			}
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public async Task DownloadImageAsync(string url, string imagePath)
		{
			try {
				using (var client = new WebClient()) {
					await client.DownloadFileTaskAsync(url, imagePath);
				}
			} catch (Exception e) {
				Debug.WriteLine(e);
				throw new ImageDownloadClientException("Neznama chyba pri stahovani obrazku ze serveru.", e);
			}
		}
	}
}
