using System;

namespace OgnAPI.Client
{
    /// <summary>
    /// Delegát pro událost, která je vyvolána, když jsou data předána klientovi.
    /// </summary>
    public delegate void AircraftBeaconEventHandler(object sender, OgnEventArgs eventArgs);

    public interface IOgnClient
    {
        /// <summary>
        /// Událost je vyvolána (v pozadí), jestliže je jsou data zpracována.
        /// </summary>
        event AircraftBeaconEventHandler AircraftBeaconProcessedAsyncEventHandler;

        /// <summary>
        /// Událost je vyvolána pokud došlo k přerušení spojení se serverem.
        /// </summary>
        event EventHandler ConnectionErrorEventHandler;

        /// <summary>
        /// Událost vyvolána pokud došlo k obnovení spojení se serverem.
        /// </summary>
        event EventHandler ConnectionRestoredEventHandler;

        /// <summary>
        /// Přopojí klienta k OGN serveru (bez filtru letadel).
        /// </summary>
		/// <param name="enableLogs">Příznak zda mají být informace logovány.</param>
		void Connect(bool enableLogs = false);

		/// <summary>
		/// Připojí klienta k OGN serveru.
		/// </summary>
		/// <param name="filter">filtruje letadla, která budou klientovi předána. Pokud je null, klient se připojí bez filtru.</param>
		/// <param name="enableLogs">>Příznak zda mají být informace logovány.</param>
		void Connect(string filter, bool enableLogs = false);

        /// <summary>
        ///  Odpojí klienta z OGN serveru.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Zapne zpětné přehrávání.
        /// </summary>
        /// <returns>Časový posun informací vůči aktuálnímu času.</returns>
        TimeSpan StartReplay(string id);

		/// <summary>
		/// Posun lokálního systemového času vůči času serveru.
		/// </summary>
        long CurrentSynchronizationOffset { get; set; }
    }
}
