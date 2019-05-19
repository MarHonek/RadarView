using System.Collections.Generic;
using RadarView.Model.Entities.AviationData.Airspace;
using RadarView.Model.Entities.Geographic;

namespace RadarView.Model.DataService.AviationData.AirspaceDataService
{
	/// <summary>
	/// DataService pro pristup k informacim o vzdusnych prostorech.
	/// </summary>
	public interface IAirspaceDataService
	{
		/// <summary>
		/// Vrati vzdusne prostory nachazejici se v zadane oblasti.
		/// </summary>
		/// <param name="boundingBox">Hranice zvoleneho prostoru.</param>
		/// <returns>Vyfiltrovane vzdusne prostory.</returns>
		List<Airspace> GetAllAirspaceInBoundingBox(BoundingBox boundingBox);
	}
}
