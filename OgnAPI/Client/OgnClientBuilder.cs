namespace OgnAPI.Client
{
    /// <summary>
    /// Builder pro třídu OgnClient.
    /// </summary>
    public class OgnClientBuilder
    {
        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_SERVER_NAME"/>
        /// </summary>
        private string srvName = OgnClientConstants.OGN_DEFAULT_SERVER_NAME;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_SRV_PORT"/>
        /// </summary>
        private int srvPort = OgnClientConstants.OGN_DEFAULT_SRV_PORT;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_SRV_PORT_FILTERED"/>
        /// </summary>
        private int srvPortFiltered = OgnClientConstants.OGN_DEFAULT_SRV_PORT_FILTERED;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_RECONNECTION_TIMEOUT_MS"/>
        /// </summary>
        private int reconnectionTimeout = OgnClientConstants.OGN_DEFAULT_RECONNECTION_TIMEOUT_MS;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_CLIENT_DEFAULT_KEEP_ALIVE_INTERVAL_MS"/>
        /// </summary>
        private int keepAlive = OgnClientConstants.OGN_CLIENT_DEFAULT_KEEP_ALIVE_INTERVAL_MS;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_APP_NAME"/>
        /// </summary>
        private string appName = OgnClientConstants.OGN_DEFAULT_APP_NAME;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_APP_VERSION"/>
        /// </summary>
        private string appVersion = OgnClientConstants.OGN_DEFAULT_APP_VERSION;

        /// <summary>
        /// Určuje zda mají být shromažďováný logy.
        /// </summary>
        private bool collectLogs;

        public OgnClientBuilder ServerName(string name)
        {
            this.srvName = name;
            return this;
        }

        public OgnClientBuilder Port(int port)
        {
            this.srvPort = port;
            return this;
        }

        public OgnClientBuilder PortFiltered(int port)
        {
            this.srvPortFiltered = port;
            return this;
        }

        public OgnClientBuilder ReconnectionTimeout(int timeout)
        {
            this.reconnectionTimeout = timeout;
            return this;
        }

        public OgnClientBuilder AppName(string name)
        {
            this.appName = name;
            return this;
        }

        public OgnClientBuilder AppVersion(string version)
        {
            this.appVersion = version;
            return this;
        }

        public OgnClientBuilder KeepAlive(int keepAliveInt)
        {
            this.keepAlive = keepAliveInt;
            return this;
        }

        public OgnClientBuilder CollectLogs(bool collectLogs)
        {
            this.collectLogs = collectLogs;
            return this;
        }

        public OgnClient Build()
        {
            return new OgnClient(this.srvName, this.srvPort, this.srvPortFiltered, this.reconnectionTimeout, this.keepAlive, this.appName, this.appVersion);
        }
    }
}
