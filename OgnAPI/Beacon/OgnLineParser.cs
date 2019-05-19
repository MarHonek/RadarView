using System.Text.RegularExpressions;

namespace OgnAPI.Beacon
{
	/// <summary>
	/// Třída reprezentuje parser raw packetu.
	/// </summary>
	public static class OgnLineParser
	{
		/// <summary>
		/// Zparsuje data ve formě řádku textu získané z aprsServeru.
		/// </summary>
		/// <param name="aprsLine">data ve formě řádku textu.</param>
		/// <returns>Základní informace o letadle.</returns>
		public static IAircraftBeacon Parse(string aprsLine)
		{
			IAircraftBeacon beacon = null;
			if (!aprsLine.StartsWith(OgnAircraftBeaconPatterns.APPS_SRV_MSG_FIRST_CHARACTER)) {
				//odfiltruje nepotřebná data.
				if (Regex.IsMatch(aprsLine, OgnAircraftBeaconPatterns.AIRCRAFT_BEACON_SENTENCE_PATTERN)) {
					beacon = new OgnAircraftBeacon(aprsLine);
				}
			}

			return beacon;
		}
	}
}
