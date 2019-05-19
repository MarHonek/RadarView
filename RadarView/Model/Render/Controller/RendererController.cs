using System;
using System.Diagnostics;
using System.Windows.Media.TextFormatting;
using System.Windows.Threading;
using RadarView.Model.Managers.DataSwitch;
using RadarView.Properties;

namespace RadarView.Model.Render.Controller
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class RendererController : IRendererController
	{
		/// <summary>
		/// Časový interval aktualizace aktuální polohy letadel (milisekundy).
		/// </summary>
		private readonly int RenderSampleUpdateInterval = Settings.Default.RenderUpdateInterval;

		/// <summary>
		/// Časový interval aktualizace trail dotů (milisekundy).
		/// </summary>
		private readonly int TrailDotUpdateInterval = Settings.Default.AircraftTrailDotsUpdateIntervalMiliseconds;

		/// <summary>
		/// Počet Trail dotů, o které renderer žádá prediktor
		/// </summary>
		private readonly int TrailDotsCount = Settings.Default.AircraftTrailDotsMaxCount;

		/// <summary>
		/// Časovač pro pravidelné aktualizovaní aktualní polohy letadel.
		/// </summary>
		private DispatcherTimer renderSampleUpdateTimer;

		/// <summary>
		/// Počet aktualizace aktualní polohy k aktualizaci polohy trail dotů.
		/// Slouží k tomu, aby bylo možné nastavít různé intervaly aktualizace aktuální polohy a trail dotů.
		/// </summary>
		private int numberOfRenderUpdatesToTrailDotsUpdate;

		/// <summary>
		/// Aktuální počet aktualizací aktuální polohy před aktualizací polohy trail dotů.
		/// </summary>
		private int currentRenderUpdateBeforeTrailDotUpdate;

		/// <summary>
		/// Poslední časy trail dotů před aktualizací.
		/// </summary>
		private DateTime[] lastTrailDotsTimes;


		private readonly IAircraftDataSwitch _aircraftDataSwitch;

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public event EventHandler<RendererControllerEventArgs> SamplesReceived;

		public RendererController(IAircraftDataSwitch aircraftDataSwitch)
		{
			this._aircraftDataSwitch = aircraftDataSwitch;

			//Počet aktuálizací aktuální polohy než bude aktualizována poloha trail dotů.
			this.numberOfRenderUpdatesToTrailDotsUpdate = this.TrailDotUpdateInterval / this.RenderSampleUpdateInterval;

			this.renderSampleUpdateTimer = new DispatcherTimer(DispatcherPriority.Render);
			this.renderSampleUpdateTimer.Interval = new TimeSpan(0,0,0,0,this.RenderSampleUpdateInterval);
			this.renderSampleUpdateTimer.Tick += this.RenderSampleUpdateTimer_Tick;
		}

		/// <summary>
		/// Zažádá o data obsahující aktuální polohu letadel.
		/// </summary>
		private void RenderSampleUpdateTimer_Tick(object sender, EventArgs e)
		{
			//Zjistí aktuální čas.
			var timeNow = DateTime.UtcNow;
			var timeNowWithoutMillis = new DateTime(timeNow.Year, timeNow.Month, timeNow.Day, timeNow.Hour, timeNow.Minute, timeNow.Second);

			//Pokud byla aktuální poloha aktulizována tolikrát aby byla aktualizována poloha trail dotů, vyresetuje počítadlo.
			if (this.currentRenderUpdateBeforeTrailDotUpdate >= this.numberOfRenderUpdatesToTrailDotsUpdate - 1) {
				this.currentRenderUpdateBeforeTrailDotUpdate = 0;
			}

			if (this.currentRenderUpdateBeforeTrailDotUpdate == 0) {
				//Je čas na aktualizaci polohy traildotů.
				//Dotáže se na polohu a časy uloží.
				var trailDotTimes = new DateTime[this.TrailDotsCount];
				var correctedTime = this.CorrectTime(timeNowWithoutMillis);

				for (var i = 0; i < this.TrailDotsCount; i++) {
					correctedTime = correctedTime.AddMilliseconds(-this.TrailDotUpdateInterval);
					trailDotTimes[i] = correctedTime;
				}

				this.lastTrailDotsTimes = trailDotTimes;
			}

			//Požádá o pozice.
			var samples = this._aircraftDataSwitch.GetSample(timeNowWithoutMillis, this.lastTrailDotsTimes);

			//Aktualizuje počítadlo vykreslování aktuální polohy vůči trail dotům.
			this.currentRenderUpdateBeforeTrailDotUpdate++;

			//Odešle informace k překreslení.
			this.SamplesReceived?.Invoke(this, new RendererControllerEventArgs(samples));
		}

		/// <summary>
		/// Opraví drobné odchylky v čase způsobené nepřesností časovače.
		/// </summary>
		/// <param name="dateTime">čas před korekcí.</param>
		/// <returns>čas po korekci.</returns>
		private DateTime CorrectTime(DateTime dateTime)
		{
			var traildotIntervalSeconds = this.TrailDotUpdateInterval / 1000;
			var modulo = dateTime.Second % traildotIntervalSeconds;
			if (modulo == 0) {
				return dateTime;
			}

			var halfOfUpdateInterval = traildotIntervalSeconds / 2;
			if (modulo > halfOfUpdateInterval) {
				return dateTime.AddSeconds(traildotIntervalSeconds - modulo);
			} else {
				return dateTime.AddSeconds(-modulo);
			}
		}

		/// <summary>
		/// Synchronizuje čas vykreslování na sekundy dělitelné pěti.
		/// </summary>
		private void SynchronizeToSecondsDivisibleByFive()
		{
			var syncTimer = new DispatcherTimer(DispatcherPriority.Loaded);
			syncTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
			syncTimer.Tick += this.SyncTimer_Tick;
			syncTimer.Start();
		}

		private void SyncTimer_Tick(object sender, EventArgs e)
		{
			var utcNow = DateTime.UtcNow;
			if (utcNow.Second % 5 == 0) {
				var timer = (DispatcherTimer) sender;
				timer.Stop();
				this.renderSampleUpdateTimer.Start();
			}
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Start()
		{
			this.SynchronizeToSecondsDivisibleByFive();
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Stop()
		{
			this.renderSampleUpdateTimer.Stop();
		}
	}
}
