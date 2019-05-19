using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using RadarView.Model.Entities.Aviation;
using RadarView.Model.Render.Abstract;

namespace RadarView.Model.Render.Targets
{
	/// <summary>
	/// Spravuje komponenty cíle.
	/// </summary>
	public interface ITargetComponentsManager : IRenderable
	{
		/// <summary>
		/// Inicializační metoda.
		/// </summary>
		void Initialize();

		/// <summary>
		/// Aktualizuje stav myši.
		/// </summary>
		/// <param name="mouseState"></param>
		void UpdateMouseState(MouseState mouseState);

		/// <summary>
		/// Aktualizuje komponenty cíle, případně vytvoří nový pokud neexistuje.
		/// </summary>
		/// <param name="newSample">kolekce nových informací o letadlech.</param>
		void UpdateTargets(Dictionary<AircraftIdentifier, Aircraft> newSample);

		/// <summary>
		/// Změní maximální zobrazovanou výšku.
		/// </summary>
		/// <param name="maxAltitude">Maximální zobrazovaná výška (stopy).</param>
		void ChangeMaxAltitude(int maxAltitude);

		/// <summary>
		/// Příznak určující zda uživatel kliknul na popisek letadla.
		/// </summary>
		bool IsLabelPressed { get; set; }
	}
}
