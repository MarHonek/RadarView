using System;
using LilyPath;
using Microsoft.Xna.Framework;

namespace RadarView.Model.Render.Abstract
{
    /// <summary>
    /// Reprezentuje objekt, který bude vykreslen rendererem.
    /// </summary>
    /// <remarks>
    /// Optimalizace výkonu!
    /// Třídá je navrhnuta tak aby probíhalo co nejméně výpočtů při vykreslování. tzn. Get metoda pouze vrací hodnoty.
    /// </remarks>
    public abstract class RenderObject : IDisposable
    {
        /// <summary>
        /// atribut určující zda došlo k uvolnění prostředků.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// Výsledná barv po úpravě (viditelnosti)
        /// </summary>
        private Color _color;

        /// <summary>
        /// Standartní barva s maximální viditelností
        /// </summary>
        private Color _standardColor;

        /// <summary>
        /// Šířka linky nevyplněného objektu
        /// </summary>
        private float _strokeThickness = 1f;
        public float StrokeThickness
        {
	        get { return this._strokeThickness; }
	        set
	        {
		        this._strokeThickness = value;
		        this.ChangeDrawingResources(this._color);
	        }
        }

        /// <summary>
        /// Viditelnost objektu.
        /// </summary>
        private bool _isVisible = true;
        public bool IsVisible
        {
	        get { return this._isVisible; }
	        set
	        {
		        this._isVisible = value;
		        if (this._isVisible) {
			        this._opacity = this.opacityOfHiddenObject;
		        } else {
			        this.opacityOfHiddenObject = this._opacity;
			        this._opacity = 0f;
		        }

		        this.ChangeOpacityColor(this._opacity);
	        }
        }

        /// <summary>
        /// Hodnota průhlednosti před změnou viditelnosti.
        /// </summary>
        private float opacityOfHiddenObject = 100f;

        /// <summary>
        /// Průhlednost objektu. Hodnota v intervalu [0-100]
        /// </summary>
        private float _opacity = 100f;
        public float Opacity
        {
	        get { return this._opacity; }
	        set
	        {
		        this._opacity = value;
		        this.opacityOfHiddenObject = this._opacity;
		        this.ChangeOpacityColor(this._opacity);
	        }
        }

        /// <summary>
        /// Barva objektu.
        /// Pruhlednost barvy je dána vlastností Opacity.
        /// Pokud je vlastnost IsVisible nastavena na false, objekt je vykreslen s maximální průhledností.
        /// </summary>      
        public Color Color
        {
	        get { return this._color; }
	        set
	        {
		        this._standardColor = value;
		        var newColor = this._standardColor * (this._opacity / 100);

		        //Pokud se barva liší od předchozí, změní ji tj. vytvoří nový objekt Pen a Brush.
		        if (this.Pen == null || this.Brush == null || newColor != this._color) {
			        this.ChangeDrawingResources(newColor);
		        }

		        this._color = newColor;
	        }
        }


        /// <summary>
        /// Pero pro vykreslení čar
        /// </summary>
        public Pen Pen { get; private set; }

        /// <summary>
        /// Štětec pro vykreslení vyplněných tvarů
        /// </summary>
        public Brush Brush { get; private set; }

        protected RenderObject()
        {
        }

        protected RenderObject(Color color)
        {
            this.Color = color;
        }

        protected RenderObject(Color color, float strokeThickness)
        {
            this._strokeThickness = strokeThickness;
            this.Color = color;
        }


        //Třídy Pen a Brush neumožňují změnu barvy. Je nutné znovu vytvořit objekt.
        private void ChangeDrawingResources(Color color)
        {
            this.Pen?.Dispose();
            this.Brush?.Dispose();
            this.Pen = new Pen(color, this._strokeThickness);
            this.Brush = new SolidColorBrush(color);
        }

		/// <summary>
		/// Změní barvu podle zadané průhlednosti.
		/// </summary>
		/// <param name="opacity">průhlednost v procentech -[0-100]%.</param>
		private void ChangeOpacityColor(float opacity) 
		{
			this._color = this._standardColor * (opacity / 100);
			this.ChangeDrawingResources(this._color);
		}

        /// <summary>
        /// Uvolní prostředky
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
	        if (this.disposed) {
		        return;
	        }

	        if (disposing) {
		        this.Pen?.Dispose();
		        this.Brush?.Dispose();
	        }

	        this.disposed = true;
        }
    }
}
