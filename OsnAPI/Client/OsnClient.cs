using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Logger;
using OsnAPI.Aircraft;
using RestSharp;
using RestSharp.Authenticators;

namespace OsnAPI.Client
{
    /// <summary>
    /// Klient pro stahování dat ze OpenSky serverů.
    /// </summary>
    public class OsnClient : IOsnClient
    {
        /// <summary>
        /// Uživatelské jméno k účtu OpenSkyNetwork.
        /// </summary>
        private string username;

        /// <summary>
        /// Heslo k účtu OpenSkyNetwork.
        /// </summary>
        private string password;

        /// <summary>
        /// Zdroj tokenů pro rušení Tasků.
        /// </summary>
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Atribut určující zda je je služba OpenSky dostupná.
        /// </summary>
        private bool isConnectionOk = false;

        /// <summary>
        /// Instance třídy Logger pro zapisování a čtení logů.
        /// </summary>
        private SimpleJsonLogger<OsnAircraftEventArgs> logger;

        /// <summary>
        /// Určuje zda se budou zapisovat logy.
        /// </summary>
        private bool enabledLogs = false;

        /// <inheritdoc/>
        public event EventHandler ConnectionLost;

        /// <inheritdoc/>
        public event EventHandler<OsnAircraftEventArgs> AircraftDataReceived;

        /// <inheritdoc/>
        public event EventHandler ConnectionRestored;

        /// <summary>
        /// Rozdíl mezi lokálním časem a časem na serveru. (sekundy)
        /// </summary>
        public long CurrentSynchronizationOffset { get; set; }

		/// <summary>
		/// Lock pro synchronizaci.
		/// </summary>
		private readonly object SynchronizationLock = new object();

		/// <summary>
		/// Příznak určující zda byla zavolána metoda Connect.
		/// </summary>
		public bool WasConnectionMethodCalled { get; private set; } = false;

        /// <summary>
        /// Inicializuje instanci třídy OsnClient
        /// </summary>
        /// <param name="username">uživatelské jméno do šlužby OpenSkyNetwork</param>
        /// <param name="password">heslo do služby OpenSkyNetwork</param>
        public OsnClient(string username, string password)
        {
            this.username = username;
            this.password = password;

            //Cesta k místu pro logování.
            var logPath = OsnClientConstants.RootPath;
            this.logger = new SimpleJsonLogger<OsnAircraftEventArgs>(logPath);
        }

        /// <inheritdoc/>
        public void Connect(float north, float west, float south, float east, bool enabledLogs = false)
        {
	        if (this.WasConnectionMethodCalled) {
		        return;
	        }

	        this.WasConnectionMethodCalled = true;
	        this.enabledLogs = enabledLogs;

	        this.cancellationTokenSource = new CancellationTokenSource();
	        var cancellationToken = this.cancellationTokenSource.Token;

	        var restClient = this.InitializeRestClient();
	        restClient.Timeout = OsnClientConstants.REQUEST_TIMEOUT;
	        var request = new RestRequest(OsnClientConstants.RESOURCES_URI, Method.GET);
	        request.AddParameter("lamin", south.ToString(CultureInfo.InvariantCulture));
	        request.AddParameter("lomin", west.ToString(CultureInfo.InvariantCulture));
	        request.AddParameter("lamax", north.ToString(CultureInfo.InvariantCulture));
	        request.AddParameter("lomax", east.ToString(CultureInfo.InvariantCulture));

	        Task.Run(async () => {

		        while (!cancellationToken.IsCancellationRequested) {
			        this.ReceiveData(restClient, request);
			        await Task.Delay(OsnClientConstants.MONITORED_AIRCRAFT_UPDATE_INTERVAL);
		        }
	        }, cancellationToken);
        }

