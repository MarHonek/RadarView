namespace OgnAPI.Client
{
    /// <summary>
    /// Konfigurační konstanty.
    /// </summary>
    public static class OgnClientConstants
    {
        /// <summary>
        /// Název serveru.
        /// </summary>
        public const string OGN_DEFAULT_SERVER_NAME = "aprs.glidernet.org";

        /// <summary>
        /// Název aplikace.
        /// </summary>
        public const string OGN_DEFAULT_APP_NAME = "fi_muni_thesis";

        /// <summary>
        /// Verze aplikace.
        /// </summary>
        public const string OGN_DEFAULT_APP_VERSION = "1.0.0";

        /// <summary>
        /// Port OGN služby (bez filtru).
        /// </summary>
        public const int OGN_DEFAULT_SRV_PORT = 10152;

        /// <summary>
        /// Port OGN služby (s filtrem).
        /// </summary>
        public const int OGN_DEFAULT_SRV_PORT_FILTERED = 14580;

        /// <summary>
        /// Čas po kterém s v případě výpadku spojení klient pokusí spojení obnovit (milisekundy).
        /// </summary>
        public const int OGN_DEFAULT_RECONNECTION_TIMEOUT_MS = 5000;

        /// <summary>
        /// Časový interval ping dotazu na serveru. Pingo ověří dostupnost serveru (milisekundy).
        /// </summary>
        public const int OGN_PING_TIMEOUT = 20000;

        /// <summary>
        /// Časový interval v jakém budou na server zasílaný zprávy pro udrždní spojení (milisekundy).
        /// </summary>
        public const int OGN_CLIENT_DEFAULT_KEEP_ALIVE_INTERVAL_MS = 10 * 60 * 1000;

    }
}
