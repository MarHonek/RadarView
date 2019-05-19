using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using RadarView.Model.DataService.Weather.PrecipitationRadarDataService;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Background.BackgroundManager;
using RadarView.Model.Render.Background.PrecipitationRadar;
using RadarView.Model.Render.Controller;
using RadarView.Model.Render.MeasuringLine;
using RadarView.Model.Render.Targets;
using RadarView.Model.Service.Config;
using RadarView.Model.Service.SessionContext;
using RadarView.Properties;
using RadarView.Utils;
using Point = System.Windows.Point;

namespace RadarView.Model.Render
{
    /// <summary>
    /// Třída starající se o vykreslování.
    /// </summary>
    public class Renderer : WpfGame, IRenderer
    {
        #region Monogame attributes
        private IGraphicsDeviceService graphicsDeviceManager;
        private WpfKeyboard keyboard;
        private WpfMouse mouse;

        private SpriteBatch spriteBatch;
        private DrawBatch drawBatch;
        #endregion

        #region Textures
        public static SpriteFont spriteFont;

        public static Texture2D Rectangle;
        public static Texture2D Circle;
        public static Texture2D Triangle;
        public static Texture2D FilledRectangle;
        public static Texture2D Star;
        public static Texture2D Cross;

        public static Texture2D NorthEastArrow;
        public static Texture2D ClockwiseTexture;
        public static Texture2D SouthEastArrow;
        public static Texture2D HorizonTexture;
        #endregion

        #region Constants
        /// <summary>
        /// Počet FPS.
        /// </summary>
        private static readonly int Fps = Settings.Default.ViewRenderFPS;

        /// <summary>
        /// Hodnota změny zoom levelu.
        /// </summary>
        private static readonly float ZoomIncreaseValue = Settings.Default.ViewZoomLevelChange;
        #endregion

        #region Input variables
		/// <summary>
		/// Předchozí hodnota kolečka myši.
		/// </summary>
        private int previousMouseWheelValue = 0;

		/// <summary>
		/// Předchozí pozice myši.
		/// </summary>
        private Microsoft.Xna.Framework.Point previousMousePosition;
		#endregion

		#region DependencyInjection
		private readonly IRendererController _rendererController;

		private readonly ISessionContext _sessionContext;

		private readonly IViewportProjection _mapProjection;

		private readonly IBackgroundManager _backgroundManager;

		private readonly IPrecipitationRadarDataService _precipitationRadarDataService;

		private readonly IBackgroundPrecipitationRadar _backgroundPrecipitationRadar;

		private readonly ITargetComponentsManager _targetComponentsManager;

		private readonly IMeasuringLine _measuringLine;
		#endregion

		private int fps;

		/// <summary>
		/// Atribut určující zda bylo okno aktivní.
		/// Slouží k zabránění nepříjemných skoků.
		/// </summary>
		private bool wasActive = true;

		public Renderer(ISessionContext sessionContext,
						IRendererController rendererController, 
						IViewportProjection mapProjection,
						IBackgroundManager backgroundManager,
						IPrecipitationRadarDataService precipitationRadarDataService,
						IBackgroundPrecipitationRadar backgroundPrecipitationRadar,
						ITargetComponentsManager targetComponentsManager,
						IMeasuringLine measuringLine)
        {
	        this._sessionContext = sessionContext;
	        this._rendererController = rendererController;
	        this._mapProjection = mapProjection;
	        this._backgroundManager = backgroundManager;
	        this._measuringLine = measuringLine;
	        this._precipitationRadarDataService = precipitationRadarDataService;
	        this._backgroundPrecipitationRadar = backgroundPrecipitationRadar;
	        this._targetComponentsManager = targetComponentsManager;
        }

