using System.Collections.Generic;
using RadarView.Model.Entities.AviationData.Airspace;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Service.AviationData;

namespace RadarView.Model.DataService.AviationData.AirspaceDataService
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class AirspaceDataService : IAirspaceDataService
	{

		private IAirspaceFileReader _airspaceFileReader;


		public AirspaceDataService(IAirspaceFileReader airspaceFileReader)
		{
			this._airspaceFileReader = airspaceFileReader;
		}

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public List<Airspace> GetAllAirspaceInBoundingBox(BoundingBox boundingBox)
		{
			var allAirspace = this._airspaceFileReader.ListAll();

			var airspaceListInBoundingBox = new List<Airspace>();
			foreach (var airspace in allAirspace) {
				var airspaceCoordinates = airspace.Geometry.Coordinates;

				foreach (var airspaceCoordinate in airspaceCoordinates) {
					if (boundingBox.Contains(airspaceCoordinate)) {
						airspaceListInBoundingBox.Add(airspace);
						break;
					}
				}
			}
			return airspaceListInBoundingBox;
		}
	}
}
