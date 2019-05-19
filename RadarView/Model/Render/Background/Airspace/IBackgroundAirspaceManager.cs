using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using RadarView.Model.Entities.AviationData.Airspace;
using RadarView.Model.Render.Abstract;
using RadarView.Model.Service.Config;

namespace RadarView.Model.Render.Background.Airspace
{
    public interface IBackgroundAirspaceManager : IRenderable
    {
	    /// <summary>
	    /// Inicializační metoda.
	    /// </summary>
	    void Initialize();

	    /// <summary>
	    /// Nastaví videtelnost vzdušných prostorů podle jejich kategorie
	    /// </summary>
	    /// <param name="category">kategorie vzdušných prostorů</param>
	    /// <param name="visible">true pokud má být vzdušný prostor viditelný, jinak false</param>
	    void AirspaceCategoryVisibility(AirspaceCategory category, bool visible);
    }
}
