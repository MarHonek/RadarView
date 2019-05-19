using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using RadarView.Model.Entities.Geographic;
using BoundingBox = RadarView.Model.Entities.Geographic.BoundingBox;

namespace RadarView.Model.Managers.MapProjection
{
	/// <summary>
	/// Třída, která se stará o převod zeměpisnou polohu na pozici pixelů.
	/// </summary>
	public interface IViewportProjection
    {
	    /// <summary>
	    /// zeměpisná poloha středového bodu mapy.
	    /// </summary>
	    Location Center { get; set; }

	    /// <summary>
	    /// Aktuální zoom level
	    /// </summary>    
	    float ZoomLevel { get; set; }

	    /// <summary>
	    /// Transformuje zeměpisnou polohu na souřadnice pixelu
	    /// </summary>
	    /// <param name="location">zeměpisná poloha</param>
	    /// <returns>souřadnice pixelu</returns>
	    Vector2 LocationToViewportPoint(Location location);

	    /// <summary>
	    /// Transformuje souřadnice pixelu na geografickou polohu
	    /// </summary>
	    /// <param name="point">souřadnice pixelu</param>
	    /// <returns>zeměpisná poloha</returns>
	    Location ViewportPointToLocation(Vector2 point);

	    /// <summary>
	    /// Nastaví hodnotu vlastnosti Zoom při zachování stejného středového bodu zobrazení (viewportu)
	    /// </summary>
	    /// <param name="center">souřadnice středového bodu pro zoom</param>
	    /// <param name="zoomLevel">nový zoom level</param>
	    void ZoomMap(Vector2 center, float zoomLevel);

	    /// <summary>
	    /// Změní vlastnost Center na základě vektoru posunu v pixelech.
	    /// </summary>
	    /// <param name="translation">vektor posunu (pixely)</param>
	    void TranslateMap(Vector2 translation);

	    /// <summary>
	    /// Vytvoří novou instanci třídy BoundingBox s hranicemi odpovídajícimi hranicím výchozího zobrazení (viewportu)
	    /// </summary>
	    /// <returns>vytvořený BoundingBox</returns>
	    BoundingBox ViewportToBoundingBox();

	    /// <summary>
	    /// Nastaví dočasný středový bod zobrazení (viewportu)
	    /// Tento středový bod je automaticky resetován když vlastnost Center je změněna
	    /// </summary>
	    /// <param name="center">střed.</param>
	    void SetTransformCenter(Vector2 center);

	    /// <summary>
	    /// Velikost (rozlišení) obrazu pro vykreslení.
	    /// </summary>
	    Vector2 RenderSize { get; set; }

		/// <summary>
		/// Provede inicializaci.
		/// </summary>
		/// <param name="center">Střed zobrazení.</param>
	    void Initialize(Location center);

		/// <summary>
		/// Vyvolá událost při změně mapové oblasti (viewportu)
		/// </summary>
		event EventHandler ViewportChanged;
	}
}
