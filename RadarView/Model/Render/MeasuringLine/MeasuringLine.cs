using System;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Abstract;
using RadarView.Properties;
using RadarView.Utils;

namespace RadarView.Model.Render.MeasuringLine
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
    public class MeasuringLine : RenderObject, IMeasuringLine
    {
        /// <summary>
        /// Přechozí stav myši.
        /// </summary>
        private MouseState previousMouseState;

        /// <summary>
        /// Pozice, kde uživatel zmáčknul tlačítko.
        /// </summary>
        private Point mouseButtonPressPosition;

        /// <summary>
        /// Pozice počátečního bodu čáry
        /// </summary>
        private Vector2 startLinePosition;

        /// <summary>
        /// Pozice koncového bodu čáry.
        /// </summary>
        private Vector2 endLinePosition;

        /// <summary>
        /// Geografická poloha počátečního bodu čáry.
        /// </summary>
        private Location startLineLocation;

        /// <summary>
        /// Geografická poloha počátečního bodu čáry.
        /// </summary>
        private Location endLineLocation;

        /// <summary>
        /// Mapová projekce. Přepočítá geografické souřadnice na body viewportu.
        /// </summary>
        private IViewportProjection projector;

        /// <summary>
        /// Šířka měřící čáry.
        /// </summary>
        private readonly float MeasuringLineThickness = Settings.Default.MeasuringLineThickness;

		/// <summary>
		/// Barva měřící čáry.
		/// </summary>
        private readonly Color MeasuringLineColor = ColorUtil.HexToColor(Settings.Default.MeasuringLineColor);

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public event EventHandler<MeasuringLineEventArgs> StateChanged;


		public MeasuringLine(IViewportProjection projector)
		{
			this.Color = this.MeasuringLineColor;
			this.Pen.Width = this.MeasuringLineThickness;
            this.projector = projector;
            this.startLinePosition = Vector2.Zero;
            this.endLinePosition = Vector2.Zero;
            projector.ViewportChanged += this.Projector_ViewportChanged;
        }

        private void Projector_ViewportChanged(object sender, EventArgs e)
        {
            //Pokud se změní viewport, přepočítá body pro vykreslení čáry.
           if (this.startLineLocation != null && this.endLineLocation != null)
            {
                this.startLinePosition = this.projector.LocationToViewportPoint(this.startLineLocation);
                this.endLinePosition = this.projector.LocationToViewportPoint(this.endLineLocation);
            }
        }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
        public void UpdateMouseState(MouseState mouseState)
        {
            this.CheckMouseInput(mouseState);
            this.previousMouseState = mouseState;
        }

        /// <summary>
        /// Zkontroluje uživatelský vstup
        /// </summary>
        /// <param name="mouseState"></param>
        private void CheckMouseInput(MouseState mouseState)
        {
	        if (mouseState.RightButton == ButtonState.Pressed &&
	            this.previousMouseState.RightButton == ButtonState.Released) {
		        //Uživatel zmáčknul pravé tlačítko
		        this.RightButtonPress(mouseState);
	        }

	        if (mouseState.RightButton == ButtonState.Released &&
	            this.previousMouseState.RightButton == ButtonState.Pressed) {
		        //Uživatel uvolnil pravé tlačitko.
		        this.RightButtonRelease(mouseState);
	        }

	        if (mouseState.RightButton == ButtonState.Pressed &&
	            mouseState.Position != this.previousMouseState.Position) {
		        //Uživatel posunul kurzor.
		        this.MouseMoveWithPressedRightButton(mouseState);
	        }
        }

        /// <summary>
        /// Vypočítá vzdálenost mezi koncovými body měřící čáry a zobrazí popisek.
        /// </summary>
        /// <param name="mouseState">stav myši</param>
        private void RightButtonRelease(MouseState mouseState)
        {
	        if (mouseState.Position == this.mouseButtonPressPosition) {
				this.StateChanged?.Invoke(this, new MeasuringLineEventArgs(null, null));
	        }
        }

        /// <summary>
        /// Určí počáteční bod měřící čáry.
        /// </summary>
        /// <param name="mouseState">stav myši</param>
        private void RightButtonPress(MouseState mouseState)
        {
            this.endLineLocation = null;
            this.startLineLocation = null;

            this.mouseButtonPressPosition = mouseState.Position;
            this.previousMouseState = mouseState;
            this.startLinePosition = mouseState.Position.ToVector2();
            this.endLinePosition = this.startLinePosition;

            this.StateChanged?.Invoke(this, new MeasuringLineEventArgs(null, null));
		}

		private void MouseMoveWithPressedRightButton(MouseState mouseState) 
		{
			this.endLinePosition = mouseState.Position.ToVector2();

			//Přepočítá body viewportu na geografické souřadnice.
			this.startLineLocation = this.projector.ViewportPointToLocation(this.startLinePosition);
			this.endLineLocation = this.projector.ViewportPointToLocation(this.endLinePosition);

			//Vypočítá vzdálenost
			var distance = this.startLineLocation.GetDistanceTo(this.endLineLocation);

			//Určí kurz
			var course = MathExtension.DetermineAzimuth(this.startLineLocation, this.endLineLocation);

			this.StateChanged?.Invoke(this, new MeasuringLineEventArgs((int)course, (int)distance));
		}

		public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {
            drawBatch.DrawLine(this.Pen, this.startLinePosition, this.endLinePosition);             
        }
    }

	/// <summary>
	/// Argumenty k události meřící linie.
	/// </summary>
	public class MeasuringLineEventArgs : EventArgs
	{
		/// <summary>
		/// Kurz měřící linie (stupně).
		/// </summary>
		public int? Course { get; set; }

		/// <summary>
		/// Délka meřící linie (metry).
		/// </summary>
		public int? Length { get; set; }


		public MeasuringLineEventArgs(int? course, int? length)
		{
			this.Course = course;
			this.Length = length;
		}
	}
}
