using System;
using System.Threading.Tasks;

namespace RadarView.Model.DataService.Weather.PrecipitationRadarDataService
{
	/// <summary>
	/// TODO: dodelat 
	/// DataService pro....
	/// Stará se o stahování radarových snímků srážek.
	/// </summary>
	public interface IPrecipitationRadarDataService
	{

		/// <summary>
		/// Zjistí čas poslední aktualizace radarových snímků srážek.
		/// </summary>
		/// <returns>Datum a čas poslední aktualizace.</returns>
		Task<DateTime> GetLastPrecipitationRadarUpdateDateTimeAsync();


		/// <summary>
		/// Stáhne aktuální radarových snímek srážek.
		/// </summary>
		/// <returns>Čas platnosti posledního nejaktuálnějšího snímku.</returns>
		Task<DateTime> DownloadCurrentPrecipitationRadarImageAsync();


		/// <summary>
		/// Událost vyvolána při úspěšném stažení radarového snímku.
		/// </summary>
		event EventHandler ImageWasDownloaded;

	}
}
