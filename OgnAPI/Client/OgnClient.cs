using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Logger;
using OgnAPI.Beacon;
using OgnAPI.Beacon.Database;
using OgnAPI.Utils;

namespace OgnAPI.Client
{
    /// <summary>
    /// Klient pro stahování dat o letadlech z OGN serveru.
    /// </summary>
    public class OgnClient : IOgnClient
    {
        /// <summary>
        /// Příznak, že lze data na serveru pouze číst.
        /// </summary>
        private const string READ_ONLY_PASSCODE = "-1";

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_SERVER_NAME"/>
        /// </summary>
        private string ognServerName;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_SRV_PORT"/>
        /// </summary>
        private int ognPort;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_SRV_PORT_FILTERED"/>
        /// </summary>
        private int ognPortFiltered;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_RECONNECTION_TIMEOUT_MS"/>
        /// </summary>
        private int reconnectionTimeout;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_CLIENT_DEFAULT_KEEP_ALIVE_INTERVAL_MS"/>
        /// </summary>
        private int keepAlive;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_APP_NAME"/>
        /// </summary>
        private string appName;

        /// <summary>
        /// <see cref="OgnClientConstants.OGN_DEFAULT_APP_VERSION"/>
        /// </summary>
        private string appVersion;

        /// <summary>
        /// Klient pro načtení OGN databáze s dodatečnými informacemi o letadlech.
        /// </summary>
        private OgnDescriptorDB descriptorDb;

        /// <summary>
        /// Síťová komunikace.
        /// </summary>
        private Socket socket;

        /// <summary>
        /// StreamWriter pro načítání textových řetězců ze síťové komunikace.
        /// </summary>
        private StreamWriter requestStream;

        /// <summary>
        /// Kolekce pro shromaždování dat ze serveru. Data v kolekci jsou postupně zpracovávána.
        /// </summary>
        private BlockingCollection<string> sentenceBlockingCollection = new BlockingCollection<string>();

        /// <summary>
        /// Token pro zrušení tasku.
        /// </summary>
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Lock pro synchronizaci vláken při připojení k serveru.
        /// </summary>
        private static readonly object ConnectLock = new object();

        /// <summary>
        /// Lock pro synchronizaci vláken při odpojení ze serveru.
        /// </summary>
        private static readonly object DisconnectLock = new object();

        /// <summary>
        /// Událost vyvolána pokud jsou příchozí data zpracována a připravena k předání klientovi.
        /// </summary>
        public event AircraftBeaconEventHandler AircraftBeaconProcessedAsyncEventHandler;

        /// <summary>
        /// Událost vyvolána pokud došlo k přerušení spojení se serverem.
        /// </summary>
        public event EventHandler ConnectionErrorEventHandler;

        /// <summary>
        /// Událost vyvolána pokud došlo k obnovení spojení se serverem.
        /// </summary>
        public event EventHandler ConnectionRestoredEventHandler;

        /// <summary>
        /// Logování chyb.
        /// </summary>
        private ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Logování příchozích dat ze serveru.
        /// </summary>
        private SimpleJsonLogger<AircraftBeacon> logger;

        /// <summary>
        /// Atribut určující, zda mají být příchozí data zalogována.
        /// </summary>
        private bool collectLogs;

		/// <summary>
		/// Aktuální posun lokálního systémového času vůči času serveru.
		/// </summary>
		public long CurrentSynchronizationOffset { get; set; }

        /// <summary>
        /// Příznak určující zda je spojení se serverem v pořádku.
        /// </summary>
        private bool isConnectionOk = false;

        /// <summary>
        /// Příznak určující zda byla zavolána metoda Connect.
        /// </summary>
        public bool WasConnectMethodCalled { get; private set; } = false;


        public OgnClient(string aprsServerName, int aprsPort, int aprsPortFiltered, int reconnectionTimeout,
          int keepAlive, string appName, string appVersion)
        {
            this.ognServerName = aprsServerName;
            this.ognPort = aprsPort;
            this.ognPortFiltered = aprsPortFiltered;
            this.reconnectionTimeout = reconnectionTimeout;
            this.keepAlive = keepAlive;
            this.appName = appName;
            this.appVersion = appVersion;

            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DataSource", "OpenGliderNetwork");
            this.logger = new SimpleJsonLogger<AircraftBeacon>(logPath);
        }