		/// <summary>
		/// Inicializuje prostředky
		/// </summary>
		protected override void Initialize()
		{
			this.graphicsDeviceManager = new WpfGraphicsDeviceService(this);

			base.Initialize();

			this.TargetElapsedTime = TimeSpan.FromSeconds(1d / Fps);
			this.keyboard = new WpfKeyboard(this);
			this.mouse = new WpfMouse(this);
			this.drawBatch = new DrawBatch(this.GraphicsDevice);
			this.spriteBatch = new SpriteBatch(this.GraphicsDevice);

			#region Texture Loading
			spriteFont = this.Content.Load<SpriteFont>("DefaultFont");
			Rectangle = this.Content.Load<Texture2D>("Rectangle");
			Circle = this.Content.Load<Texture2D>("Circle");
			Triangle = this.Content.Load<Texture2D>("Triangle");
			NorthEastArrow = this.Content.Load<Texture2D>("North_east_arrow");
			ClockwiseTexture = this.Content.Load<Texture2D>("Anticlockwise_arrow");
			SouthEastArrow = this.Content.Load<Texture2D>("South_east_arrow");
			HorizonTexture = this.Content.Load<Texture2D>("Equals");
			Star = this.Content.Load<Texture2D>("Letter_star");
			FilledRectangle = this.Content.Load<Texture2D>("Filled_rectangle");
			Cross = this.Content.Load<Texture2D>("Cross");

			this.LoadMapTextures(this.GraphicsDevice);
			#endregion

			this._mapProjection.Center = this._sessionContext.DefaultCenter;
			this._rendererController.SamplesReceived += this._rendererController_SamplesReceived;
			this._precipitationRadarDataService.ImageWasDownloaded += this._precipitationRadarDataService_ImageWasDownloaded;

			this.InitializeComponents();
		}

		/// <summary>
		/// Událost vyvolána pokud byl stažen nový radarový snímek srážek.
		/// </summary>
		private void _precipitationRadarDataService_ImageWasDownloaded(object sender, EventArgs e)
		{			
			var imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "PrecipitationRadar.png");
			using (var fileStream = new FileStream(imagePath, FileMode.Open)) {
				this._backgroundPrecipitationRadar.RadarTexture = Texture2D.FromStream(this.GraphicsDevice, fileStream);

			}
		}

		/// <summary>
		/// Inicializuje komponenty vykreslování.
		/// </summary>
		private void InitializeComponents()
		{
			this._backgroundManager.Initialize();
			this._targetComponentsManager.Initialize();
		}


		private void _rendererController_SamplesReceived(object sender, RendererControllerEventArgs e)
		{
			this._targetComponentsManager.UpdateTargets(e.Samples);
		}

