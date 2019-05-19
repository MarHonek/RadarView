using System;
using System.IO;

namespace OsnAPI.Client
{
    /// <summary>
    /// Konstanty klienta služby OpenSkyNetwork.
    /// </summary>
    public static class OsnClientConstants
    {
        /// <summary>
        /// Url služby OpenSkyNetwork.
        /// </summary>
        public const string URL = "https://opensky-network.org";

        /// <summary>
        /// URI zdrojů.                  
        /// </summary>
        public const string RESOURCES_URI = "/api/states/all";

        /// <summary>
        /// Časový interval určující jak často se provede REST dotaz na službu OpenSky (milisekundy).
        /// </summary>
        public const int MONITORED_AIRCRAFT_UPDATE_INTERVAL = 5000;

        /// <summary>
        /// Čas po kterém se v případě výpadku spojení, pokusí spojení obnovit (milisekundy).
        /// </summary>
        public const int REQUEST_TIMEOUT = 5000;

		/// <summary>
		/// Cesta k souborům.
		/// </summary>
		public static readonly string RootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataSource", "OpenSkyNetwork");
    }
}