        /// <summary>
        /// Začne stahovat data ze serveru.
        /// </summary>
        /// <param name="token">Token pro zrušení vlákna v pozadí.</param>
        /// <param name="aprsFilter">Filter APRS serveru.</param>
        private async Task StartReadDataFromServer(CancellationToken token, string aprsFilter)
        {
	        while (!token.IsCancellationRequested) {
		        try {
			        var port = this.ognPort;
			        string loginSentence = null;

			        //Vygeneruje unikátní id.
			        var clientId = AprsUtils.GenerateClientId();

			        if (aprsFilter == null) {
				        loginSentence = AprsUtils.FormatAprsLoginLine(clientId, READ_ONLY_PASSCODE, this.appName, this.appVersion);
			        } else {
				        port = this.ognPortFiltered;
				        loginSentence = AprsUtils.FormatAprsLoginLine(clientId, READ_ONLY_PASSCODE, this.appName, this.appVersion, aprsFilter);
			        }

			        var hostEntry = Dns.GetHostEntry(this.ognServerName);
			        IPAddress ip = null;

			        if (hostEntry.AddressList.Length > 0) {
				        ip = hostEntry.AddressList[0];
			        }

			        this.socket = new Socket(ip.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			        this.socket.Connect(ip, port);

			        //Zašle přihlašovací údaje na server.
			        this.requestStream = new StreamWriter(new NetworkStream(this.socket));
			        this.requestStream.WriteLine(loginSentence);
			        this.requestStream.Flush();

					//Začne udržovat spojení se serverem.
					this.StartKeepAlive(token, this.requestStream, loginSentence);

					//Zkontroluje dostupnost ogn serveru.
					this.CheckConnectivity(token);

			        //přečte data z serveru.
			        using (var stream = new StreamReader(new NetworkStream(this.socket))) {
				        string line;
				        while ((line = stream.ReadLine()) != null) {

					        if (token.IsCancellationRequested) {
						        break;
					        }

					        this.sentenceBlockingCollection.Add(line);
					        if (!this.isConnectionOk) {
						        this.ConnectionRestoredEventHandler?.Invoke(this, new EventArgs());
								this.isConnectionOk = true;
					        }
				        }
			        }
		        } catch (Exception ex) {
					this.log.Error(string.Format("exception caught while trying to connect to {0}:{1}. retrying in {2} ms", this.ognServerName,
						this.ognPort, this.reconnectionTimeout, ex));

			        if (this.isConnectionOk) {
				       this.ConnectionErrorEventHandler?.Invoke(this, new EventArgs());
						this.isConnectionOk = false;
			        }

			        //timeout při ztrátě spojení.
			        await Task.Delay(this.reconnectionTimeout);
		        } finally {
			        this.requestStream?.Close();
			        this.socket?.Close();
		        }
	        }
        }

        /// <summary>
        /// Zpracuje data čekající v kolekci.
        /// </summary>
        /// <param name="token">Token pro zrušení vlákna.</param>
        private async Task StartProcessData(CancellationToken token)
        {
	        this.descriptorDb = new OgnDescriptorDB();
	        await this.descriptorDb.Initialize();

	        while (!token.IsCancellationRequested) {
		        //aprs -> systme na základě radiokomunikace v reálném čase.
		        var aprsLine = this.sentenceBlockingCollection.Take();
		        var beacon = OgnLineParser.Parse(aprsLine);
		        if (beacon != null) {

			        beacon.Timestamp += this.CurrentSynchronizationOffset;

			        if (this.collectLogs) {
				        this.logger.Write((AircraftBeacon) beacon);
			        }

			        var descriptor = this.descriptorDb?.FindDescriptor(beacon.Address);
			        this.AircraftBeaconProcessedAsyncEventHandler?.Invoke(this, new OgnEventArgs(beacon, descriptor));
		        }
	        }
        }

        /// <summary>
        /// Zapne zpětné přehrávání.
        /// </summary>
        public TimeSpan StartReplay(string id)
        {
			var offset = this.logger.StartReplaying("Replay" + id + ".txt");
			this.logger.LogItemReplayed += this.Logger_LogItemReplayed;
			this.logger.ReplayIsOver += this.Logger_ReplayIsOver;

			return offset;
        }

        /// <summary>
        /// Handler předá klientovi data z logů a nahradí čas zalogování aktuálním časem.
        /// </summary>
        private void Logger_LogItemReplayed(object sender, JsonLogEventArgs<AircraftBeacon> e)
        {
	        var totalSeconds = (int) (DateTime.UtcNow - e.LogDate).TotalSeconds;
	        e.Data.Timestamp += totalSeconds;
	        this.AircraftBeaconProcessedAsyncEventHandler?.Invoke(this, new OgnEventArgs(e.Data, null));

	        if (!this.isConnectionOk) {
				this.isConnectionOk = true;
		        this.ConnectionRestoredEventHandler?.Invoke(this, new EventArgs());
	        }
        }


        /// <summary>
		/// Replay skončil.
		/// </summary>
		private void Logger_ReplayIsOver(object sender, EventArgs e)
		{
			this.ConnectionErrorEventHandler?.Invoke(this, new EventArgs());
		}

        /// <summary>
        /// Pravidelně testuje síťové spojení se serverem.
        /// </summary>
        /// <param name="token">token pro zrušení běžícího tasku.</param>
        private void CheckConnectivity(CancellationToken token)
        {
	        Task.Run(async () => {
		        while (!token.IsCancellationRequested) {
			        await Task.Delay(OgnClientConstants.OGN_PING_TIMEOUT);
			        this.PingHost(OgnClientConstants.OGN_DEFAULT_SERVER_NAME);
		        }
	        }, token);
        }

        /// <summary>
        /// Vyšle ping na serveru.
        /// </summary>
        /// <param name="host">Adresa cílového serveru.</param>
        private void PingHost(string host)
        {
	        var ping = new Ping();
	        try {
		        var reply = ping.Send(host);
	        } catch (PingException ex) {
		        this.log.Error(string.Format("exception caught while trying to connect to {0}:{1}.", this.ognServerName,
			        this.ognPort, ex));
		        this.ConnectionErrorEventHandler?.Invoke(this, new EventArgs());
		        this.isConnectionOk = false;
	        }
        }

        /// <summary>
		/// Začne pravidelně zasílat zprávy pro udržení síťového spojení.
		/// </summary>
		/// <param name="token">Token pro zrušení vlákna.</param>
		/// <param name="writter">writer pro zaslání zprávy.</param>
		/// <param name="loginSentence">věta pro přihlášení.</param>
		private void StartKeepAlive(CancellationToken token, StreamWriter writter, string loginSentence)
		{
			var keepAliveMsg = loginSentence.StartsWith("#") ? loginSentence : "#" + loginSentence;
			Task.Run(async () => {
				while (!token.IsCancellationRequested) {
					await Task.Delay(this.keepAlive);
					writter.WriteLine(keepAliveMsg);
					writter.Flush();
				}
			}, token);
		}

		/// <summary>
        /// <see cref="IOgnClient.Connect"/>
        /// </summary>
        public void Connect(bool enableLogs = false)
        {
            this.Connect(null, enableLogs);
        }

		/// <summary>
		/// <see cref="IOgnClient.Connect(string)"/>
		/// </summary>
		public void Connect(string filter, bool enableLogs = false)
		{
			lock (ConnectLock) {
				if (this.WasConnectMethodCalled) {
					return;
				}

				this.WasConnectMethodCalled = true;

				this.collectLogs = enableLogs;
				this.cancellationTokenSource = new CancellationTokenSource();
				var cancellationToken = this.cancellationTokenSource.Token;
				var receiveDataTask = Task.Run(() => this.StartReadDataFromServer(cancellationToken, filter), cancellationToken);
				var processDataTask = Task.Run(async () => await this.StartProcessData(cancellationToken), cancellationToken);
			}
		}

		/// <summary>
        /// <see cref="IOgnClient.Disconnect"/>
        /// </summary>
        public void Disconnect()
        {
	        lock (DisconnectLock) {
		        this.WasConnectMethodCalled = false;
		        this.cancellationTokenSource?.Cancel();
		        this.ConnectionErrorEventHandler?.Invoke(this, new EventArgs());
	        }
        }
    }
}
