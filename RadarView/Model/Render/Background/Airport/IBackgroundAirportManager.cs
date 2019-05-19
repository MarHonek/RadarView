using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadarView.Model.Render.Abstract;

namespace RadarView.Model.Render.Background.Airport
{
	/// <summary>
	/// Manager třída starající se vykreslované letiště.
	/// </summary>
    public interface IBackgroundAirportManager : IRenderable
    {
		/// <summary>
		/// Inicializační metoda.
		/// </summary>
	    Task Initialize();

		/// <summary>
		/// Nastaví viditelnost všech letišť kromě v konfiguraci nastavéného.
		/// </summary>
		/// <param name="shouldBeVisible">hodnota určující, zda mají být letiště viditelná.</param>
		void SetVisibilityExceptSelectedAirport(bool shouldBeVisible);
    }
}
