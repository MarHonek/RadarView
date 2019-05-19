using System;
using System.Collections.Generic;
using System.Diagnostics;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RadarView.Model.Entities.Aviation;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Abstract;
using RadarView.Properties;
using RadarView.Utils;

namespace RadarView.Model.Render.Targets
{
    /// <summary>
    /// Reprezentuje komponenty cíle (aktuální polohu,  předešlé polohy, popisek, predikovanou polohu, poslední známou polohu atd).
    /// </summary>
    public class TargetComponents : IRenderable, IDisposable
    {
        /// <summary>
        /// Mapová projekce.
        /// </summary>
        private IViewportProjection projection;

        /// <summary>
        /// Letadlo které je vykreslováno.
        /// </summary>
        private Aircraft aircraft;
        public Aircraft Aircraft
        {
            get
            {
                return this.aircraft;
            }
            set
            {
	            this.aircraft = value;

	            //Schová/zobrazí speed vektor pokud/není letadlo v kroužení.
	            if (this.aircraft.CurrentStateVector.Maneuver == AircraftManeuver.Circling) {
		            this.SpeedVector.IsVisible = false;
	            } else {
		            this.SpeedVector.IsVisible = true;
	            }

	            //Aktualizuje polohu cíle.
	            this.UpdateTargetComponentsPosition();
            }
        }

		/// <summary>
		/// Maximální čas po který bude cíl zneviditelňován (čím starší informace ze zdroje tím menší viditelnost).
		/// Pokud je čas překročen cíl bude neviditelný.
		/// </summary>
		private readonly int MaxFadingTime = Settings.Default.AircraftMaxFadingTime;

        /// <summary>
        /// Maximální počet trail dotů, které lze zobrazit.
        /// </summary>
        public int TrailDotsMaxCount { get; set; }

        /// <summary>
        /// Počet zobrazovaných trail dotů.
        /// </summary>
        public int TrailDotsCount { get; set; }

        /// <summary>
        /// Maximální počet zobrazených reálných fixů.
        /// </summary>
        public int RealFixesMaxCount { get; set; }

        /// <summary>
        /// Počet zobrazených reálných fixů.
        /// </summary>
        public int RealFixesCount { get; set; }

        /// <summary>
        /// Popisek cíle
        /// </summary>
        public TargetLabel Label { get; set; }

        /// <summary>
        /// Spojnice mezi popiskem a cílem
        /// </summary>
        public TargetLabelConnector TargetLabelConnector { get; set; }

        /// <summary>
        /// Cíl (aktuální poloha letadla)
        /// </summary>
        public Target Target { get; set; }

        /// <summary>
        /// Vektor, určující rychylost a směr letu.
        /// </summary>
        public TargetSpeedVector SpeedVector { get; set; }

        /// <summary>
        /// Historické polohy cíle
        /// </summary>
        public List<TargetTRailDot> TrailDots { get; set; }

        /// <summary>
        /// Seznam všech fixů získaných z datového zdroje.
        /// </summary>
        public List<TargetRealFix> RealFixes { get; }

        /// <summary>
        /// Velikost objektu reprezentující poslední polohu získanou ze zdroje
        /// </summary>
        private readonly Vector2 RealFixSize = new Vector2(Settings.Default.AircraftRealFixTextureSize);

        /// <summary>
        /// Rozměry cíle (targetu)
        /// </summary>
        private readonly Vector2 TargetSize = new Vector2(Settings.Default.AircraftTargetTextureSize);

        /// <summary>
        /// Rozměry trail dotů.
        /// </summary>
        private readonly Vector2 TrailDotSize = new Vector2(Settings.Default.AircraftTrailDotTextureSize);

        /// <summary>
        /// Příznak určující, zda mají být zobrazený reálné fixy.
        /// </summary>
        private readonly bool RealFixesVisibility = Settings.Default.AircraftRealFixesAreVisible;

		/// <summary>
		/// Nastaví barvu pro všechny komponenty cíle.
		/// </summary>
		private Color _color;
        public Color Color
        {
            set
            {
                this._color = value;
                this.Target.Color = this._color;
                this.Label.Color = this._color;
                this.TargetLabelConnector.Color = this._color;
                this.TrailDots.ForEach(t => t.Color = this._color);
                this.SpeedVector.Color = this._color;
                this.RealFixes.ForEach(f => f.Color = this._color);
            }
        }

        /// <summary>
        /// Nastaví viditelnost pro všechny komponenty cíle.
        /// </summary>
        private bool _isVisible;
        public bool IsVisible
        {
			get 
			{
				return this._isVisible;
			}
            set
            {
                this._isVisible = value;
                this.ChangeVisibility(this._isVisible);
            }
        }

        /// <summary>
        /// Změní viditelnost všech komponent.
        /// </summary>
        /// <param name="visibility">true pokud má být objekt viditelný.</param>
        private void ChangeVisibility(bool visibility)
        {
            this.Target.IsVisible = visibility;
            this.Label.IsVisible = visibility;
            this.TargetLabelConnector.IsVisible = visibility;
            this.TrailDots.ForEach(t => t.IsVisible = visibility);
            this.SpeedVector.IsVisible = visibility;
            this.RealFixes.ForEach(t => t.IsVisible = visibility);
        }

        /// <summary>
        /// Událost vyvolána při kliknutí na popisek.
        /// </summary>
        public event EventHandler Clicked;

