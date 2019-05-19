namespace RadarView.Model.Entities.AviationData.Airports
{
	/// <summary>
	/// Výčet typů letišť.
	/// </summary>
	public enum AirportType
	{
		/// <summary>
		/// Malé letiště.
		/// </summary>
		SmallAirport,

		/// <summary>
		/// Střední letiště.
		/// </summary>
		MediumAirport,

		/// <summary>
		/// Velké letiště.
		/// </summary>
		LargeAirport,

		/// <summary>
		/// Uzavřené letiště.
		/// </summary>
		Closed,

		/// <summary>
		/// Neznámá hodnota.
		/// </summary>
		Unknown
	}

	/// <summary>
	/// pomocná třída pro enum <see cref="AirportType"/>
	/// </summary>
	public static class AirportTypeHelper
	{
		/// <summary>
		/// Deserializuje typ letiště.
		/// </summary>
		/// <param name="airportType">typ letiště v textové podobě.</param>
		/// <returns>Typ letiště.</returns>
		public static AirportType DeserializeAirportType(string airportType)
		{
			switch (airportType) {
				case "large_airport":
					return AirportType.LargeAirport;
				case "medium_airport":
					return AirportType.MediumAirport;
				case "small_airport":
					return AirportType.SmallAirport;
				case "closed":
					return AirportType.Closed;
				default:
					return AirportType.Unknown;
			}
		}
	}
}
