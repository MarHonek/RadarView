using System;

namespace RadarView.Model.DataService.Network
{
	/// <summary>
	/// DataService pro síťovou komunikaci s aplikaci AfisStrip.
	/// (Aktuálně se jedná pouze o přípravu na propojení. Prozatím není Afistrip na komunikaci připraven).
	/// Přijaté zprávy obsahují identifikátory letadel a vzdušných prostorů, které se mají zvýraznit.
	/// </summary>
	public interface IAfisStripDataService
	{
		/// <summary>
		/// Událost vyvolána při obdržení informací (z UDP klienta) o aktivních vzdušných prostorech. 
		/// </summary>
	    event EventHandler<AfisStripDataService.ActiveAirspaceEventArgs> ActiveAirspaceReceived;
	}
}
