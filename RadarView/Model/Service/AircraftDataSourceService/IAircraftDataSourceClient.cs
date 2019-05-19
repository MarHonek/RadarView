using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadarView.Model.Entities.AviationData;
using RadarView.Model.Entities.Geographic;

namespace RadarView.Model.Service.AircraftDataSourceService
{
	/// <summary>
	/// Shromažduje data o aktuálních polohách letadel z ruzných zdrojů a transformuje je do jednotné formy pomocí objektu třídy AircraftRawData.
	/// </summary>
	public interface IAircraftDataSourceClient
	{
		/// <summary>
		/// Spustí získávání polohových dat letadel ze serverů skrze API.
		/// Polohová data jsou omezena v oblasti dané parametrem boundingBox.
		/// Polohová data je možné spustit z logů uložených v souboru.
		/// </summary>
		/// <param name="monitoredArea">hranice prostoru v kterém jsou polohová data žádána.</param>
		void Connect(BoundingBox monitoredArea);

		/// <summary>
		/// Připojí klienta k serveru Open Glider Network.
		/// </summary>
		/// <param name="monitoredArea">Sledovaná oblast.</param>
		void ConnectToOgn(BoundingBox monitoredArea);

		/// <summary>
		/// Připojí klienta k serveru Open Sky Network.
		/// </summary>
		/// <param name="monitoredArea">Sledovaná oblast.</param>
		void ConnectToOsn(BoundingBox monitoredArea);

		/// <summary>
		/// Odpojí klienta ze všech serverů (API).
		/// </summary>
		void Disconnect();

		/// <summary>
		/// Odpojí klienta ze serveru Open Glider Network.
		/// </summary>
		void DisconnectFromOgn();

		/// <summary>
		/// Odpojí klienta ze serveru Open Sky Network.
		/// </summary>
		void DisconnectFromOsn();

		/// <summary>
		/// Spustí replay pro datový zdroj OGN.
		/// </summary>
		/// <param name="fileName">Název souboru, který bude přehrán.</param>
		void StartOgnReplay(string fileName);

		/// <summary>
		/// Spustí replay pro datový zdroj OSN.
		/// </summary>
		/// <param name="fileName">Název souboru, který bude přehrán.</param>
		void StartOsnReplay(string fileName);

		/// <summary>
		/// Inicializuje klienta.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Rozdíl mezi časem logů a aktuálním časem (sekundy).
		/// </summary>
		int ReplayOffset { get; set; }

		/// <summary>
		/// Událost vývolána pokud objekt obdrží nové data
		/// </summary>
		event EventHandler<AircraftDataSourceEventArgs> AircraftReceived;
	}
}
