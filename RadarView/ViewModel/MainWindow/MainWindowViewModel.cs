using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using MathNet.Numerics;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using RadarView.Model.DataService.AviationData.AirportDataService;
using RadarView.Model.DataService.Weather;
using RadarView.Model.DataService.Weather.PrecipitationRadarDataService;
using RadarView.Model.Entities.AviationData;
using RadarView.Model.Entities.AviationData.Airspace;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Background.Airport;
using RadarView.Model.Render.Background.Map;
using RadarView.Model.Render.Background.PrecipitationRadar;
using RadarView.Model.Render.Controller;
using RadarView.Model.Render.MeasuringLine;
using RadarView.Model.Render.Targets;
using RadarView.Model.Service.AircraftDataSourceService;
using RadarView.Model.Service.SessionContext;
using RadarView.Model.Service.UdpCommunicationClient;
using RadarView.Properties;
using RadarView.View;
using RadarView.ViewModel.Base;
using RadarView.ViewModel.Converters;
using RadarView.ViewModel.WindowAirportSelection;
using RadarView.ViewModel.WindowSettings;
using BoundingBox = RadarView.Model.Entities.Geographic.BoundingBox;
using Window = System.Windows.Window;

namespace RadarView.ViewModel.MainWindow
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public partial class MainWindowViewModel : BaseViewModel, IMainWindowViewModel
	{
		private readonly IAirportDataService _airportDataService;

		private readonly ISessionContext _sessionContext;

		private readonly IAircraftDataSourceClient _aircraftDataSourceClient;

		private readonly IWeatherDataService _weatherDataService;

		private readonly IPrecipitationRadarDataService _precipitationRadarDataService;

		private readonly IBackgroundPrecipitationRadar _precipitationRadar;

		private readonly IBackgroundMapManager _mapManager;

		private readonly IWindowsSettingsViewModel _settingsViewModel;

		private readonly IWindowAirportSelectionViewModel _airportSelectionViewModel;

		private readonly IMeasuringLine _measuringLine;

		private readonly IRendererController _rendererController;

		private readonly IUdpCommunicationClient _udpClient;

		private readonly ITargetComponentsManager _targetComponentsManager;

		private readonly IBackgroundAirportManager _backgroundAirportManager;

		public MainWindowViewModel(IAirportDataService airportDataService, 
								  ISessionContext sessionContext,
								  IAircraftDataSourceClient aircraftDataSourceClient,
								  IWeatherDataService weatherDataService,
								  IPrecipitationRadarDataService precipitationRadarDataService,
								  IBackgroundPrecipitationRadar precipitationRadar,
								  IBackgroundMapManager mapManager,
								  IWindowsSettingsViewModel settingsViewModel,
								  IWindowAirportSelectionViewModel airportSelectionViewModel,
								  IMeasuringLine measuringLine,
								  IRendererController rendererController,
								  IUdpCommunicationClient udpClient,
								  ITargetComponentsManager targetComponentsManager,
								  IBackgroundAirportManager backgroundAirportManager)
		{
			this._airportDataService = airportDataService;
			this._sessionContext = sessionContext;
			this._aircraftDataSourceClient = aircraftDataSourceClient;
			this._weatherDataService = weatherDataService;
			this._precipitationRadarDataService = precipitationRadarDataService;
			this._precipitationRadar = precipitationRadar;
			this._mapManager = mapManager;
			this._airportSelectionViewModel = airportSelectionViewModel;
			this._settingsViewModel = settingsViewModel;
			this._measuringLine = measuringLine;
			this._rendererController = rendererController;
			this._udpClient = udpClient;
			this._targetComponentsManager = targetComponentsManager;
			this._backgroundAirportManager = backgroundAirportManager;
		}

		/// <summary>
		/// Příznak určující, zda jsou zobrazeny letiště (neplatí pro uživatelem nastavené letiště).
		/// </summary>
		private bool _airportsVisibility;
		public bool AirportsVisibility
		{
			get { return this._airportsVisibility; }
			set
			{
				this._airportsVisibility = value;
				this._backgroundAirportManager.SetVisibilityExceptSelectedAirport(value);
				this.OnPropertyChanged(nameof(this.AirportsVisibility));
			}
		}

		/// <summary>
		/// Command pro provedení procedur při zavření okna.
		/// </summary>
		private ICommand _closingWindowCommand;
		public ICommand ClosingWindowCommand {
			get {
				if (this._closingWindowCommand == null) {
					this._closingWindowCommand = new RelayCommand(x =>
					{
						this.StoreToConfig();
					});
				}
				return this._closingWindowCommand;
			}
		}

		/// <summary>
		/// Command pro zavření okna.
		/// </summary>
		private ICommand _buttonClosingWindowCommand;
		public ICommand ButtonClosingWindowCommand {
			get {
				if (this._buttonClosingWindowCommand == null) {
					this._buttonClosingWindowCommand = new RelayCommand(x => {
						((Window)x).Close();
					});
				}
				return this._buttonClosingWindowCommand;
			}
		}

		/// <summary>
		/// Command pro otevření okna s nastavením.
		/// </summary>
		private ICommand _showWindowSettingsCommand;
		public ICommand ShowWindowSettingsCommand {
			get {
				if (this._showWindowSettingsCommand == null) {
					this._showWindowSettingsCommand = new RelayCommand(x => {
						new SettingsWindow(this._settingsViewModel).Show();
					});
				}
				return this._showWindowSettingsCommand;
			}
		}

		/// <summary>
		/// Command pro otevření okna s informacemi o aplikaci.
		/// </summary>
		private ICommand _showWindowAboutCommand;
		public ICommand ShowWindowAboutCommand {
			get {
				if (this._showWindowAboutCommand == null) {
					this._showWindowAboutCommand = new RelayCommand(x => {
						new AboutBox().ShowDialog();
					});
				}
				return this._showWindowAboutCommand;
			}
		}

		/// <summary>
		/// Command pro otevření okna s výběrem letiště.
		/// </summary>
		private ICommand _showWindowAirportSelectionCommand;
		public ICommand ShowWindowAirportSelectionCommand {
			get {
				if (this._showWindowAirportSelectionCommand == null) {
					this._showWindowAirportSelectionCommand = new RelayCommand(x => {
							new View.WindowAirportSelection(this._airportSelectionViewModel).ShowDialog();
						});
				}
				return this._showWindowAirportSelectionCommand; ;
			}
		}

		/// <summary>
		/// Kurz měřící čáry (stupně).
		/// </summary>
		private int? _measuringLineCourse;
		public int? MeasuringLineCourse
		{
			get { return this._measuringLineCourse; }
			set
			{
				this._measuringLineCourse = value;
				this.OnPropertyChanged(nameof(this.MeasuringLineCourse));
				this.OnPropertyChanged(nameof(this.MeasuringLineInfo));
			}
		}

		/// <summary>
		/// Délka měřící čáry (metry).
		/// </summary>
		private int? _measuringLineLength;
		public int? MeasuringLineLength
		{
			get { return this._measuringLineLength; }
			set
			{
				this._measuringLineLength = value;
				this.OnPropertyChanged(nameof(this.MeasuringLineLength));
				this.OnPropertyChanged(nameof(this.MeasuringLineInfo));
			}
		}

		/// <summary>
		/// Informace z meřící čáry.
		/// </summary>
		public string MeasuringLineInfo
		{
			get
			{
				if (this.MeasuringLineCourse.HasValue && this.MeasuringLineLength.HasValue) {
					var kmLength = this._measuringLineLength / 1000.0;
					return "Vzdálenost: " + Math.Round(kmLength.Value, 2) + " km, Azimut: " + this.MeasuringLineCourse + " °";
				} else {
					return "";
				}
			}
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		private void _measuringLine_StateChanged(object sender, MeasuringLineEventArgs e)
		{
			this.MeasuringLineCourse = e.Course;
			this.MeasuringLineLength = e.Length;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Initialize()
		{
			this._measuringLine.StateChanged += this._measuringLine_StateChanged;

			this.WeatherViewModelInit();
			this.MapViewModelInit();
			this.StatusPanelViewModelInit();

			this._aircraftDataSourceClient.Initialize();
			this._sessionContext.AircraftDataSourceWrapper.AvailabilityChanged += this.DataSourceInfo_AvailabilityChanged;

			//Zjistí jestli má být spušten replay.
			var replayWasStarted = this.StartReplay();

			if (!replayWasStarted) {
				//Uživatel nechce spustit replay. Zapne reálna data.
				this.ConnectToDataClient();
			}

			this.AirportsVisibility = Settings.Default.AirportsAreVisible;

			this._udpClient.Initialize(Settings.Default.AfisStripUdpPort);
			this._udpClient.Listen();

			//Spustí vykreslování letadel.
			this._rendererController.Start();
		}

		/// <summary>
		/// Spustí zpětné přehrávání vzdušné situace.
		/// </summary>
		/// <returns>true pokud by replay spuštěn, jinak false.</returns>
		private bool StartReplay()
		{
			var replayCollection = this._sessionContext.DataSourceReplayFiles;

			if (replayCollection == null) {
				this.IsReplaying = false;
				return false;
			}

			if (replayCollection[AircraftDataSourceEnum.OGN] != null) {
				this._aircraftDataSourceClient.StartOgnReplay(replayCollection[AircraftDataSourceEnum.OGN]);
			}


			if (replayCollection[AircraftDataSourceEnum.OSN] != null) {
				this._aircraftDataSourceClient.StartOsnReplay(replayCollection[AircraftDataSourceEnum.OSN]);
			}

			this.IsReplaying = true;
			return true;

		}

		/// <summary>
		/// Uloží hodnoty do konfiguračního souboru.
		/// </summary>
		public void StoreToConfig()
		{
			this.StoreWeatherInfoToConfig();
			this.StoreStatusPanelInfoToConfig();
			this.StoreMapInfoToConfig();

			Settings.Default.AirportsAreVisible = this.AirportsVisibility;

			this._sessionContext.SaveToConfig();

			Settings.Default.Save();
		}

		/// <summary>
		/// Připojí klienta k API.
		/// </summary>
		private void ConnectToDataClient()
		{
			var monitoredArea = this._sessionContext.MonitoredArea;
			if (Settings.Default.OgnEnabled) {
				this._aircraftDataSourceClient.ConnectToOgn(monitoredArea);
			}

			if (Settings.Default.OsnEnabled) {
				this._aircraftDataSourceClient.ConnectToOsn(monitoredArea);
			}
		}

		/// <summary>
		/// Uloží informace o nastavení počasí do konfigu. 
		/// </summary>
		partial void StoreWeatherInfoToConfig();

		/// <summary>
		/// Inicializuje viewModel pro mapy.
		/// </summary>
		partial void MapViewModelInit();

		/// <summary>
		/// Inicializuje viewModel pro počasí.
		/// </summary>
		partial void WeatherViewModelInit();

		/// <summary>
		/// Inicializuje viewModel pro status panel.
		/// </summary>
		partial void StatusPanelViewModelInit();

		/// <summary>
		/// Uloží informace ze status panelu do konfigu.
		/// </summary>
		partial void StoreStatusPanelInfoToConfig();

		/// <summary>
		/// Uloží informace o mapách do konfigu.
		/// </summary>
		partial void StoreMapInfoToConfig();


		/// <summary>
		/// Kolekce obsahující barvy k jednotlivým kategoriím vzdušných prostorů.
		/// </summary>
		[JsonProperty("airspaceCategoriesColors")]
		public  Dictionary<AirspaceCategory, Color> AirspaceCategoriesColors { get; set; }
	}
}
