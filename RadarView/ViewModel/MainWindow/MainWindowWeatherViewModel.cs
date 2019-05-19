using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using System.Windows.Threading;
using AutoMapper.Configuration.Conventions;
using RadarView.Model.Entities.Exceptions;
using RadarView.Properties;
using RadarView.ViewModel.Base;

namespace RadarView.ViewModel.MainWindow
{
	/// <summary>
	/// ViewModel pro zobrazení informací o počasí.
	/// </summary>
	public partial class MainWindowViewModel
	{
		/// <summary>
		/// Časový interval aktualizace meteorologických dat (minuty).
		/// </summary>
		private readonly int WeatherTimerIntervalSeconds = Settings.Default.MeteoDataUpdateIntervalSeconds;

		/// <summary>
		/// Časový interval aktualizace radarových snímků (sekundy).
		/// </summary>
		private readonly int PrecipitationRadarTimeIntervalSeconds = Settings.Default.PrecipitationRadarUpdateIntervalSeconds;

		/// <summary>
		/// Název místa, kde se nachází meteorologická stanice.
		/// </summary>
		private string _stationName;
		public string StationName
		{
			get { return this._stationName; }
			set
			{
				this._stationName = value;
				this.OnPropertyChanged(nameof(this.StationName));
			}
		}

		/// <summary>
		/// Viditelnost (metr).
		/// </summary>
		private int? _visibility;
		public int? Visibility
		{
			get { return this._visibility; }
			set
			{
				this._visibility = value; 
				this.OnPropertyChanged(nameof(this.Visibility));
			}
		}

		/// <summary>
		/// Rychlost větru.
		/// </summary>
		private double? _windSpeed;
		public double? WindSpeed
		{
			get { return this._windSpeed; }
			set
			{
				this._windSpeed = value;
				this.OnPropertyChanged(nameof(this.WindSpeed));
				this.OnPropertyChanged(nameof(this.WindInfo));
			}
		}

		/// <summary>
		/// Směr větru.
		/// </summary>
		private double? _windCourse;
		public double? WindCourse
		{
			get { return this._windCourse; }
			set
			{
				this._windCourse = value;
				this.OnPropertyChanged(nameof(this.WindCourse));
				this.OnPropertyChanged(nameof(this.WindInfo));
			}
		}

		/// <summary>
		/// Teplota (°C).
		/// </summary>
		private double? _temperature;
		public double? Temperature
		{
			get { return this._temperature; }
			set
			{
				this._temperature = value;
				this.OnPropertyChanged(nameof(this.Temperature));
			}
		}

		/// <summary>
		/// Tlak (hPa).
		/// </summary>
		private double? _pressure;
		public double? Pressure
		{
			get { return this._pressure; }
			set
			{
				this._pressure = value;
				this.OnPropertyChanged(nameof(this.Pressure));
			}
		}

		/// <summary>
		/// Vlhkost (%).
		/// </summary>
		private int? _humidity;
		public int? Humidity
		{
			get { return this._humidity; }
			set
			{
				this._humidity = value;
				this.OnPropertyChanged(nameof(this.Humidity));
			}
		}

		/// <summary>
		/// Info o povětrnostních podmínkách.
		/// </summary>
		public string WindInfo
		{
			get
			{
				if (this.WindSpeed == null || this.WindCourse == null) {
					return null;
				}

				return this.WindSpeed + " m/s, " + this.WindCourse + "°";
			}
		}

		/// <summary>
		/// Viditelnost informací o počasí.
		/// </summary>
		private bool _weatherInfoVisibility;
		public bool WeatherInfoVisibility
		{
			get { return this._weatherInfoVisibility; }
			set
			{
				this._weatherInfoVisibility = value; 
				this.OnPropertyChanged(nameof(this.WeatherInfoVisibility));
			}
		}