        public TargetComponents(IViewportProjection mapProjection, Aircraft aircraft, int trailDotsMaxCount, int trailDotsCount, int realFixesMaxCount, int realFixesCount, Color color)
        {
            this._color = color;
            this.aircraft = aircraft;
            this.projection = mapProjection;
            this.TrailDots = new List<TargetTRailDot>();
            this.Target = new Target(mapProjection, aircraft.CurrentStateVector.RealFix.Location, this.TargetSize, aircraft.Info.AircraftType, color);
            this.Label = new TargetLabel(this.Target, aircraft, this.projection, color);
            this.Label.MouseLeftClick += this.Label_MouseLeftClick;
            this.TargetLabelConnector = new TargetLabelConnector(this.Target, this.Label, this.projection, color);
            this.RealFixes = new List<TargetRealFix>();
            this.SpeedVector = new TargetSpeedVector(mapProjection, this.Target, aircraft.SpeedVectorEndFix.Location, color);
            this.TrailDotsMaxCount = trailDotsMaxCount;
            this.TrailDotsCount = trailDotsCount;
            this.RealFixesMaxCount = realFixesMaxCount;
            this.RealFixesCount = realFixesCount;
        }

        /// <summary>
        /// Aktualizuje kontrolu uživatelského vstupu
        /// </summary>
        public void UpdateMouseState(MouseState mouseState)
        {
            this.Label.UpdateMouseState(mouseState);
        }

        private void Label_MouseLeftClick(object sender, EventArgs e)
        {
            this.Clicked?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Aktualizuje pozice všech komponent cíle.
        /// </summary>
        public void UpdateTargetComponentsPosition()
        {
			var currentDateTime = DateTime.UtcNow;
			currentDateTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, currentDateTime.Hour, currentDateTime.Minute, currentDateTime.Second , 0);


			this.Target.Location = this.aircraft.CurrentStateVector.CurrentPredictedFix.Location;

			this.SetFadingOpacity(currentDateTime, this.Target, this.aircraft.CurrentStateVector.RealFix.DateTime);
			this.SetFadingOpacity(currentDateTime, this.Label, this.aircraft.CurrentStateVector.RealFix.DateTime);
			this.SetFadingOpacity(currentDateTime, this.TargetLabelConnector, this.aircraft.CurrentStateVector.RealFix.DateTime);
			this.SetFadingOpacity(currentDateTime, this.SpeedVector, this.aircraft.CurrentStateVector.RealFix.DateTime);

			//Odstraní všechny trailDoty a fixy získané z datových zdrojů.
			foreach (var item in this.TrailDots)
            {
                item.Dispose();
            }

            foreach (var item in this.RealFixes) 
            {
                item.Dispose();
            }

            this.RealFixes.Clear();
            this.TrailDots.Clear();



			//Vytvoření nových traildotů a nastavení opacity pro fadování.
            for (var i = 0; i < Math.Min(this.RealFixesCount, this.aircraft.TrailDotsFixes.Count); i++) 
            {
                var newTrailDot = new TargetTRailDot(this.projection, this.aircraft.TrailDotsFixes[i].Location, this.TrailDotSize, this._color);
				this.SetFadingOpacity(currentDateTime, newTrailDot, this.aircraft.TrailDotsFixes[i].DateTime);

				this.TrailDots.Add(newTrailDot);
            }

			//Vytvoření nových reálných fixů a nastavení opacity pro fadování.
			for (var i = this.aircraft.RealFixes.Count - 1; i >= Math.Max(this.aircraft.RealFixes.Count - this.RealFixesCount, 0); i--) 
            {
                var newKnownFix = new TargetRealFix(this.projection, this.aircraft.RealFixes[i].Location, this.RealFixSize, this._color);
				this.SetFadingOpacity(currentDateTime, newKnownFix, this.aircraft.RealFixes[i].DateTime);
							
				this.RealFixes.Add(newKnownFix);
            }

            this.Label.Aircraft = this.aircraft;
            this.SpeedVector.Location = this.aircraft.SpeedVectorEndFix.Location;
        }

		/// <summary>
		/// Vypočítá a nastaví viditelnost pro postupné zneviditelnování objektů.
		/// </summary>
		private void SetFadingOpacity(DateTime currentDateTime, RenderObject renderObject, DateTime fixDateTime) 
		{
			//fade => zajistí postupné zneviditelnovani.
			if (currentDateTime > fixDateTime.AddSeconds(this.MaxFadingTime)) {
				renderObject.Opacity = 0;
			} else {
				//Hodnota 100 je maximální viditelnost (opacity) 0 je žádná viditelnost.
				renderObject.Opacity = MathExtension.LinearInterpolateInBoundlessInterval(100, 0, currentDateTime, currentDateTime.AddSeconds(-this.MaxFadingTime), fixDateTime);
			}
		}

        /// <summary>
        /// Uvolní všechny prostředky
        /// </summary>
        public void Dispose()
        {
            this.Target.Dispose();
            this.Label.Dispose();
            this.TrailDots.ForEach(t => t.Dispose());
            this.SpeedVector.Dispose();
            this.RealFixes.ForEach(f => f.Dispose());
            this.TargetLabelConnector.Dispose();   
        }

        /// <summary>
        /// Vykreslí komponenty
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {
	        this.Target.Draw(spriteBatch, drawBatch);
	        this.Label.Draw(spriteBatch, drawBatch);
	        this.TargetLabelConnector.Draw(spriteBatch, drawBatch);

	        if (this.RealFixesVisibility) {
		        for (var i = 0; i < Math.Min(this.RealFixesCount, this.RealFixes.Count); i++) {
			        this.RealFixes[i].Draw(spriteBatch, drawBatch);
		        }
	        }

	        for (var i = 0; i < Math.Min(this.TrailDots.Count, this.TrailDotsCount); i++) {
		        this.TrailDots[i].Draw(spriteBatch, drawBatch);
	        }

	        this.SpeedVector.Draw(spriteBatch, drawBatch);
        }
    }
}
