using System;
using LilyPath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.Managers.MapProjection;
using RadarView.Model.Render.Abstract;

namespace RadarView.Model.Render.Targets
{
    /// <summary>
    /// Reprezentuje sponici mezi aktuální (predikovanou) polohou a popiskem
    /// </summary>
    public class TargetLabelConnector : RenderObject, IRenderable
    {
        /// <summary>
        /// Popisek
        /// </summary>
        private TargetLabel label;

        /// <summary>
        /// Objekt reprezentující aktuální (predikovanou) polohu letadla
        /// </summary>
        private Model.Render.Targets.Target target;

        /// <summary>
        /// Pozice levého horního rohu popisku
        /// </summary>
        private Vector2 labelPoint;

        /// <summary>
        /// Mapová projekce
        /// </summary>
        private IViewportProjection projection;

        /// <summary>
        /// Aktuální poloha cíle.
        /// </summary>
        private Vector2 targetPoint;

        /// <summary>
        /// atribut určující zda došlo k uvolnění prostředků.
        /// </summary>
        private bool disposed;

        public TargetLabelConnector(Model.Render.Targets.Target target , TargetLabel label, IViewportProjection projection, Color color) : base(color)
        {
            this.target = target;            
            this.label = label;
            this.targetPoint = target.Point;
            this.projection = projection;
            this.target.LocationChanged += this.LocationChanged;
            projection.ViewportChanged += this.LocationChanged;
            this.label.MouseLeftPressedMove += this.LabelChanged;     
            this.label.MouseLeftClick += this.LabelChanged;
            this.labelPoint = this.GetNearestLabelSide();
        }

        private void LocationChanged(object sender, EventArgs e)
        {
            //Aktualizuje spojnici při změně pozice targetu nebo změně viewportu
            this.labelPoint = this.GetNearestLabelSide();
            this.targetPoint = this.target.Point;
        }

        private void LabelChanged(object sender, EventArgs e)
        {
            //Aktualizuje sponici při změně pozice nebo rozkliknutí popisku
            this.labelPoint = this.GetNearestLabelSide();
        }

        /// <summary>
        /// Vrací bod ve středu nejbližší strany Labelu od Targetu.
        /// </summary>
        /// <remarks>
        /// Pravá strana se mění v závislosti, zda je Label rozkliknutý nebo ne.
        /// </remarks>
        /// <returns>bod ve středu nejbližší strany Labelu</returns>
        private Vector2 GetNearestLabelSide()
        {
            return this.GetNearestVector(this.target.Point, new Vector2(this.label.Position.X + this.label.PrimaryContentSize.X / 2, this.label.Position.Y), new Vector2(this.label.Position.X + this.label.PrimaryContentSize.X / 2, this.label.Position.Y + this.label.Size.Y),
                new Vector2(this.label.Position.X, this.label.Position.Y + this.label.Size.Y / 2),
                new Vector2(this.label.Position.X + this.label.Size.X, this.label.Position.Y + this.label.Size.Y / 2));
        }

        /// <summary>
        /// Vrací pozici nejbližšího bodu z pole pointN od bodu point0.
        /// </summary>
        /// <param name="point0">pozice bodu, od kterého se měří vzdálenost</param>
        /// <param name="pointN">pole bodů pro měření vzdálenosti</param>
        /// <returns>Pozici nejbližšího bodu</returns>
        private Vector2 GetNearestVector(Vector2 point0, params Vector2[] pointN)
        {       
            var nearestVector = pointN[0];
            var minDistance = (pointN[0] - point0).Length();

            for (var i = 1; i < pointN.Length; i++)
            {
                var distance = (pointN[i] - point0).Length();
                if(distance < minDistance)
                {
                    minDistance = distance;
                    nearestVector = pointN[i];
                }           
            }
            return nearestVector;
        }


        /// <summary>
        /// Uvolní prostředky
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.target.LocationChanged -= this.LocationChanged;
                this.projection.ViewportChanged -= this.LocationChanged;
                this.label.MouseLeftPressedMove -= this.LabelChanged;
                this.label.MouseLeftClick -= this.LabelChanged;
            }

            this.disposed = true;

            base.Dispose(disposing);
        }

        public void Draw(SpriteBatch spriteBatch, DrawBatch drawBatch)
        {         
            drawBatch.DrawPrimitiveLine(this.Pen, this.targetPoint, this.labelPoint);
        }
    }
}
