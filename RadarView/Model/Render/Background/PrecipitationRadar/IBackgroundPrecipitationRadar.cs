using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using RadarView.Model.Render.Abstract;

namespace RadarView.Model.Render.Background.PrecipitationRadar
{
    public interface IBackgroundPrecipitationRadar : IRenderable
    {
	    /// <summary>
	    /// Textura nesoucí aktuální radarový snímek.
	    /// </summary>
	    Texture2D RadarTexture { get; set; }

	    /// <summary>
	    /// Změní průhlednost radarových snímků srážek.
	    /// Hodnota [0, 1]
	    /// </summary>
		float ImageOpacity { get; set; }

		/// <summary>
		/// Viditelnost radarových snímků srážek.
		/// </summary>
		bool ImageVisibility { get; set; }
    }
}
