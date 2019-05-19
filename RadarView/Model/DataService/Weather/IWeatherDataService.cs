using System.Threading.Tasks;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Entities.Weather;

namespace RadarView.Model.DataService.Weather
{
	/// <summary>
	/// DataService pro ziskavani informaci o pocasi.
	/// </summary>
	public interface IWeatherDataService
	{
		/// <summary>
		/// Ziska informace o pocasi pro zadanou oblast.
		/// </summary>
		/// <param name="location">Zemepisne souradnice.</param>
		/// <returns>Aktualni informace o pocasi.</returns>
		Task<WeatherData> GetCurrentWeatherAsync(Location location);
	}
}