        /// <inheritdoc/>
        public void Disconnect()
        {
            this.cancellationTokenSource?.Cancel();
            this.WasConnectionMethodCalled = false;
            this.ConnectionLost?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Zažádá OSN server o data a v případě odpovědi data přijme.
        /// </summary>
        /// <param name="restClient">rest klient pro stahování dat ze serveru ve formátu JSON.</param>
        /// <param name="request">rest žádost na server.</param>
        private void ReceiveData(RestClient restClient, RestRequest request)
        {
	        var response = restClient.Execute<JsonStateVectorObject>(request);
	        if (response.Data != null && response.ErrorException == null) {
		        //Stažení dat proběhlo v pořádku.    
		        if (response.Data.States != null) {
			        this.PassData(this.DeserializeData(response.Data));

			        if (!this.isConnectionOk) {
				        //Vyvolá událost indikující obnovení spojení.
				        this.ConnectionRestored?.Invoke(this, new EventArgs());
				        this.isConnectionOk = true;
			        }
		        }
	        } else {
		        //Došlo k chybě při stahování dat.
		        if (this.isConnectionOk) {
			        //Vyvolá událost indikující ztrátu spojení
			        this.ConnectionLost?.Invoke(this, new EventArgs());
			        this.isConnectionOk = false;
		        }
	        }
        }

        /// <summary>
        /// Vyvolá událost a pošle uvnitř parametru informace o vzdušné situace ke klientovi.
        /// Pokud je povoleno logování, informaci zaloguje.
        /// </summary>
        /// <param name="aircraftsArgs">Argumenty obsahující informace o vzdušné situaci.</param>
        private void PassData(OsnAircraftEventArgs aircraftsArgs)
        {
	        long synchronizationDiffHelp;
	        lock (this.SynchronizationLock) {
		        //Rozdíl mezi lokálním časem a časem serveru.
		        synchronizationDiffHelp = this.CurrentSynchronizationOffset;
	        }

	        //úprava času v datech.
	        aircraftsArgs.Timestamp += synchronizationDiffHelp;

	        foreach (var aircraft in aircraftsArgs.ListOfAircraft) {
		        aircraft.StateVector.LastContact += synchronizationDiffHelp;
	        }

	        if (this.enabledLogs) {
		        //Logování je povoleno => zaloguje.
		        var aircraftLog = new List<OsnAircraft>();
		        foreach (var aircraftToCopy in aircraftsArgs.ListOfAircraft) {
			        var aircraftToLog = aircraftToCopy.Clone() as OsnAircraft;
			        if (aircraftToLog != null) {
				        aircraftLog.Add(aircraftToLog);
			        }
		        }

		        this.logger.Write(aircraftsArgs);
	        }

	        this.AircraftDataReceived?.Invoke(this, aircraftsArgs);
        }

		/// <summary>
		/// Deserializuje data z formátu json.
		/// </summary>
		/// <returns>Argumenty události s informacemi o vzdušné situaci získané z OSN serveru.</returns>
		private OsnAircraftEventArgs DeserializeData(JsonStateVectorObject jsonObject)
		{
			var deserializedAircraftList = new List<OsnAircraft>();
			foreach (var stateVectorItem in jsonObject.States) {
				var stateVector = new OsnAircraftStateVector(stateVectorItem);
				deserializedAircraftList.Add(new OsnAircraft(stateVector));
			}

			return new OsnAircraftEventArgs(jsonObject.Time, deserializedAircraftList);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public TimeSpan StartReplay(string id)
        {
            var offset = this.logger.StartReplaying("Replay" + id +".txt");
            this.logger.LogItemReplayed += this.Logger_LogItemReplayed;
			this.logger.ReplayIsOver += this.Logger_ReplayIsOver;

			return offset;
        }

		/// <summary>
		/// Handler získá data z replay, nahradí čas zalogování aktuálním časem (předstírá, že se jedná o aktuální data) a vyvolá událost.
		/// </summary>
		private void Logger_LogItemReplayed(object sender, JsonLogEventArgs<OsnAircraftEventArgs> e)
		{
			var totalSeconds = (int) (DateTime.UtcNow - e.LogDate).TotalSeconds;
			e.Data.Timestamp += totalSeconds;
			e.Data.ListOfAircraft.ForEach(x => {
				x.StateVector.LastContact += totalSeconds;
			});
			this.AircraftDataReceived?.Invoke(this, e.Data);

			if (!this.isConnectionOk) {
				this.isConnectionOk = true;
				this.ConnectionRestored?.Invoke(this, new EventArgs());
			}
		}

		/// <summary>
		/// Handler vyvolá událost o ztrátě spojení. 
		/// </summary>
		private void Logger_ReplayIsOver(object sender, EventArgs e)
		{
			this.ConnectionLost?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Inicializuje Rest klienta. 
		/// </summary>
		/// <returns>ukazatele na klienta</returns>
		private RestClient InitializeRestClient()
        {
            //Zadává do klienta URL a uživatelské údaje služby OpenSky
            var restClient = new RestClient(OsnClientConstants.URL);
            restClient.Authenticator = new HttpBasicAuthenticator(this.username, this.password);
            return restClient;
        }
    }
}
