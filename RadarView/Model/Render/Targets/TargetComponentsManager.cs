using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RadarView.Model.DataService.Network;
using RadarView.Model.Entities.Aviation;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Service.Config;
using RadarView.Properties;
using RadarView.Utils;

namespace RadarView.Model.Render.Targets
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
    public class TargetComponentsManager : ITargetComponentsManager
    {
        /// <summary>
        /// Maximální počet trail dotů. Zobrazeny při označení letadla
        /// </summary>
        private readonly int TrailDotsMaxCount = Settings.Default.AircraftTrailDotsMaxCount;

        /// <summary>
        /// Standartní počet zobrazených trail dotů.
        /// </summary>
        private readonly int TrailDotsStandartCount = Settings.Default.AircraftTrailDotsStandartCount;

        /// <summary>
        /// Počet zobrazených reálných fixů.
        /// </summary>
        private readonly int RealFixesCount = Settings.Default.AircraftRealFixCount;

        /// <summary>
        /// Vybrané cíl uživatelem.
        /// </summary>
        private TargetComponents selectedTarget;

        /// <summary>
        /// Seznam cílů reprezentující letadla.
        /// </summary>
        private Dictionary<AircraftIdentifier, TargetComponents> targets;

        /// <summary>
        /// Aktuální pozice myši
        /// </summary>
        private Point mousePosition;

		/// <summary>
		/// Zaměřený popisek. V případě překryvajicích se popipsků je na tento aplikovaný uživatelský vstup.
		/// </summary>
		private TargetLabel focusedLabel;

		/// <summary>
		/// Stav myši.
		/// </summary>
        private MouseState previousMouseState;

		/// <summary>
		/// Příznak určující zda uživatel kliknul na cíl (popisek).
		/// </summary>
        private bool clickedOnTarget;

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public bool IsLabelPressed { get; set; }

        /// <summary>
        /// Seznam zvýrazněných cílů.
        /// </summary>
        private Dictionary<string, TargetHighlight> highlightedTargets;

        /// <summary>
        /// Lock pro asynchroní vlákna.
        /// </summary>
        private readonly object Lock = new object();


        private IViewportProjection _mapProjection;

        private readonly AfisStripDataService _afisStripDataService;


		public TargetComponentsManager(IViewportProjection mapProjection, AfisStripDataService afisStripDataService)
        {
	        this._mapProjection = mapProjection;
	        this._afisStripDataService = afisStripDataService;
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public void Initialize()
        {
	        this.highlightedTargets = new Dictionary<string, TargetHighlight>();
	        this.targets = new Dictionary<AircraftIdentifier, TargetComponents>();
			this._afisStripDataService.HighlightedAircraftReceived += this._afisStripDataService_HighlightedAircraftsReceived;
        }

		private void _afisStripDataService_HighlightedAircraftsReceived(object sender, AfisStripDataService.AircraftHighlightEventArgs e)
		{
			lock (this.Lock) {
				this.highlightedTargets = e.ListOfAircraft;
				foreach (var item in this.targets.Values) {
					item.Color = this.GetColor(item.Aircraft, item);
				}
			}
		}

		/// <summary>
		/// Maximální stanovená výška (stopy).
		/// </summary>
		private int maxAltitude = int.MaxValue;

		/// <summary>
		/// Aktualizuje komponenty cíle, případně vytvoří nový pokud neexistuje.
		/// </summary>
		/// <param name="newSample">kolekce nových informací o letadlech.</param>
		public void UpdateTargets(Dictionary<AircraftIdentifier, Aircraft> newSample)
        {
	        foreach (var aircraft in newSample) {
		        if (this.targets.ContainsKey(aircraft.Key)) {
			        //Cíl ve sledované oblasti stále existuje => aktualizují se informace a překreslí se.
			        this.UpdateTarget(this.targets[aircraft.Key], aircraft.Value);
		        } else {
			        //Nový cíl začal existovat v naší oblasti => vytvoří se nová instance a vykreslí se.
			        var newTargetComponents = this.InitializeTarget(aircraft.Value);
			        this.targets.Add(aircraft.Key, newTargetComponents);
			        this.HideTargetComponentsAboveAltitude(newTargetComponents);
		        }
	        }

	        foreach (var target in this.targets.Reverse()) {

		        //Cíl ve oblasti už NEexistuje => smaže se a dál nebude zobrazován.
		        if (!newSample.ContainsKey(target.Key)) {
			        var targetToRemove = target.Value;
			        this.targets.Remove(target.Key);
			        if (this.selectedTarget == targetToRemove) {
				        //Cíl, který má být smazán, byl označen (nakliknut), označení se zruší.
				        this.focusedLabel = null;
				        this.selectedTarget = null;
			        }

			        targetToRemove.Dispose();
		        }
	        }
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void ChangeMaxAltitude(int maxAltitude)
		{
			this.maxAltitude = maxAltitude;
			foreach (var target in this.targets) {
				this.HideTargetComponentsAboveAltitude(target.Value);
			}
		}

		/// <summary>
        /// Vytvoří komponenty cíle
        /// </summary>
        /// <param name="aircraft">letadlo.</param>
        /// <returns>komponenty cíle.</returns>
        private TargetComponents InitializeTarget(Aircraft aircraft)
        {
            var target = new TargetComponents(this._mapProjection, aircraft, this.TrailDotsMaxCount, this.TrailDotsStandartCount, int.MaxValue, this.RealFixesCount, this.GetColor(aircraft, null));
            target.Clicked += this.Target_Clicked;
            target.Label.MouseLeftReleased += this.Label_MouseLeftReleased;
            target.Label.MouseLeftPressed += this.Label_MouseLeftPressed;
            return target;
        }

        /// <summary>
        /// Aktualizuje komponenty cíle reprezentující letadlo.
        /// </summary>
        /// <param name="aircraft">letadlo podle kterého se má aktualizovat cíl.</param>
        private void UpdateTarget(TargetComponents target, Aircraft aircraft)
        {
            var targetGroup = target;
            targetGroup.Aircraft = aircraft;
            targetGroup.Color = this.GetColor(aircraft, targetGroup);
            this.HideTargetComponentsAboveAltitude(targetGroup);
        }

		//------------------------------------------------------------------------------------------------------
		//------------------------------------------------------------------------------------------------------



		/// <summary>
		/// Schová všechny cíle nacházející se nad stanovenou výškou.
		/// </summary>
		/// <param name="targetComponents">cíle které mají být schovány.</param>
		private void HideTargetComponentsAboveAltitude(TargetComponents targetComponents)
		{
			//cíl je výš než je maximální výška => bude NEvidetelný.
			//cíl je níže než je maximální výška => bude viditelný.
			targetComponents.IsVisible = MathExtension.MetersToFeet(targetComponents.Aircraft.CurrentStateVector.RealFix.Altitude) <= this.maxAltitude;
		}

		/// <summary>
		/// Vrací barvu na základě atributů letadla
		/// </summary>
		/// <param name="aircraft">letadlo</param>
		/// <param name="targetComponents">komponenty reprezentující letadlo</param>
		/// <returns>Barva letadla.</returns>
		private Color GetColor(Aircraft aircraft, TargetComponents targetComponents)
        {
	        //Barva označeného letadla.
	        if (this.selectedTarget != null && targetComponents == this.selectedTarget) {
		        return Colors.SelectedAircraftColor;
	        }

	        lock (this.Lock) {
		        foreach (var item in this.highlightedTargets) {
			        //Vybere barvu zvýraznění podle dat, které přišli ze sítě.
			        if (aircraft.Id.Contains(item.Key)) {
				        switch (item.Value) {
					        case TargetHighlight.Alert:
						        return Colors.AircraftAlertColor;
					        case TargetHighlight.Notice1:
						        return Colors.AircraftNotice1Color;
					        case TargetHighlight.Notice2:
						        return Colors.AircraftNotice2Color;
					        default:
						        return this.GetUnhighlightedTargetColor(aircraft);
				        }
			        }
		        }
	        }

	        //Pokud není označené a ani zvýrazěné vrátí barvu na základě atributů.
	        return this.GetUnhighlightedTargetColor(aircraft);
        }

        /// <summary>
        /// Metoda vyvolána události při kliknutí na popisek.
        /// </summary>
        private void Target_Clicked(object sender, EventArgs e)
        {
            var oldSelectedTarget = this.selectedTarget;
            this.selectedTarget = ((TargetComponents)sender);

            if (oldSelectedTarget != null) {
                //Resetuje zobrazení letadla, které bylo předtím označeno
                this.UnSelectTarget(oldSelectedTarget);
            }

            //Nastaví vlastnosti nově označenému letadlu - změní barvu a zobrazí větší počet trail dotů.
            this.selectedTarget.Color = Colors.SelectedAircraftColor;
            this.selectedTarget.TrailDotsCount = this.TrailDotsMaxCount;
            this.selectedTarget.RealFixesCount = int.MaxValue;
			this.selectedTarget.UpdateTargetComponentsPosition();

            this.clickedOnTarget = true;
        }

        /// <summary>
        /// Vrací barvu nezvýrazěného letadla odpovídající jeho atributům.
        /// </summary>
        /// <param name="aircraft">letadlo</param>
        /// <returns>barvu odpovídající atributům letala</returns>
        private Color GetUnhighlightedTargetColor(Aircraft aircraft)
        {
	        if (aircraft.CurrentStateVector.OnGround.Value) {
		        //Barva pokud je letadlo na zemi
		        return Colors.AircraftOnGroundColor;
	        } else {
		        //Barva pokud je letadlo ve vzduchu
		        return Colors.AircraftInAirColor;
	        }
        }

        /// <summary>
        /// Handler vyvolaný při stlačení levého tlačítka myši.
        /// </summary>
        private void Label_MouseLeftPressed(object sender, EventArgs e)
        {
	        if (this.focusedLabel == null) {
		        this.focusedLabel = (TargetLabel) sender;
		        this.focusedLabel.IsFocused = true;
		        this.IsLabelPressed = true;
	        }
        }

        /// <summary>
		/// Handler vyvolaný při uvolnění levého tlačítka myši.
		/// </summary>
        private void Label_MouseLeftReleased(object sender, EventArgs e)
        {
            this.IsLabelPressed = false;
            this.focusedLabel.IsFocused = false;
            this.focusedLabel = null;
        }

		/// <summary>
		/// Aktualizuje state pro myš.
		/// </summary>
		/// <param name="mouseState">stav uživatelské myši.</param>
		public void UpdateMouseState(MouseState mouseState)
		{
			this.clickedOnTarget = false;

			foreach (var target in this.targets.Values) {
				target.UpdateMouseState(mouseState);
			}

			if (mouseState.LeftButton == ButtonState.Pressed && this.previousMouseState.LeftButton == ButtonState.Released) {
				this.mousePosition = mouseState.Position;
			}

			if (this.previousMouseState.LeftButton == ButtonState.Pressed &&
			    mouseState.LeftButton == ButtonState.Released &&
			    this.mousePosition == mouseState.Position) {
				//uživatel pustil levé tlačítko myši.
				if (!this.clickedOnTarget) {
					if (this.selectedTarget != null) {
						var target = this.selectedTarget;
						this.selectedTarget = null;
						this.UnSelectTarget(target);
					}
				}
			}

			this.previousMouseState = mouseState;
		}

		/// <summary>
        /// Zruší výběr cíle.
        /// </summary>
        /// <param name="target"></param>
        private void UnSelectTarget(TargetComponents target)
        {
            target.Color = this.GetColor(target.Aircraft, target);
            target.TrailDotsCount = this.TrailDotsStandartCount;
            target.RealFixesCount = this.RealFixesCount;
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {
            foreach (var target in this.targets.Values) {
	            target.Draw(spriteBatch, drawBatch);
            }
        }
	}
}
