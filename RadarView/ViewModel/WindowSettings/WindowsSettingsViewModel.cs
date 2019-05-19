using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using RadarView.Model.Entities.AviationData;
using RadarView.Model.Managers.LiveDataManager;
using RadarView.Model.Render.Background.PrecipitationRadar;
using RadarView.Model.Service.AircraftDataSourceService;
using RadarView.Model.Service.SessionContext;
using RadarView.Properties;
using RadarView.ViewModel.Base;

namespace RadarView.ViewModel.WindowSettings
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class WindowsSettingsViewModel : BaseViewModel, IWindowsSettingsViewModel
	{
		/// <summary>
		/// Vychozí hodnota průhlednosti srážkového radaru.
		/// </summary>
		private int defaultPrecipitationRadarOpacity;

		private readonly IBackgroundPrecipitationRadar _precipitationRadar;

		private readonly IAircraftDataSourceClient _dataSourceClient;

		private readonly ISessionContext _sessionContext;

		private readonly IAircraftLiveDataManager _liveDataManager;

		/// <summary>
		/// Příznak určující, zda má při zavření okna vráceny revertovány hodnoty.
		/// </summary>
		private bool closeWithRevert;

		/// <summary>
		/// Viditelnost radarových snímků počasí.
		/// </summary>
		private int _precipitationRadarOpacity;
		public int PrecipitationRadarOpacity
		{
			get { return this._precipitationRadarOpacity; }
			set
			{
				if (Settings.Default.PrecipitationRadarIsVisible)  {
					this._precipitationRadar.ImageOpacity = value;	
				}
				this._precipitationRadarOpacity = value;
				this.OnPropertyChanged(nameof(this.PrecipitationRadarOpacity));
			}
		}

		/// <summary>
		/// Příznak určující, zda je povoleno měnit hodnoty průhledonosti.
		/// </summary>
		private bool _precipitationRadarEnabled;
		public bool PrecipitationRadarEnabled
		{
			get { return this._precipitationRadarEnabled; }
			set
			{
				this._precipitationRadarEnabled = value;
				this.OnPropertyChanged(nameof(this.PrecipitationRadarEnabled));
			}
		}

		/// <summary>
		/// Příznak určující zda je zapnutý datový zdroj OGN.
		/// </summary>
		private bool _isOgnEnabled;
		public bool IsOgnEnabled
		{
			get { return this._isOgnEnabled; }
			set
			{
				this._isOgnEnabled = value;
				this.OnPropertyChanged(nameof(this.IsOgnEnabled));
			}
		}

		/// <summary>
		/// Příznak určující zda je zapnutý datový zdroj OSN.
		/// </summary>
		private bool _isOsnEnabled;
		public bool IsOsnEnabled {
			get { return this._isOsnEnabled; }
			set {
				this._isOsnEnabled = value;
				this.OnPropertyChanged(nameof(this.IsOsnEnabled));
			}
		}

		/// <summary>
		/// Command pro otevření okna s nastavením.
		/// </summary>
		private ICommand _saveSettingsCommand;
		public ICommand SaveSettingsCommand {
			get {
				if (this._saveSettingsCommand == null) {
					this._saveSettingsCommand = new RelayCommand(x => {
						this.SaveSettings();
						this.closeWithRevert = false;
						((Window)x).Close();
					});
				}
				return this._saveSettingsCommand;
			}
		}

		/// <summary>
		/// Command pro zrušení změn a zavření okna s nastavením.
		/// </summary>
		private ICommand _cancelCommand;
		public ICommand CancelCommand {
			get 
			{
				if (this._cancelCommand == null) {
					this._cancelCommand = new RelayCommand(x => {
						this.closeWithRevert = true;
						((Window)x).Close();
					});
				}
				return this._cancelCommand;
			}
		}


		/// <summary>
		/// Command pro vyvolaný po načtění okna.
		/// </summary>
		private ICommand _loadedCommand;

		public ICommand LoadedCommand
		{
			get
			{
				if (this._loadedCommand == null) {
					this._loadedCommand = new RelayCommand(x => {
						this.PrecipitationRadarEnabled = Settings.Default.PrecipitationRadarIsVisible;
						this.PrecipitationRadarOpacity = Settings.Default.PrecipitationRadarOpacity;
						this.defaultPrecipitationRadarOpacity = this.PrecipitationRadarOpacity;


						this.IsOsnEnabled = Settings.Default.OsnEnabled;
						this.IsOgnEnabled = Settings.Default.OgnEnabled;
					});
				}

				return this._loadedCommand;
			}
		}


		/// <summary>
		/// Command pro zavření okna s nastavením křížkem v pravém rohu.
		/// </summary>
		private ICommand _closeCommand;
		public ICommand CloseCommand {
			get {
				if (this._closeCommand == null) {
					this._closeCommand = new RelayCommand(x => {
						if (this.closeWithRevert) {
							this.RevertChanges();
						}
					});
				}
				return this._closeCommand;
			}
		}

		public WindowsSettingsViewModel(IBackgroundPrecipitationRadar precipitationRadar, IAircraftDataSourceClient dataSourceClient, ISessionContext sessionContext, IAircraftLiveDataManager liveDataManager)
		{
			this._precipitationRadar = precipitationRadar;
			this._dataSourceClient = dataSourceClient;
			this._sessionContext = sessionContext;
			this._liveDataManager = liveDataManager;
		}



		/// <summary>
		/// Vrátí zpět změny.
		/// </summary>
		private void RevertChanges()
		{
			this._precipitationRadar.ImageOpacity = this.defaultPrecipitationRadarOpacity;
			this.PrecipitationRadarOpacity = this.defaultPrecipitationRadarOpacity;
		}

		/// <summary>
		/// Uloží nastavení.
		/// </summary>
		private void SaveSettings()
		{
			Settings.Default.PrecipitationRadarOpacity = this.PrecipitationRadarOpacity;
			Settings.Default.OgnEnabled = this.IsOgnEnabled;
			Settings.Default.OsnEnabled = this.IsOsnEnabled;

			var monitoredArea = this._sessionContext.MonitoredArea;
			
			//Zapnutí/vypnutí OSN.
			if (this.IsOsnEnabled) {
				this._dataSourceClient.ConnectToOsn(monitoredArea);
			} else {
				this._dataSourceClient.DisconnectFromOsn();
				this._liveDataManager.RemoveAircraftFromDataSource(AircraftDataSourceEnum.OSN);
			}

			//Zapnutí/vypnutí OGN.
			if (this.IsOgnEnabled) {
				this._dataSourceClient.ConnectToOgn(monitoredArea);
			} else {
				this._dataSourceClient.DisconnectFromOgn();
				this._liveDataManager.RemoveAircraftFromDataSource(AircraftDataSourceEnum.OGN);
			}

			Settings.Default.Save();
		}
	}
}
