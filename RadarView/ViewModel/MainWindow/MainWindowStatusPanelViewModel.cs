using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using RadarView.Model.Entities.AviationData;
using RadarView.Properties;
using RadarView.ViewModel.Base;

namespace RadarView.ViewModel.MainWindow
{
	/// <summary>
	/// ViewModel pro status panel.
	/// </summary>
	partial class MainWindowViewModel
	{

		/// <summary>
		/// Časovač pro zobrazení aktuální času.
		/// </summary>
		private DispatcherTimer secondTimer;


		/// <summary>
		/// Maximální zobrazovaná výška.
		/// </summary>
		private int _maxAltitude;
		public int MaxAltitude {
			get { return this._maxAltitude; }
			set
			{
				this._maxAltitude = value;
				this.OnPropertyChanged(nameof(this.MaxAltitude));
			}
		}

		/// <summary>
		/// Aktuální čas.
		/// </summary>
		private DateTime? _currentTime;
		public DateTime? CurrentTime {
			get { return this._currentTime; }
			set
			{
				this._currentTime = value;
				this.OnPropertyChanged(nameof(this.CurrentTime));
			}
		}

		/// <summary>
		/// Čas poslední aktualizace polohohy letadel.
		/// </summary>
		private DateTime? _renderTime;
		public DateTime? RenderTime
		{
			get { return this._renderTime; }
			set
			{
				this._renderTime = value;
				this.OnPropertyChanged(nameof(this.RenderTime));
			}
		}

		/// <summary>
		/// Status obsahující dostupné zdroje polohových dat letadel.
		/// </summary>
		private string _dataSourceStatus;
		public string DataSourceStatus
		{
			get { return this._dataSourceStatus; }
			set
			{
				this._dataSourceStatus = value;
				this.OnPropertyChanged(nameof(this.DataSourceStatus));
			}
		}

		/// <summary>
		/// Příznak určující zda je spustěno zpětné přehrávání logů.
		/// </summary>
		private bool _isReplaying;
		public bool IsReplaying
		{
			get { return this._isReplaying; }
			set
			{
				this._isReplaying = value;
				this.OnPropertyChanged(nameof(this.IsReplaying));
			}
		}

		/// <summary>
		/// Command pro nastavení maximální zobrazované výšky.
		/// </summary>
		private ICommand _selectMaxAltitudeCommand;
		public ICommand SelectMaxAltitudeCommand {
			get {
				if (this._selectMaxAltitudeCommand == null) {
					this._selectMaxAltitudeCommand = new RelayCommand(x =>
					{
						this.MaxAltitude = Convert.ToInt32(x);
						this._targetComponentsManager.ChangeMaxAltitude(this.MaxAltitude);
					});
				}
				return this._selectMaxAltitudeCommand;
			}
		}

		/// <summary>
		/// Příznak určující zda má být panel s informacemi o synchronizaci viditelný.
		/// </summary>
		private bool _synchronizationPanelVisibility;
		public bool SynchronizationPanelVisibility
		{
			get { return this._synchronizationPanelVisibility; }
			set
			{
				this._synchronizationPanelVisibility = value;
				this.OnPropertyChanged(nameof(this.SynchronizationPanelVisibility));
			}
		}

		/// <summary>
		/// Časový posun služby FlightRadar vůči reálnému času (sekundy).
		/// </summary>
		private int? _frSynchronizationOffset;
		public int? FrSynchronizationOffset
		{
			get { return this._frSynchronizationOffset; }
			set
			{
				this._frSynchronizationOffset = value;
				this.OnPropertyChanged(nameof(this.FrSynchronizationOffset));
			}
		}

		/// <summary>
		/// Časový posun služby Open glider network vůči reálnému času (sekundy).
		/// </summary>
		private int? _ognSynchronizationOffset;
		public int? OgnSynchronizationOffset {
			get { return this._ognSynchronizationOffset; }
			set
			{
				this._ognSynchronizationOffset = value;
				this.OnPropertyChanged(nameof(this.OgnSynchronizationOffset));
			}
		}

		/// <summary>
		/// Časový posun služby OpenSky network vůči reálnému času (sekundy).
		/// </summary>
		private int? _osnSynchronizationOffset;
		public int? OsnSynchronizationOffset {
			get { return this._osnSynchronizationOffset; }
			set
			{
				this._osnSynchronizationOffset = value;
				this.OnPropertyChanged(nameof(this.OsnSynchronizationOffset));
			}
		}

		/// <summary>
		/// Inicializuje timer slouzici pro zobrazeni aktualniho casu v aplikaci.
		/// </summary>
		private void InitSecondTimer()
		{
			this.secondTimer = new DispatcherTimer {
				Interval = new TimeSpan(0, 0, 0, 1)
			};
			this.secondTimer.Tick += this.SecondTimer_Tick;

			var syncTimer = new DispatcherTimer();
			syncTimer.Interval = new TimeSpan(0,0,0,0,50);
			syncTimer.Tick += this.SyncTimer_Tick;
			syncTimer.Start();
	
		}

		/// <summary>
		/// Synchronizuje časovač pro zobrazení aktuálního času, tak že ověřuje zda nedošlo k nové sekundě.
		/// </summary>
		private void SyncTimer_Tick(object sender, EventArgs e)
		{
			var currentTime = DateTime.UtcNow;
			if (currentTime.Millisecond < 100) {
				((DispatcherTimer)sender).Stop();
				this.secondTimer.Start();
			}
		}

		/// <summary>
		/// Aktializuje aktuální čas.
		/// </summary>
		private void SecondTimer_Tick(object sender, EventArgs e)
		{
			this.CurrentTime = DateTime.Now;
		}

		/// <summary>
		/// Inicializuje viewModel pro mapy.
		/// </summary>
		partial void StatusPanelViewModelInit()
		{
			this._rendererController.SamplesReceived += this._rendererController_SamplesReceived;

			this.SynchronizationPanelVisibility = Settings.Default.NtpSynchronizationOffsetPanelIsVisible;
			this.MaxAltitude = Settings.Default.ViewMaxAltitude;
			this._targetComponentsManager.ChangeMaxAltitude(this.MaxAltitude);
			this.InitSecondTimer();
		}

		private void _rendererController_SamplesReceived(object sender, Model.Render.Controller.RendererControllerEventArgs e)
		{
			var dataSourceWrapper = this._sessionContext.AircraftDataSourceWrapper;
			this.OsnSynchronizationOffset = dataSourceWrapper.GetTimeOffset(AircraftDataSourceEnum.OSN);
			this.OgnSynchronizationOffset = dataSourceWrapper.GetTimeOffset(AircraftDataSourceEnum.OGN);

			var renderTime = DateTime.UtcNow;
			if (this.IsReplaying) {
				renderTime = renderTime.AddSeconds(-this._aircraftDataSourceClient.ReplayOffset);
			}

			this.RenderTime = renderTime;
		}

		/// <summary>
		/// Uloží informace ze status panelu do konfigu.
		/// </summary>
		partial void StoreStatusPanelInfoToConfig()
		{
			Settings.Default.ViewMaxAltitude = this.MaxAltitude;
		}

		/// <summary>
		/// Vypíše status konektivity datových zdrojů.
		/// </summary>
		private void DataSourceInfo_AvailabilityChanged(object sender, EventArgs e)
		{
			this.DataSourceStatus = this._sessionContext.AircraftDataSourceWrapper.GetConnectionStatusString();
		}
	}
}
