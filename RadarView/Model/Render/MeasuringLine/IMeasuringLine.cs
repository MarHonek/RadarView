using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;
using RadarView.Model.Render.Abstract;

namespace RadarView.Model.Render.MeasuringLine
{
	/// <summary>
	/// Reprezentuje čáru pro měření vzdálenosti mezi body.
	/// </summary>
	public interface IMeasuringLine : IRenderable
	{
		/// <summary>
		/// Událost vyvolána, pokud byla meřena vzdálenost nebo linie zrušena.
		/// </summary>
		event EventHandler<MeasuringLineEventArgs> StateChanged;

		/// <summary>
		/// Aktualizuje stav užívatelské myši.
		/// </summary>
		/// <param name="mouseState">Stav uživatelské myši.</param>
		void UpdateMouseState(MouseState mouseState);
	}
}
