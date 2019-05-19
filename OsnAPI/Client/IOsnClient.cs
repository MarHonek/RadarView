using System;

namespace OsnAPI.Client
{
    /// <summary>
    /// Rozhraní klienta pro získávání dat ze služby OpenSkyNetwork
    /// </summary>
    public interface IOsnClient
    {
        /// <summary>
        /// Vyvolá událost (v jiném vlákně) jestliže byla data přijdata a zpracována.
        /// </summary>
        event EventHandler<OsnAircraftEventArgs> AircraftDataReceived;

        /// <summary>
        /// Událost vyvolána pokud je služba nedostupná.
        /// </summary>
        event EventHandler ConnectionLost;

        /// <summary>
        /// Událost vyvolána při obnovení dostupnosti služby. Musela být předtím vyvolána událost ConnectionLost.
        /// </summary>
        event EventHandler ConnectionRestored;

        /// <summary>
        /// Připojí se pomocí klienta ke službě OpenSkyNetwork. Začne sledovat zadanou oblast.
        /// </summary>
        /// <param name="north">Severní hranice oblasti.</param>
        /// <param name="west">Západní hranice oblasti.</param>
        /// <param name="south">Jižní hranice oblasti.</param>
        /// <param name="east">Východní hranice oblasti.</param>
        /// <param name="enableLogs">Povolit logování dat.</param>
        void Connect(float north, float west, float south, float east, bool enableLogs = false);

        /// <summary>
        ///  Odpojí klienta ze služby OpenSkyNetwork.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Spustí načítání dat ze souboru.
        /// </summary>
        /// <returns>časový posun informací vůči aktuálnímu času.</returns>
        TimeSpan StartReplay(string id);

		/// <summary>
		/// Aktuální posun lokálního systemového času vůči času serveru.
		/// </summary>
		long CurrentSynchronizationOffset { get; set; }
	}
}
