using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadarView.Model.Render.Abstract;

namespace RadarView.Model.Render.Background.BackgroundManager
{
	/// <summary>
	/// Manager třída starající se o vykreslení všech komponent v pozadí.
	/// </summary>
    public interface IBackgroundManager : IRenderable
    {
		/// <summary>
		/// Provede incializaci komponent vykreslených v pozadí.
		/// </summary>
	    Task Initialize();
    }
}
