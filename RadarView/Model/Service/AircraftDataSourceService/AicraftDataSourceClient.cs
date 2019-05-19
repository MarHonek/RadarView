using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using GuerrillaNtp;
using OgnAPI;
using OgnAPI.Client;
using OsnAPI.Client;
using RadarView.Model.Entities.Aviation;
using RadarView.Model.Entities.AviationData;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Service.SessionContext;
using RadarView.Properties;
using RadarView.Utils;
using AircraftType = RadarView.Model.Entities.Aviation.AircraftType;
using Timer = System.Timers.Timer;

namespace RadarView.Model.Service.AircraftDataSourceService
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
    public class AircraftDataSourceClient : IAircraftDataSourceClient
    {
	    private readonly ISessionContext _sessionContext;

		/// <summary>
		/// Globální časová známka Open Sky Network. Definuje poslední čas aktualizace všech dat
		/// </summary>
		private long osnLastGlobalTimestamp = 0;

		/// <summary>
		/// Uživatelské jméno pro službu Open Sky Network
		/// </summary>
		private readonly string OsnUserName = Settings.Default.OsnUserName;

        /// <summary>
        /// Heslo pro službu Open sky Network
        /// </summary>
        private readonly string OsnPassword = Settings.Default.OsnPassword;

        /// <summary>
        /// Určuje zda mají být zaznamenávány logy z datových zdrojů.
        /// </summary>
        private readonly bool EnabledDataSourceLogs = Settings.Default.DataSourceLogsEnabled;

        /// <summary>
        /// Klient služby Open Sky Network
        /// </summary>
        private IOsnClient osnClient;

        /// <summary>
        /// Klient služby Open Glider Network
        /// </summary>
        private IOgnClient ognClient;

        /// <summary>
		/// <inheritdoc/>
		/// </summary>
        public event EventHandler<AircraftDataSourceEventArgs> AircraftReceived;

        /// <summary>
        /// Informace o datových zdrojích.
        /// </summary
		private AircraftDataSourceWrapper dataSourceInfo;


        /// <summary>
        /// Konstruktor incializuje API datových zdrojů.
        /// </summary>
        public AircraftDataSourceClient(ISessionContext sessionContext)
        {
	        this._sessionContext = sessionContext;
		}

        /// <summary>
        /// Rozdíl mezi časem logů a aktuálním časem (sekundy).
        /// </summary>
        private static int _replayOffset = int.MaxValue;
        public int ReplayOffset {
	        get { return Interlocked.CompareExchange(ref _replayOffset, 0, 0); }
	        set {
		        var offset = value;
		        if (value < _replayOffset) {
			        Interlocked.Exchange(ref _replayOffset, offset);
		        }
	        }
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Initialize()
        {
	        this.osnClient = new OsnClient(this.OsnUserName, this.OsnPassword);
	        this.ognClient = OgnClientFactory.CreateClient();

	        //Příjem dat.
	        this.osnClient.AircraftDataReceived += this.OsnClient_AircraftDataReceivedAsync;
	        this.ognClient.AircraftBeaconProcessedAsyncEventHandler += this.OgnClient_AircraftBeaconProcessedAsyncEventHandler;


	        //Přerušené síťové spojení.
	        this.osnClient.ConnectionLost += this.OsnClient_ConnectionLost;
	        this.ognClient.ConnectionErrorEventHandler += this.OgnClient_ConnectionErrorEventHandler;

	        //Obnovene síťové spojení.
	        this.osnClient.ConnectionRestored += this.OsnClient_ConnectionRestored;
	        this.ognClient.ConnectionRestoredEventHandler += this.OgnClient_ConnectionRestoredEventHandler;

	        this.dataSourceInfo = new AircraftDataSourceWrapper();
	        this._sessionContext.AircraftDataSourceWrapper = this.dataSourceInfo;

	        var ntpSync = Settings.Default.NtpSynchronizationEnabled;
	        if (ntpSync) {
		        var ntpSyncTimer = new Timer();
		        ntpSyncTimer.Interval = Settings.Default.NtpSynchronizationIntervalSeconds * 1000;
				ntpSyncTimer.Elapsed += this.NtpSyncTimer_Elapsed;
				ntpSyncTimer.Start();
				this.NtpSyncTimer_Elapsed(this, null);
	        }
        }

		private void NtpSyncTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			TimeSpan offset;
			try {
				using (var ntp = new NtpClient(Dns.GetHostAddresses(Settings.Default.NtpSynchronizationServerUrl)[0]))
					offset = -ntp.GetCorrectionOffset();

				this.ognClient.CurrentSynchronizationOffset = (long)offset.TotalSeconds;
				this.osnClient.CurrentSynchronizationOffset = (long)offset.TotalSeconds;
			} catch (Exception) {
			}
		}

		/// <summary>
		/// Handler obsluhuje data ze serveru Open sky network.
		/// </summary>
		private void OsnClient_AircraftDataReceivedAsync(object sender, OsnAircraftEventArgs e)
        {
			//Ověří zda je globální časová známka větší než přechozí. Pokud není, data zahodí.
			if (this.osnLastGlobalTimestamp != e.Timestamp)
			{
                foreach (var osnAircraft in e.ListOfAircraft)
                {					
                    var stateVector = osnAircraft.StateVector;
                    var aircraft = new AircraftRawDataBuilder()
                        .Address(stateVector.ModeSCode.ToUpper())
                        .CallSign(stateVector.Callsign?.Trim())
                        .Latitude(stateVector.Latitude)
                        .Longitude(stateVector.Longitude)
                        .Timestamp(stateVector.LastContact)
                        .Altitude(stateVector.GeoAltitude ?? stateVector.BaroAltitude)
                        .GroundSpeed(stateVector.Velocity)
                        .Track(stateVector.Heading)
                        .VerticalSpeed(stateVector.VerticalRate)
                        .OnGround(stateVector.OnGround)
                        .AircraftType(AircraftType.Other)
                        .DataSource(AircraftDataSourceEnum.OSN)
                        .Squawk(stateVector.Squawk)
                        .OriginCountry(stateVector.OriginCountry)
                        .Build();
						
                        this.AircraftReceived?.Invoke(this, new AircraftDataSourceEventArgs(aircraft));
				}

				this.osnLastGlobalTimestamp = e.Timestamp;
				this._sessionContext.AircraftDataSourceWrapper.UpdateTimeOffset(AircraftDataSourceEnum.OSN, (int)this.osnClient.CurrentSynchronizationOffset);
			}
        }

		/// <summary>
		/// Handler obsluhuje polohová data získaná ze serveru Open glider network.
		/// </summary>
		private void OgnClient_AircraftBeaconProcessedAsyncEventHandler(object sender, OgnEventArgs eventArgs)
		{
			if (!string.IsNullOrEmpty(eventArgs.Beacon.CallSign)) {
				var beacon = eventArgs.Beacon;
				var descriptor = eventArgs.Descriptor;

				var regNumber = descriptor?.RegNumber;
				var cn = descriptor?.CN;
				var model = descriptor?.Model;
				var registration = descriptor?.RegNumber;


				//Převede jednotky:
				//výšku na metry
				var altitude = beacon.Altitude;
				altitude = MathExtension.FeetToMeters(altitude);

				//Rychlost na metry za sekundu
				var groundSpeed = beacon.GroundSpeed;
				if (groundSpeed.HasValue) {
					groundSpeed = MathExtension.KnotsToMetersPerSecond(groundSpeed.Value);
				}

				//Rychlost na metry za sekundu.
				var climbRate = beacon.ClimbRate;
				if (climbRate.HasValue) {
					climbRate = MathExtension.FeetPerMinuteToMetersPerSecond(climbRate.Value);
				}

				var aircraft = new AircraftRawDataBuilder()
					.Address(beacon.CallSign)
					.CompetitionName(cn)
					.CallSign(regNumber ?? beacon.Address)
					.Latitude((float?)beacon.Latitude)
					.Longitude((float?)beacon.Longitude)
					.Timestamp(beacon.Timestamp)
					.GroundSpeed(groundSpeed)
					.Altitude(altitude)
					.Track(beacon.Track)
					.VerticalSpeed(climbRate)
					.AircraftType(this.GetAircraftTypeFromOgnSource(beacon.AircraftType))
					.DataSource(AircraftDataSourceEnum.OGN)
					.Model(model)
					.Registration(registration)
					.Build();

				this.AircraftReceived?.Invoke(this, new AircraftDataSourceEventArgs(aircraft));
				this._sessionContext.AircraftDataSourceWrapper.UpdateTimeOffset(AircraftDataSourceEnum.OGN, (int)this.ognClient.CurrentSynchronizationOffset);
			}
		}

		#region Connection
		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Connect(BoundingBox monitoredArea)
		{
			if (Settings.Default.OsnEnabled) {
				this.ConnectToOsn(monitoredArea);
			}

			if (Settings.Default.OgnEnabled) {
				this.ConnectToOgn(monitoredArea);
			}
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void ConnectToOgn(BoundingBox monitoredArea)
		{
			//Rozšíří oblast z důvodu nepřesnosti měření a také, aby se letadla náhodou nesekla na kraji oblasti.
			var extendedArea = monitoredArea.ExtendBox(0.1f);

			//Připojí klienta k serveru s filtrem, který filtruje letadla podle zadane oblasti.
			this.ognClient.Connect("filter a/"
			                       + extendedArea.North.ToString(CultureInfo.InvariantCulture) + "/"
			                       + extendedArea.West.ToString(CultureInfo.InvariantCulture) + "/"
			                       + extendedArea.South.ToString(CultureInfo.InvariantCulture) + "/"
			                       + extendedArea.East.ToString(CultureInfo.InvariantCulture), this.EnabledDataSourceLogs);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void ConnectToOsn(BoundingBox boundingBox)
		{
			//Rozšíří oblast z důvodu nepřesnosti měření a také, aby se letadla náhodou nesekla na kraji oblasti.
			var extendedArea = boundingBox.ExtendBox(0.1f);
			this.osnClient.Connect(extendedArea.North, extendedArea.West, extendedArea.South, extendedArea.East, this.EnabledDataSourceLogs);
		}

		#endregion

		#region Disconnection

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Disconnect()
		{
			this.DisconnectFromOgn();
			this.DisconnectFromOsn();
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void DisconnectFromOgn()
		{
			this.ognClient?.Disconnect();
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void DisconnectFromOsn()
		{
			this.osnClient?.Disconnect();
		}
		#endregion

		#region Replay

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void StartOgnReplay(string fileName)
		{
			this.ReplayOffset = (int)this.ognClient.StartReplay(fileName).TotalSeconds;
		}

		/// <summary>
		/// Spustí replay z OSN zdroje.
		/// </summary>
		/// <param name="fileName">název souboru.</param>
		public void StartOsnReplay(string fileName)
		{
			this.ReplayOffset = (int)this.osnClient.StartReplay(fileName).TotalSeconds;
		}
		#endregion

		#region Connectivity change
		private void OgnClient_ConnectionRestoredEventHandler(object sender, EventArgs e)
		{
			this._sessionContext.AircraftDataSourceWrapper.SetAvailability(AircraftDataSourceEnum.OGN, true);
		}

		private void OsnClient_ConnectionRestored(object sender, EventArgs e)
		{
			this._sessionContext.AircraftDataSourceWrapper.SetAvailability(AircraftDataSourceEnum.OSN, true);
		}

		private void OgnClient_ConnectionErrorEventHandler(object sender, EventArgs e)
		{
			this._sessionContext.AircraftDataSourceWrapper.SetAvailability(AircraftDataSourceEnum.OGN, false);
		}

		private void OsnClient_ConnectionLost(object sender, EventArgs e)
		{
			this._sessionContext.AircraftDataSourceWrapper.SetAvailability(AircraftDataSourceEnum.OSN, false);
		}
		#endregion

		/// <summary>
		/// Převede typ letadla z Open Glider Network na typ pro AircraftRawData
		/// </summary>
		/// <param name="ognAircraftType">OGN typy letadel</param>
		/// <returns></returns>
		private AircraftType GetAircraftTypeFromOgnSource(OgnAPI.Beacon.AircraftType ognAircraftType)
        {
            switch (ognAircraftType)
            {
                case OgnAPI.Beacon.AircraftType.Glider:
                    return AircraftType.Glider;
                default:
                    return AircraftType.Other;
            }
        }
    }

    /// <summary>
    /// Argument události AircraftReceived obsahující surová data o letadle.
    /// </summary>
    public class AircraftDataSourceEventArgs : EventArgs
    {
		/// <summary>
		/// Surová data přijatá z datových zdrojů.
		/// </summary>
        public AircraftRawData RawData { get; set; }

        public AircraftDataSourceEventArgs(AircraftRawData rawData)
        {
            this.RawData = rawData;
        }
    }
}