		/// <summary>
		/// Načte mapové podklady.
		/// </summary>
		private void LoadMapTextures(GraphicsDevice graphicsDevice)
		{
			var mapLayers = this._sessionContext.MapLayerCollection.Layers;
			var airport = this._sessionContext.CurrentAirport;

			foreach (var mapLayer in mapLayers) {
				mapLayer.Textures = new Dictionary<int, Texture2D>();
				try {
					foreach (var mapLevel in mapLayer.BoundingBoxes.Keys) {

						var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content",
							MapUtils.GetMapTextureName(mapLayer.ImageName, airport.IcaoIdent, mapLevel));
						var pngExists = File.Exists(path + ".png");
						if (pngExists) {
							path += ".png";
						} else {
							path += ".jpeg";
						}

						using (var fileStream = new FileStream(path, FileMode.Open)) {
							mapLayer.Textures[mapLevel] = Texture2D.FromStream(graphicsDevice, fileStream);
						}
					}
				} catch (Exception) {
					mapLayer.Renderable = false;
					Debug.WriteLine("Chyba při načtení mapových podkladů:" + mapLayer.Name);
					System.Windows.MessageBox.Show("Nepodařilo se načíst mapové podklady: " +  mapLayer.Name);
				}
			}
		}

		/// <summary>
		/// Metoda zavolána při změně velikosti okna
		/// </summary>
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            var size = sizeInfo.NewSize;
            var previousSize = this._mapProjection.RenderSize;
            this._mapProjection.RenderSize = new Vector2((float)size.Width, (float)size.Height);
            if (previousSize == Vector2.Zero) {
				this._mapProjection.Initialize(this._sessionContext.DefaultCenter);
            }
        }
		
        /// <summary>
        /// Periodicky kontroluje uživatelský vstup
        /// </summary>
        protected override void Update(GameTime time)
        {
            var mouseState = this.mouse.GetState();

			//Umožňuje vyvolávat eventy pro klasické WPF GUI komponenty.
			this.mouse.CaptureMouseWithin = false;
            var keyboardState = this.keyboard.GetState();

            var fps  = 1.0f / time.ElapsedGameTime.TotalSeconds;
            this.fps = (int)fps;

			this.PreventViewportJump(mouseState);
            this._targetComponentsManager.UpdateMouseState(mouseState);
            this._measuringLine.UpdateMouseState(mouseState);

            this.CheckMove(mouseState);
            this.CheckZoomChange(mouseState);
            this.previousMousePosition = mouseState.Position;
        }

        /// <summary>
        /// Zkontroluje zda uživatel chce provést zoom
        /// </summary>
        /// <param name="mouseState"></param>
        private void CheckZoomChange(MouseState mouseState)
        {
	        //Rozmezí hodnot pro kolečko myši.
	        var mouseScrollValues = 120;
	        var originalWheelValue = mouseState.ScrollWheelValue / mouseScrollValues;
	        if (originalWheelValue > this.previousMouseWheelValue) {
		        this._mapProjection.ZoomMap(mouseState.Position.ToVector2(), this._mapProjection.ZoomLevel + ZoomIncreaseValue);
	        }
	        else if (originalWheelValue < this.previousMouseWheelValue) {
		        this._mapProjection.ZoomMap(mouseState.Position.ToVector2(), this._mapProjection.ZoomLevel - ZoomIncreaseValue);
	        }

	        this.previousMouseWheelValue = originalWheelValue;
        }

        /// <summary>
        /// Zkontroluje zda uživatel neprovedl zoom.
        /// </summary>
        /// <param name="mouseState">stav myši</param>
        private void CheckMove(MouseState mouseState)
        {
	        if (mouseState.Position != this.previousMousePosition && mouseState.LeftButton == ButtonState.Pressed &&
	            !this._targetComponentsManager.IsLabelPressed) {

		        this._mapProjection.TranslateMap((mouseState.Position - this.previousMousePosition).ToVector2());
	        }
        }

        /// <summary>
        /// Pokud uživatel klikne mimo aplikaci (aplikace se stane neaktivní) a pak klikne zpátky objeví se skok.
        /// Tato metoda tomu zabraňuje.
        /// </summary>
        /// <param name="mouseState"></param>
        private void PreventViewportJump(MouseState mouseState)
        {
	        if (this.IsActive && !this.wasActive) {
		        this.previousMousePosition = mouseState.Position;
	        }

	        this.wasActive = this.IsActive;
        }

        /// <summary>
        /// Vykresluje objekty.
        /// </summary>
        protected override void Draw(GameTime time)
        {
            this.GraphicsDevice.Clear(Colors.ViewBackgroundColor);

            //Vykresli 'rozpixelovane' textury napr. srážkový radar s blur efektem. 
            this.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearWrap, DepthStencilState.Default, null);
            this.drawBatch.Begin(DrawSortMode.Immediate, null, SamplerState.LinearWrap, null, null);          
            this._backgroundManager.Draw(this.spriteBatch, this.drawBatch);
            this.drawBatch.End();
            this.spriteBatch.End();

        
            //Targety vekreslí ostře
            this.spriteBatch.Begin();
            this.drawBatch.Begin();
            this._targetComponentsManager.Draw(this.spriteBatch, this.drawBatch);
            this._measuringLine.Draw(this.spriteBatch, this.drawBatch);
			#if DEBUG
			this.spriteBatch.DrawString(spriteFont, this.fps.ToString() + "FPS", new Vector2(20,20), Color.White);
			#endif
            this.drawBatch.End();
            this.spriteBatch.End();
    
        }
    }
}
