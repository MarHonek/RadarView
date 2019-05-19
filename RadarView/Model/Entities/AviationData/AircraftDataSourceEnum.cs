using System;
using System.Collections.Generic;

namespace RadarView.Model.Entities.AviationData
{
	/// <summary>
	/// Výčet zdrojů polohových dat letadel.
	/// </summary>
    public enum AircraftDataSourceEnum
    {
		/// <summary>
		/// Open Glider Network.
		/// </summary>
		OGN,

		/// <summary>
		/// Open Sky Network.
		/// </summary>
		OSN,

		/// <summary>
		/// Jiný.
		/// </summary>
		Other
    }

	/// <summary>
	/// Pomocná třída pro práci s enum AircraftDataSourceEnum.
	/// </summary>
	public class AircraftDataSourceEnumHelper 
	{
		/// <summary>
		/// Konvertuje string na enum.
		/// </summary>
		public AircraftDataSourceEnum ConvertToEnum(string enumString) 
		{
			AircraftDataSourceEnum dataSourceEnum;
			Enum.TryParse(enumString, out dataSourceEnum);
			return dataSourceEnum;
		}
	}
}
