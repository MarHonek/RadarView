using System;
using System.Collections.Generic;
using System.Linq;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Abstract;
using RadarView.Properties;

namespace RadarView.Model.Render.Background.Airport
{
    /// <summary>
    /// Reprezentuje letiště jako objekt vykreslení.
    /// </summary>
    public class BackgroundAirport : RenderObject, IRenderable
    {
		/// <summary>
		/// Letiště.
		/// </summary>
		public Entities.AviationData.Airports.Airport Airport { get; set; }

        /// <summary>
        /// Seznam vzletových dráh letiště.
        /// </summary>
        private List<BackgroundRunway> runways;

        /// <summary>
        /// Mapová projekce
        /// </summary>
        private IViewportProjection mapProjection;

		/// <summary>
		/// font textu
		/// </summary>
		private SpriteFont font;

		/// <summary>
		/// Pozice textu (ICAO letiště).
		/// </summary>
		private Vector2 IcaoPosition;

		/// <summary>
		/// Zeměpisná poloha textu (ICAO letiště).
		/// </summary>
		private Location IcaoLabelLocation;

        /// <summary>
        /// Konstruktor vytvoří nové objekty pro vzletové dráhy (runway) a přidá je do seznamu.
        /// </summary>
        /// <param name="mapProjection">mapová projekce</param>
        /// <param name="airport">letiště</param>
        /// <param name="color">barva</param>
        public BackgroundAirport(IViewportProjection mapProjection, Entities.AviationData.Airports.Airport airport, Color color, float strokeThickness) : base(color)
        {
			this.font = Renderer.spriteFont;
            this.mapProjection = mapProjection;
			this.mapProjection.ViewportChanged += this.MapProjectionViewportChanged;
			this.Airport = airport;
			this.IcaoLabelLocation = this.DetermineAirportIcaoLabelLocation(airport, mapProjection);
			this.IcaoPosition = this.DetermineAirportIcaolLabelPosition();
            this.runways = new List<BackgroundRunway>();
            foreach (var runway in airport.Runway) {
	            if (runway.StartLocation != null && runway.EndLocation != null) {
					this.runways.Add(new BackgroundRunway(mapProjection, runway.StartLocation, runway.EndLocation, this.Color, strokeThickness));
	            }
            }  
        }

		private void MapProjectionViewportChanged(object sender, EventArgs e)
		{
			if (this.IcaoLabelLocation != null) {
				this.IcaoPosition = this.DetermineAirportIcaolLabelPosition();
			}
		}

		/// <summary>
		/// Změní viditelnost názvu letiště i vzletových/přistávacích drah.
		/// </summary>
		/// <param name="shouldBeVisible"></param>
		public void ChangeVisibilityOfAirportAndRunways(bool shouldBeVisible)
		{
			this.IsVisible = shouldBeVisible;
			this.runways.ForEach(r => r.IsVisible = shouldBeVisible);
		}

		/// <summary>
		/// Určí pozici (pixely) popisku s ICAO letiště. 
		/// </summary>
		/// <returns>Pozice popisku s ICAO letiště.</returns>
		private Vector2 DetermineAirportIcaolLabelPosition()
		{
			var position = this.mapProjection.LocationToViewportPoint(this.IcaoLabelLocation);
			position.Y += 10;

			var textSize = this.font.MeasureString("XXXX");
			position.X -= textSize.X / 2;
			return position;
		}

		/// <summary>
		/// Určí zeměpisnou polohu pro vykreslení textu (ICAO letiště).
		/// </summary>
		/// <param name="airport">letiště pro které se bude text vykreslovat.</param>
		/// <param name="projection">mapový projektor.</param>
		/// <returns>Zeměpisná poloha popisku..</returns>
		private Location DetermineAirportIcaoLabelLocation(Entities.AviationData.Airports.Airport airport, IViewportProjection projection) 
		{
			var runways = airport.Runway;

			if (runways.Count <= 0) {
				return airport.Location;
			}

			if (!runways.Exists(x => x.StartLocation != null && x.EndLocation != null)) {
				return airport.Location;
			}

			//Vypočítá pozici aby se text vykreslil pod vzletovýma dráhama.
			var latitude = runways
				.Select(x => x.StartLocation.Latitude)
				.Concat(runways.Select(x => x.EndLocation.Latitude))
				.Min();

			//Vypočíta pozici aby se text vykreslil na středu vzletových drah.
			var longitudes = runways.Select(x => x.StartLocation.Longitude).Concat(runways.Select(x => x.EndLocation.Longitude));
			var minLongitude = longitudes.Min();
			var maxLongitude = longitudes.Max();
			var centerLongitude = (minLongitude + maxLongitude) / 2;

			return new Location(latitude, centerLongitude);
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {
            this.runways.ForEach(runway => runway.Draw(spriteBatch, drawBatch));
			spriteBatch.DrawString(this.font, this.Airport.IcaoIdent ?? "", this.IcaoPosition, this.Color);
        }
    }
}
