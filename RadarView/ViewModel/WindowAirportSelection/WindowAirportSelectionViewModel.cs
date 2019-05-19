using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using RadarView.Model.DataService.AviationData.AirportDataService;
using RadarView.Model.DataService.Map.MapImageDataSource;
using RadarView.Model.Entities.AviationData.Airports;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Service.SessionContext;
using RadarView.Properties;
using RadarView.Utils;
using RadarView.ViewModel.Base;
using BoundingBox = RadarView.Model.Entities.Geographic.BoundingBox;

namespace RadarView.ViewModel.WindowAirportSelection
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class WindowAirportSelectionViewModel : BaseViewModel, IWindowAirportSelectionViewModel
	{

		/// <summary>
		/// Rozměr stažených map
		/// </summary>
		private readonly Vector2 MapSize = new Vector2(Settings.Default.MapDownloadResolution);

		/// <summary>
		/// API klíč služby pro stahování mapových podkladů
		/// </summary>
		private readonly string ApiKey = Settings.Default.MapSourceApiKey;

		/// <summary>
		/// Url služby pro stahování mapových podkladů
		/// </summary>
		private readonly string Url = Settings.Default.MapSourceUrl;

		/// <summary>
		/// Název normálních mapových podkladů.
		/// </summary>
		private const string BASIC_MAP_NAME = "Základní mapa";

		/// <summary>
		/// Vzdálenost o středu sledované oblasti (v metrech). Tato vzdálenost určuje hranice sledované oblasti.
		/// </summary>
		private static readonly int MonitoredAreaDistanceFromCenter = Settings.Default.MonitoredAreaAutoSetSizeKilometers * 1000;

		private readonly IAirportDataService _airportDataService;

		private readonly ISessionContext _sessionContext;

		private readonly IMapImageDataService _mapImageDataService;

		/// <summary>
		/// Seznam letišť.
		/// </summary>
		private ObservableCollection<Airport> _airports;
		public ObservableCollection<Airport> Airports
		{
			get { return this._airports; }
			set
			{
				this._airports = value;
				this.OnPropertyChanged(nameof(this.Airports));
			}
		}

		/// <summary>
		/// Vybrané letiště.
		/// </summary>
		private Airport _selectedAirport;
		public Airport SelectedAirport
		{
			get { return this._selectedAirport; }
			set
			{
				this._selectedAirport = value;
				this.Name = this._selectedAirport.Name;
				this.Icao = this._selectedAirport.IcaoIdent;
				this.Longitude = this._selectedAirport.Location.Longitude;
				this.Latitude = this._selectedAirport.Location.Latitude;
				this.Altittude = this._selectedAirport.Altitude;

				var runways = new ObservableCollection<RunwayViewModel>();
				this.SelectedAirport.Runway.ForEach(x => runways.Add(new RunwayViewModel(x.Name, MathExtension.FeetToMeters(x.Length), x.StartHeading, x.EndHeading)));
				this.Runways = new ObservableCollection<RunwayViewModel>(runways);

				this.OnPropertyChanged(nameof(this.Runways));
				this.OnPropertyChanged(nameof(this.SelectedAirport));
			}
		}

		/// <summary>
		/// Název letiště.
		/// </summary>
		private string _name;
		public string Name
		{
			get { return this._name; }
			set
			{
				this._name = value;
				this.OnPropertyChanged(nameof(this.Name));
			}
		}

		/// <summary>
		/// ICAO letiště.
		/// </summary>
		private string _icao;
		public string Icao
		{
			get { return this._icao; }
			set
			{
				this._icao = value; 
				this.OnPropertyChanged(nameof(this.Icao));
			}
		}

		/// <summary>
		/// Informace o vzletových a přistávacích drahách.
		/// </summary>
		public ObservableCollection<RunwayViewModel> Runways { get; set; }

		/// <summary>
		/// Zeměpisná délka délky letiště.
		/// </summary>
		private float? _longitude;
		public float? Longitude
		{
			get { return this._longitude; }
			set
			{
				this._longitude = value;
				this.OnPropertyChanged(nameof(this.Longitude));
			}
		}

		/// <summary>
		/// Zeměpisná šířka polohy letiště.
		/// </summary>
		private float? _latitude;
		public float? Latitude
		{
			get { return this._latitude; }
			set
			{
				this._latitude = value; 
				this.OnPropertyChanged(nameof(this.Latitude));
			}
		}

		/// <summary>
		/// Nadmořská výška letiště.
		/// </summary>
		private float _altitude;
		public float Altittude
		{
			get { return this._altitude; }
			set
			{
				this._altitude = value;
				this.OnPropertyChanged(nameof(this.Altittude));
			}
		}

		/// <summary>
		/// Hodnota progressbaru při stahování mapových podkladů.
		/// </summary>
		private int _progressBarValue;
		public int ProgressBarValue
		{
			get { return this._progressBarValue; }
			set
			{
				this._progressBarValue = value; 
				this.OnPropertyChanged(nameof(this.ProgressBarValue));
			}
		}

		/// <summary>
		/// Příznak určující zda má být progressBar viditelný.
		/// </summary>
		private bool _progressBarVisibility;
		public bool ProgressBarVisibility
		{
			get { return this._progressBarVisibility; }
			set
			{
				this._progressBarVisibility = value; 
				this.OnPropertyChanged(nameof(this.ProgressBarVisibility));
			}
		}


		/// <summary>
		/// Command potvrdí výběr a stáhné mapové podklady centrované na vybrané letiště.
		/// </summary>
		private ICommand _acceptCommand;
		public ICommand AcceptCommand {
			get {
				if (this._acceptCommand == null) {
					this._acceptCommand = new RelayCommand(async x =>
					{
						var result = await this.DownloadMaps();
						if (result) {
							MessageBox.Show("Změny se projeví až po restartování aplikace.");
							((Window)x).Close();
						}
					});
				}
				return this._acceptCommand;
			}
		}

		/// <summary>
		/// Command inicializuje viewmodel
		/// </summary>
		private ICommand _initializeCommand;
		public ICommand InitializeCommand {
			get {
				if (this._initializeCommand == null) {
					this._initializeCommand = new AsyncCommand(async () => { await this.Initialize(); });
				}
				return this._initializeCommand;
			}
		}


		public WindowAirportSelectionViewModel(IAirportDataService airportDataService, ISessionContext sessionContext, IMapImageDataService mapImageDataService)
		{
			this._airportDataService = airportDataService;
			this._sessionContext = sessionContext;
			this._mapImageDataService = mapImageDataService;
		}


		/// <summary>
		/// Stáhne mapové podklady.
		/// </summary>
		private async Task<bool> DownloadMaps()
		{
			//Stáhne podkladové mapy vycentrované na první runway
			//Aktualizuje progress bar
			var selectedAirport = this.SelectedAirport;
			if (selectedAirport == null) {
				MessageBox.Show("Není vybráno žádné letiště.", "Chyba");
				return false;
			}

			//Vyresetuje a zviditelní progressbar.
			this.ProgressBarValue = 0;
			this.ProgressBarVisibility = true;

			var mapProjection = new ViewportProjection {
				RenderSize = new Vector2(this.MapSize.X, this.MapSize.Y)
			};

			mapProjection.Initialize(this.SelectedAirport.Location);

			//Najde vrstvu odpovídající běžným mapovým podkladům.
			var layers = this._sessionContext.MapLayerCollection.Layers;
			var mapLayer = layers.Find(x => x.Name == BASIC_MAP_NAME);
			if (mapLayer == null) {
				MessageBox.Show("V konfiguračním souboru se nenachází informace pro stažení těchto map.", "Chyba");
				return false;
			}

			try {

				//Stáhne mapy pro různé zoom levely.
				this.ProgressBarValue = 20;

				await this._mapImageDataService.DownloadMapAsync(this.GetMapServerUrl(9, this.SelectedAirport.Location),
					mapLayer.ImageName, 9, this.SelectedAirport.IcaoIdent);
				mapProjection.ZoomLevel = 9;
				mapLayer.BoundingBoxes[9] = mapProjection.ViewportToBoundingBox();

				this.ProgressBarValue = 40;

				await this._mapImageDataService.DownloadMapAsync(
					this.GetMapServerUrl(11, this.SelectedAirport.Location), mapLayer.ImageName, 11,
					this.SelectedAirport.IcaoIdent);
				mapProjection.ZoomLevel = 11;
				mapLayer.BoundingBoxes[11] = mapProjection.ViewportToBoundingBox();
				this.ProgressBarValue = 70;

				await this._mapImageDataService.DownloadMapAsync(
					this.GetMapServerUrl(13, this.SelectedAirport.Location), mapLayer.ImageName, 13,
					this.SelectedAirport.IcaoIdent);
				mapProjection.ZoomLevel = 13;
				mapLayer.BoundingBoxes[13] = mapProjection.ViewportToBoundingBox();

				this.ProgressBarValue = 100;
				this.ProgressBarVisibility = false;

			} catch (Exception) {
				MessageBox.Show("Došlo k chybě při stahování map.", "Chyba");
			}

			//Uspešně staženo = dočasné soubory přejmenuje na stálé.
			this._mapImageDataService.ReplaceTemporaryFile(mapLayer.ImageName, 9, this.SelectedAirport.IcaoIdent);
			this._mapImageDataService.ReplaceTemporaryFile(mapLayer.ImageName, 11, this.SelectedAirport.IcaoIdent);
			this._mapImageDataService.ReplaceTemporaryFile(mapLayer.ImageName, 13, this.SelectedAirport.IcaoIdent);

			this._sessionContext.CurrentAirport = this.SelectedAirport;


			//Vypočítá hranice sledovaného prostoru.
			var center = this.SelectedAirport.Location;
			this._sessionContext.MonitoredArea = BoundingBox.CreateBoundingBox(center, MonitoredAreaDistanceFromCenter);

			return true;
		}

		/// <summary>
		/// Vrátí url prostažení mapového podkladu.
		/// </summary>
		/// <returns>Url serveru s mapovým podkladem.</returns>
		private string GetMapServerUrl(int zoomLevel, Location center)
		{
			return string.Format(this.Url, this.ApiKey, this.MapSize.X, this.MapSize.Y, zoomLevel,
				center.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
				center.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Provede inicializaci ViewModelu.
		/// </summary>
		public async Task Initialize()
		{
			this.ProgressBarVisibility = false;

			var airports = await this._airportDataService.GetAllNonClosedAirportsWithLeastOneRunwayInCountryAsync(this._sessionContext.CountryCode);
			this.Airports = new ObservableCollection<Airport>(airports);
		}
	}
}
