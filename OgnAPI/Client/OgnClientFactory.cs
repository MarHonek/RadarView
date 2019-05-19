namespace OgnAPI.Client
{
    /// <summary>
    /// Třídá sloužící k vytvoření instance třídy OGN klient.
    /// </summary>
    public class OgnClientFactory
    {
        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_SERVER_NAME"/>
        /// </summary>
        private const string SERVER_NAME = OgnClientConstants.OGN_DEFAULT_SERVER_NAME;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_SRV_PORT"/>
        /// </summary>
        private const int PORT = OgnClientConstants.OGN_DEFAULT_SRV_PORT;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_SRV_PORT_FILTERED"/>
        /// </summary>
        private const int PORT_FILTERED = OgnClientConstants.OGN_DEFAULT_SRV_PORT_FILTERED;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_RECONNECTION_TIMEOUT_MS"/>
        /// </summary>
        private const int RECONNECTION_TIMEOUT = OgnClientConstants.OGN_DEFAULT_RECONNECTION_TIMEOUT_MS;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_CLIENT_DEFAULT_KEEP_ALIVE_INTERVAL_MS"/>
        /// </summary>
        private const int KEEP_ALIVE_INTERVAL = OgnClientConstants.OGN_CLIENT_DEFAULT_KEEP_ALIVE_INTERVAL_MS;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_APP_NAME"/>
        /// </summary>
        private const string APP_NAME = OgnClientConstants.OGN_DEFAULT_APP_NAME;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_APP_VERSION"/>
        /// </summary>
        private const string APP_VERSION = OgnClientConstants.OGN_DEFAULT_APP_VERSION;

        public static OgnClientBuilder GetBuilder()
        {
            return new OgnClientBuilder()
                .ServerName(SERVER_NAME)
                .Port(PORT)
                .PortFiltered(PORT_FILTERED)
                .ReconnectionTimeout(RECONNECTION_TIMEOUT)
                .AppName(APP_NAME)
                .AppVersion(APP_VERSION)
                .KeepAlive(KEEP_ALIVE_INTERVAL);
        }

        /// <summary>
        /// Vytvoří instanci třídy OgnClient.
        /// </summary>
        /// <returns></returns>
        public static IOgnClient CreateClient()
        {
            return GetBuilder().Build();
        }
    }
}
