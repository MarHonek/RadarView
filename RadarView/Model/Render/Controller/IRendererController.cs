using System;

namespace RadarView.Model.Render.Controller
{
	public interface IRendererController
	{
		/// <summary>
		/// Spustí vykreslování.
		/// </summary>
		void Start();

		/// <summary>
		/// Zastaví vykreslování.
		/// </summary>
		void Stop();

		/// <summary>
		/// Událost vyvolána když je možné letadla vykreslit.
		/// </summary>
		event EventHandler<RendererControllerEventArgs> SamplesReceived;
	}
}
