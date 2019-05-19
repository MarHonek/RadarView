using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadarView.Model.Render.Abstract;

namespace RadarView.Model.Render.Background.Map
{
	/// <summary>
	/// Třída se stará o vykreslování mapových podkladů.
	/// </summary>
    public interface IBackgroundMapManager : IRenderable
	{
		/// <summary>
		/// Inicializuje mapové podklady.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Změní viditelnost mapového podkladů
		/// </summary>
		/// <param name="mapName">Název mapy.</param>
		/// <param name="isVisible">Hodnota určující zda má být mapa zobrazena.</param>
		void ChangeVisibility(string mapName, bool isVisible);
	}
}
