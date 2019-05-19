using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using RadarView.Model.DataService.AviationData.AirportDataService;
using RadarView.Model.DataService.AviationData.AirspaceDataService;
using RadarView.Model.DataService.Map.MapImageDataSource;
using RadarView.Model.DataService.Network;
using RadarView.Model.DataService.Weather;
using RadarView.Model.DataService.Weather.PrecipitationRadarDataService;
using RadarView.Model.Entities.AviationData;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Entities.MapLayer;
using RadarView.Model.Managers.DataSwitch;
using RadarView.Model.Managers.LiveDataManager;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render;
using RadarView.Model.Render.Background.Airport;
using RadarView.Model.Render.Background.Airspace;
using RadarView.Model.Render.Background.BackgroundManager;
using RadarView.Model.Render.Background.Map;
using RadarView.Model.Render.Background.PrecipitationRadar;
using RadarView.Model.Render.Controller;
using RadarView.Model.Render.MeasuringLine;
using RadarView.Model.Render.Targets;
using RadarView.Model.Service.AircraftDataSourceService;
using RadarView.Model.Service.ApiRestClient;
using RadarView.Model.Service.AviationData;
using RadarView.Model.Service.ImageDownloadClient;
using RadarView.Model.Service.SessionContext;
using RadarView.Model.Service.UdpCommunicationClient;
using RadarView.Properties;
using RadarView.View;
using RadarView.ViewModel.MainWindow;
using RadarView.ViewModel.WindowAirportSelection;
using RadarView.ViewModel.WindowSettings;
using Unity;
using Unity.Lifetime;

namespace RadarView
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string[] Args;

        protected override async void OnStartup(StartupEventArgs e)
        {
            Args = e.Args;
            base.OnStartup(e);

			var unityContainer = new UnityContainer();
			unityContainer.AddExtension(new Diagnostic());

			unityContainer.RegisterType<IMainWindowViewModel, MainWindow>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IMainWindowViewModel, MainWindowViewModel>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IWindowsSettingsViewModel, WindowsSettingsViewModel>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IWindowsSettingsViewModel, SettingsWindow>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IWindowAirportSelectionViewModel, WindowAirportSelection>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IWindowAirportSelectionViewModel, WindowAirportSelectionViewModel>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IAirspaceFileReader, AirspaceFileReader>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IAirportFileReader, AirportFileReader>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IAirportDataService, AirportDataService>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IAirspaceDataService, AirspaceDataService>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IRendererController, RendererController>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<ISessionContext, SessionContext>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IAircraftLiveDataManager, AircraftLiveDataManager>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IAircraftDataSourceClient, AircraftDataSourceClient>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IRendererController, RendererController>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IApiRestClient, ApiRestClient>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IWeatherDataService, WeatherDataService>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IWindowsSettingsViewModel, WindowsSettingsViewModel>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IImageDownloadClient, ImageDownloadClient>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IPrecipitationRadarDataService, PrecipitationRadarDataService>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IRenderer, Renderer>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IAircraftDataSwitch, AircraftDataSwitch>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IBackgroundManager, BackgroundManager>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IBackgroundMapManager, BackgroundMapManager>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IViewportProjection, ViewportProjection>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IBackgroundAirportManager, BackgroundAirportManager>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IBackgroundAirspaceManager, BackgroundAirspaceManager>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IAfisStripDataService, AfisStripDataService>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IUdpCommunicationClient, UdpCommunicationClient>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IBackgroundPrecipitationRadar, BackgroundPrecipitationRadar>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IMapImageDataService, MapImageDataService>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<IMeasuringLine, MeasuringLine>(new ContainerControlledLifetimeManager());
			unityContainer.RegisterType<ITargetComponentsManager, TargetComponentsManager>(new ContainerControlledLifetimeManager());

			await this.Initialize(unityContainer);
		
			unityContainer.Resolve<MainWindow>().Show();

		}

		/// <summary>
		/// Inicializuje zdroje a nastavení aplikace.
		/// </summary>
        private async Task Initialize(IUnityContainer unityContainer)
        {
	        var sessionContext = unityContainer.Resolve<ISessionContext>();
	        var airportDataService = unityContainer.Resolve<IAirportDataService>();
	        var airspaceFileReader = unityContainer.Resolve<IAirspaceFileReader>();

			//Letiště.
	        var airport = Settings.Default.AirportSelected;
	
			//Vybrané letiště uživatelem.
		    var currentAirport = await airportDataService.GetAirportByIcaoAsync(airport);
		    sessionContext.CurrentAirport = currentAirport;

			//Oblast kolem letiště. Slouží např. k detekci letadla na zemi.
			sessionContext.AirportArea = new Tuple<int, BoundingBox>(currentAirport.Altitude, BoundingBox.CreateBoundingBox(currentAirport.Location, Settings.Default.AirportAreaSizeMeters));

			var monitoredAreaSerializedLocation = Settings.Default.MonitoredArea;
	        sessionContext.MonitoredArea = new BoundingBox(monitoredAreaSerializedLocation);

			//Vzdušné prostory
	        await airspaceFileReader.ReadAllAsync();

			//Mapové podklady.
	        var mapLayersJson = Settings.Default.MapConfig;
	        var mapLayers = JsonConvert.DeserializeObject<MapLayerCollection>(mapLayersJson);
	        sessionContext.MapLayerCollection = mapLayers;

			this.SolveAppArgs(sessionContext);

			//Zkopíruje všechna nastavení do uživatelského adresáře.
			foreach (SettingsProperty currentProperty in Settings.Default.Properties) {
				Settings.Default[currentProperty.Name] = Settings.Default[currentProperty.Name];
			}

			Settings.Default.Save();
		}

		/// <summary>
		/// Vyřeší argumenty aplikace.
		/// </summary>
		private void SolveAppArgs(ISessionContext sessionContext)
		{
			var args = Args;
			var runsWithReplayParam = args.Length > 0 && args[0] == "-r";
			if (!runsWithReplayParam) {
				return;
			}

			sessionContext.DataSourceReplayFiles = new Dictionary<AircraftDataSourceEnum, string>();

			var regexOgn = new Regex("-g[0-9]");
			var paramOgn = args.SingleOrDefault(x => regexOgn.IsMatch(x));
			sessionContext.DataSourceReplayFiles[AircraftDataSourceEnum.OGN] = paramOgn?[2].ToString();

			var regexOsn = new Regex("-s[0-9]");
			var paramOsn = args.SingleOrDefault(x => regexOsn.IsMatch(x));
			sessionContext.DataSourceReplayFiles[AircraftDataSourceEnum.OSN] = paramOsn?[2].ToString();
		}
    }
}