		/// <summary>
		/// Viditelnost srážkového radaru.
		/// </summary>
		private bool _precipitationRadarVisibility;
		public bool PrecipitationRadarVisibility
		{
			get { return this._precipitationRadarVisibility; }
			set
			{
				this._precipitationRadarVisibility = value;
				this._precipitationRadar.ImageVisibility = value;
				this.OnPropertyChanged(nameof(this.PrecipitationRadarVisibility));
				Settings.Default.PrecipitationRadarIsVisible = this._precipitationRadarVisibility;
				Settings.Default.Save();
			}
		}

		/// <summary>
		/// Čas poslední aktualizace radarového snímku srážek.
		/// </summary>
		private DateTime? _precipitationRadarImageUpdateDateTime;
		public DateTime? PrecipitationRadarImageUpdateDateTime
		{
			get { return this._precipitationRadarImageUpdateDateTime; }
			set
			{
				this._precipitationRadarImageUpdateDateTime = value;
				this.OnPropertyChanged(nameof(this.PrecipitationRadarImageUpdateDateTime));
			}
		}

		/// <summary>
		/// Inicializuje viewModel pro počasí.
		/// </summary>
		partial void WeatherViewModelInit()
		{
			Task.Run(async () => {
				this.PrecipitationRadarImageUpdateDateTime = await this._precipitationRadarDataService.DownloadCurrentPrecipitationRadarImageAsync();
			});

			this.WeatherInfoVisibility = Settings.Default.MeteoDataAreVisible;
			this._precipitationRadar.ImageOpacity = Settings.Default.PrecipitationRadarOpacity;
			this.PrecipitationRadarVisibility = Settings.Default.PrecipitationRadarIsVisible;

			var weatherRequestTimer = new DispatcherTimer(DispatcherPriority.Background);
			weatherRequestTimer.Interval = new TimeSpan(0, 0, 0, this.WeatherTimerIntervalSeconds);
			weatherRequestTimer.Tick += this.WeatherRequestTimer_Tick;
			weatherRequestTimer.Start();

			this.WeatherRequestTimer_Tick(this, null);

			var precipitationRequestTimer = new DispatcherTimer(DispatcherPriority.Background);
			precipitationRequestTimer.Interval = new TimeSpan(0, 0, 0, this.PrecipitationRadarTimeIntervalSeconds);
			precipitationRequestTimer.Tick += this.PrecipitationRequestTimer_Tick;
			precipitationRequestTimer.Start();
		}

		private void PrecipitationRequestTimer_Tick(object sender, EventArgs e)
		{
			Task.Run(async () => {
				this.PrecipitationRadarImageUpdateDateTime =
					await this._precipitationRadarDataService.DownloadCurrentPrecipitationRadarImageAsync();
			});
		}

		private void WeatherRequestTimer_Tick(object sender, EventArgs e)
		{
			Task.Run(async () =>
			{
				try {
					var result = await this._weatherDataService.GetCurrentWeatherAsync(this._sessionContext.DefaultCenter);
					if (result != null) {
						this.Humidity = result.Weather.Humidity;
						this.Temperature = Math.Round(result.Weather.TemperatureCelsius, 2);
						this.WindSpeed = Math.Round(result.Wind.Speed, 2);
						this.WindCourse = Math.Round(result.Wind.Degree, 2);
						this.Pressure = result.Weather.Pressure;
						this.Visibility = result.Visibility;
						this.StationName = result.Name;
					}
				}
				catch (RestApiClientException restApiClientException) {
					Debug.WriteLine("CHYBA pri stahovani informaci o pocasi\n" + restApiClientException.Message);
				}
				catch (Exception exception) {
					Debug.WriteLine(exception);
				}
			});
		}

		/// <summary>
		/// Uloží informace o nastavení počasí do konfigu. 
		/// </summary>
		partial void StoreWeatherInfoToConfig()
		{
			Settings.Default.MeteoDataAreVisible = this.WeatherInfoVisibility;
			Settings.Default.PrecipitationRadarIsVisible = this.PrecipitationRadarVisibility;
		}
	}
}
