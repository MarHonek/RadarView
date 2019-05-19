using System;
using System.Diagnostics;
using System.Linq;
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
	/// Definuje popisek cíle.
	/// </summary>
	/// <remarks>
	/// Optimalizace výkonu!
	/// Třídá je navrhnuta tak aby probíhalo co nejméně výpočtů při vykreslování. tzn. Get metoda pouze vrací hodnoty.
	/// </remarks>
	public class TargetLabel : RenderObject, IRenderable
	{
		/// <summary>
		/// Mapová projekce
		/// </summary>
		private IViewportProjection projection;

		/// <summary>
		/// Vlastnosti určující zda došlo k uvolnění prostředků.
		/// </summary>
		private bool disposed;

		/// <summary>
		/// Jednotka rychlosti (uzly nebo km/h)
		/// </summary>
		private string GroundSpeedUnit;

		/// <summary>
		/// Objekt který reprezentuje aktuílní polohu letadla
		/// </summary>
		private Model.Render.Targets.Target target;

		/// <summary>
		/// SpriteFont pro vykreslení textu
		/// </summary>
		private SpriteFont spriteFont;

		/// <summary>
		/// Vlastnost, která určuje kde se vůči targetu zobrazuje popisek
		/// </summary>
		private Vector2 TargetOffset = new Vector2(-30, -120);

		/// <summary>
		/// Textura reprezentující manévr letadla
		/// </summary>
		private Texture2D maneuverTexture;

		/// <summary>
		/// Letadlo pro které se zobrazují informace v popisku
		/// </summary>
		private Aircraft _aircraft;

		public Aircraft Aircraft
		{
			set
			{
				this._aircraft = value;
				this.UpdateLabelContent();
			}
		}

		/// <summary>
		/// Text popisku v nerozkliknutelném stavu
		/// </summary>
		private string content;

		/// <summary>
		/// Vlastnost, určující zda jsou zobrazené dodatečné informace
		/// </summary>
		private bool secondaryContentVisible = false;

		/// <summary>
		/// Pozice levého horního rohu popisku
		/// </summary>
		private Vector2 _position;
		public Vector2 Position
		{
			get { return this._position; }
		}

		/// <summary>
		/// Pozice ikony zobrazující manévr letadla
		/// </summary>
		private Vector2 maneuverIconPosition;

		/// <summary>
		/// Pozice ikony manévru od levého horního rohu obdelníku obsahující text 
		/// </summary>
		private Vector2 maneuverIconOffset;

		/// <summary>
		/// Velikost ikony manévru
		/// </summary>
		private Vector2 maneiverIconSize;

		/// <summary>
		/// Rozměr popisku
		/// </summary>
		private Vector2 _size;
		public Vector2 Size
		{
			get { return this._size; }
		}

		/// <summary>
		/// Rozměry obdelníku zobrazující základní informace
		/// </summary>
		private Vector2 _primaryContentSize;

		public Vector2 PrimaryContentSize
		{
			get { return this._primaryContentSize; }
		}

		/// <summary>
		/// Velikost jednoho znaku
		/// </summary>
		private Vector2 charSize;

		/// <summary>
		/// Počet zobrazovaných číslic výšky
		/// </summary>
		private readonly int AltitudeDigitsCount = Settings.Default.AircraftLabelAltitudeDigitsCount;

		/// <summary>
		/// Počet zobrazovaných číslic rychlosti.
		/// </summary>
		private readonly int GroundSpeedDigitsCount = Settings.Default.AircraftLabelGroundSpeedDigitsCount;

		/// <summary>
		/// Počet mezer mezi výškou a rychlostí
		/// </summary>
		private readonly int AltitudeSpeedSpaces = Settings.Default.AircraftLabelAltSpeedSpacesCount;

		/// <summary>
		/// Počet mezer mezi sloupci
		/// </summary>
		private readonly int ColumnSpacesCount = Settings.Default.AircraftLabelColumnSpacesCount;

		/// <summary>
		/// Pole obsahující řádky a sloupce textu
		/// </summary>
		private string[,] lines;

		/// <summary>
		/// Předchozí stav myši.
		/// </summary>
		private MouseState previousMouseState;

		/// <summary>
		/// Pozice bodu, kde uživatel zmáčknul levé tlačítko.
		/// </summary>
		private Point? mousePressedPosition;

		/// <summary>
		/// Vlastnost, jestli je tento popisek zaměřen.
		/// V případě překrývajících se popisku má jen jeden popisek
		/// nastevenou tuto vlastnost na true.
		/// </summary>
		public bool IsFocused { get; set; }

		/// <summary>
		/// Událost vyvolána při kliknutí na popisek.
		/// </summary>
		public event EventHandler MouseLeftClick;

		/// <summary>
		/// Událost vyvolána při stlačení levého tlačítka.
		/// </summary>
		public event EventHandler MouseLeftPressed;

		/// <summary>
		/// Událost vyvolána při uvolnění levého tlačítka.
		/// </summary>
		public event EventHandler MouseLeftReleased;

		/// <summary>
		/// Událost při změny polohy kursoru při stlačeném tlačítku.
		/// </summary>
		public event EventHandler MouseLeftPressedMove;

		/// <summary>
		/// Inicializuje instanci třídy TargetLabel
		/// </summary>
		/// <param name="target">cíl reprezentující aktuální polohu letadla</param>
		/// <param name="aircraft">letadlo</param>
		/// <param name="projection">mapová projekce</param>
		/// <param name="color">barva popisku</param>
		public TargetLabel(Model.Render.Targets.Target target, Aircraft aircraft, IViewportProjection projection, Color color) : base(color)
		{
			this.projection = projection;
			this.projection.ViewportChanged += this.LocationChanged;
			this.GroundSpeedUnit = Settings.Default.SpeedUnit;
			this.lines = new string[4, 2];
			this.spriteFont = Renderer.spriteFont;
			this.charSize = this.spriteFont.MeasureString("0");
			this.maneiverIconSize = this.charSize;
			this.Aircraft = aircraft;
			this.target = target;
			this.target.LocationChanged += this.LocationChanged;

			//Nastaví zobrazení základních informací
			this.content = this.GetPrimaryContent();
			this._size = this.spriteFont.MeasureString(this.content);
			this._primaryContentSize = this.spriteFont.MeasureString(this.lines[2, 0]);
			this.SetManeuverIconOffset(this.secondaryContentVisible);
			this.LocationChanged(this, null);
			this.SetManeuverIconPosition();
		}

		/// <summary>
		/// Aktualizuje text popisku na základe atributu letadla
		/// </summary>
		private void UpdateLabelContent()
		{
			var dataSources = "";
			foreach (var dataSource in this._aircraft.ParticipatingDataSources) {
				dataSources += dataSource.ToString() + " ";
			}

			var lastRealFixTime = this._aircraft.RealFixes.LastOrDefault().DateTime;
			var altitude = (MathExtension.MetersToFeet(this._aircraft.CurrentStateVector.RealFix.Altitude) / 100).ToString(new string('0', this.AltitudeDigitsCount));
			this.maneuverTexture = this.GetManeuverIcon(this._aircraft.CurrentStateVector.Maneuver);

			var cities = "";
			if (!string.IsNullOrWhiteSpace(this._aircraft.Info.OriginAirportCode) || !string.IsNullOrWhiteSpace(this._aircraft.Info.DestinationAirportCode)) {
				cities = this._aircraft.Info.OriginAirportCode + " -> " + this._aircraft.Info.DestinationAirportCode;
			}

			this.lines[0, 0] = this._aircraft.Info.CompetitionName ?? (this._aircraft.Id.Squawk != null ? "sqw " + this._aircraft.Id.Squawk : "--");
			this.lines[1, 0] = this._aircraft.Id.CallSign?.Trim() ?? this._aircraft.Id.Address;
			this.lines[2, 0] = altitude + new string(' ', this.AltitudeSpeedSpaces) +
			                   (this.GetGroundSpeedInConfiguredUnit(this._aircraft.CurrentStateVector.GroundSpeed.Value) / 10).ToString(new string('0', this.GroundSpeedDigitsCount));
			this.lines[3, 0] = this._aircraft.CurrentStateVector.VerticalSpeed.Value.ToString("+0.0;-0.0;0.0") + "m/s";

			this.lines[0, 1] = this._aircraft.Id.IataFlightNumber?.ToString();
			this.lines[1, 1] = cities;
			this.lines[2, 1] = this._aircraft.Info.Model ?? "";
			Debug.WriteLine(lastRealFixTime);
			this.lines[3, 1] = dataSources + string.Format("({0}s)", (int) (DateTime.UtcNow - lastRealFixTime).TotalSeconds);

			this.AddSpacesToAlign();

			if (this.secondaryContentVisible) {
				this.content = this.GetWholeContent();
			} else {
				this.content = this.GetPrimaryContent();
			}

			this.SetManeuverIconOffset(this.secondaryContentVisible);
		}

		/// <summary>
		/// Vrací text obsahující základní informace
		/// </summary>
		/// <returns>text obsahující základní informace</returns>
		private string GetPrimaryContent()
		{
			return this.lines[1, 0] + "\n" + this.lines[2, 0];
		}

		/// <summary>
		/// Vrátí text obsahující dodatečné informace
		/// </summary>
		/// <returns>text obsahující dodatečné informace</returns>
		private string GetWholeContent()
		{
			var columnSpace = new string(' ', this.ColumnSpacesCount);
			return this.lines[0, 0] + columnSpace + this.lines[0, 1] + "\n" +
			       this.lines[1, 0] + columnSpace + this.lines[1, 1] + "\n" +
			       this.lines[2, 0] + columnSpace + this.lines[2, 1] + "\n" +
			       this.lines[3, 0] + columnSpace + this.lines[3, 1];
		}

		/// <summary>
		/// Přídá mezery na každém řádku pro zarovnání textu doprava.
		/// </summary>
		private void AddSpacesToAlign()
		{
			var max = int.MinValue;
			for (var i = 0; i < this.lines.GetLength(0); i++) {
				if (this.lines[i, 0].Length > max) {
					max = this.lines[i, 0].Length;
				}
			}

			for (var i = 0; i < this.lines.GetLength(0); i++) {
				this.lines[i, 0] = new string(' ', max - this.lines[i, 0].Length) + this.lines[i, 0];
			}
		}

		/// <summary>
		/// Vrací rychlost v jednotce nastavené v konfiguračním souboru
		/// </summary>
		/// <param name="groundSpeed">rychlost v metrech za sekundu</param>
		/// <returns>rychlost v jednotce, dle hodnoty config. souboru</returns>
		private float GetGroundSpeedInConfiguredUnit(float groundSpeed)
		{
			if (this.GroundSpeedUnit == "Knots") {
				groundSpeed = MathExtension.MetersPerSecondsToKnots(groundSpeed);
			} else {
				groundSpeed = MathExtension.MetersPerSecondToKilometersPerHour(groundSpeed);
			}

			return groundSpeed;
		}

		/// <summary>
		/// Změní stav popisku. Zobrazí dodatečné informace pokud jsou zobrazeny jen základní (a naopak)
		/// </summary>
		public void ChangeLabelState()
		{
			this.secondaryContentVisible = !this.secondaryContentVisible;
			this.UpdateLabelContent();
			this.SetManeuverIconOffset(this.secondaryContentVisible);
			this._size = this.spriteFont.MeasureString(this.content);
			this.UpdateLabelPosition();
		}

		/// <summary>
		/// Nastaví pozici ikony indikující manévr
		/// </summary>
		private void SetManeuverIconPosition()
		{
			this.maneuverIconPosition = this.Position + this.maneuverIconOffset;
		}

		/// <summary>
		/// Nastaví offset pozice ikony v závislosti zda jsou zobrazeny dodatečné informace
		/// </summary>
		/// <param name="secondaryContentVisible">Příznak, zda jsou zobrazeny další informace.</param>
		private void SetManeuverIconOffset(bool secondaryContentVisible)
		{
			var iconLeftOffset = this.lines[2, 0].Length - (this.AltitudeSpeedSpaces + this.GroundSpeedDigitsCount);
			this.maneuverIconOffset = new Vector2(this.charSize.X * iconLeftOffset, secondaryContentVisible ? 2 * this.charSize.Y : this.charSize.Y);
		}

		/// <summary>
		/// Vrací texturu odpovídající zadanému manévru
		/// </summary>
		/// <param name="maneuver">mánevr, který určuje jaká ikona se má zobrazit</param>
		/// <returns>texturu odpovídající manéveru.</returns>
		private Texture2D GetManeuverIcon(AircraftManeuver maneuver)
		{
			switch (maneuver) {
				case AircraftManeuver.Climb:
					return Renderer.NorthEastArrow;
				case AircraftManeuver.Descent:
					return Renderer.SouthEastArrow;
				case AircraftManeuver.Horizon:
					return Renderer.HorizonTexture;
				case AircraftManeuver.Circling:
					return Renderer.ClockwiseTexture;
				default:
					return Renderer.HorizonTexture;
			}
		}

		/// <summary>
		/// Peroidicky aktualizuje stav myši.
		/// </summary>
		/// <param name="mouseState">stav myši</param>
		public void UpdateMouseState(MouseState mouseState)
		{
			this.CheckMouseInput(mouseState);
			this.previousMouseState = mouseState;
		}

		/// <summary>
		/// Zkontrole uživatelský vstup (myš)
		/// </summary>
		/// <param name="currentMouseState"></param>
		private void CheckMouseInput(MouseState currentMouseState)
		{
			if (currentMouseState.LeftButton == ButtonState.Pressed) {
				if (this.previousMouseState.LeftButton == ButtonState.Released) {
					//Pokud je aktuálně zmáčknuté tlačítko a minulou kontrolu NEbylo
					this.CheckLeftButtonPress(currentMouseState);
				} else {
					//Pokud je tlačítko zmáčknuté a bylo i v předchozí kontrole => zkontroluje posunutí popisku
					this.CheckLabelMove(currentMouseState);
				}
			} else if (currentMouseState.LeftButton == ButtonState.Released && this.previousMouseState.LeftButton == ButtonState.Pressed) {
				//Pokud není tlačítko zmáčknuté a bylo při minulé kontrole bylo.
				this.CheckLabelClickOrRelease(currentMouseState);
				this.mousePressedPosition = null;
			}
		}

		/// <summary>
		/// Zkontroluje zda je levé tlačítko zmačknuté unvitř labelu
		/// </summary>
		/// <param name="currentMouseState">stav myši</param>
		private void CheckLeftButtonPress(MouseState currentMouseState)
		{
			if (this.IsOverLabel(currentMouseState.Position)) {
				this.MouseLeftPressed?.Invoke(this, new EventArgs());
				this.mousePressedPosition = currentMouseState.Position;
			}
		}

		/// <summary>
		/// Zkontroluje zda uživatel chce posunout popisek
		/// </summary>
		/// <param name="currentMouseState">stav myši (pressed, released)</param>
		private void CheckLabelMove(MouseState currentMouseState)
		{
			//Pokud se popisky překryvají, zjistí zda je tento zaměřený (focused). Pokud je umožní s ním pohnout
			if (this.mousePressedPosition != null && this.IsFocused) {
				//Posune popisek o vektor daný pozicí myši.
				var translationVector = (currentMouseState.Position - this.previousMouseState.Position).ToVector2();
				this.TargetOffset += translationVector;
				this.UpdateLabelPosition();
				this.MouseLeftPressedMove?.Invoke(this, new EventArgs());
			}
		}

		/// <summary>
		/// Zkontroluje uživatel kliknul na popisek.
		/// </summary>
		/// <param name="currentMouseState">stav myši</param>
		private void CheckLabelClickOrRelease(MouseState currentMouseState)
		{

			//Zkontroluje jestli je levé tlačitko myši zmáčknuté a zároveň se pozice nezměnila => vyhodnotí jako kliknutí
			if (this.IsFocused) {
				if (this.mousePressedPosition == currentMouseState.Position) {
					this.ChangeLabelState();
					this.MouseLeftClick?.Invoke(this, new EventArgs());
				}

				this.MouseLeftReleased?.Invoke(this, new EventArgs());
			}
		}

		/// <summary>
		/// Aktualizuje pozici popisku v závislosti jaké informace se zobrazují
		/// </summary>
		private void UpdateLabelPosition()
		{
			if (this.secondaryContentVisible) {
				//Pokud jsou zobrazeny základní informace, posune text o řádek výš.
				this._position = new Vector2(this.target.Point.X + this.TargetOffset.X, this.target.Point.Y + this.TargetOffset.Y - this.maneiverIconSize.Y);
			} else {
				this._position = this.target.Point + this.TargetOffset;
			}

			this.SetManeuverIconPosition();
		}

		private void LocationChanged(object sender, EventArgs e)
		{
			this.UpdateLabelPosition();
		}

		/// <summary>
		/// Uvolní přostředky
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (this.disposed) {
				return;
			}

			if (disposing) {
				this.projection.ViewportChanged -= this.LocationChanged;
				this.target.LocationChanged -= this.LocationChanged;
			}

			this.disposed = true;

			base.Dispose(disposing);
		}

		/// <summary>
		/// Zkotroluje zda je bod uvnitř labelu
		/// </summary>
		/// <param name="mousePosition">bod kursoru</param>
		/// <returns>true pokud je uvnitř labelu, jinak false</returns>
		private bool IsOverLabel(Point mousePosition)
		{
			return new Rectangle(this.Position.ToPoint(), this.Size.ToPoint()).Contains(mousePosition);
		}

		public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
		{
			spriteBatch.DrawString(this.spriteFont, this.content, this._position, this.Color);
			spriteBatch.Draw(this.maneuverTexture, new Rectangle(this.maneuverIconPosition.ToPoint(), new Point((int) this.maneiverIconSize.Y)), this.Color);

		}
	}
}
