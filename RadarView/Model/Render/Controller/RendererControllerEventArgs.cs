using System.Collections.Generic;
using RadarView.Model.Entities.Aviation;

namespace RadarView.Model.Render.Controller
{
	/// <summary>
	/// Argumenty pro událost komponenty RendererController.
	/// </summary>
    public class RendererControllerEventArgs
	{
		/// <summary>
		/// Kolekce letadel k vykreslení.
		/// </summary>
		public Dictionary<AircraftIdentifier, Aircraft> Samples { get; set; }


		public RendererControllerEventArgs(Dictionary<AircraftIdentifier, Aircraft> sample)
		{
			this.Samples = sample;
		}
	}
}
