using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using OgnAPI.Utils;

namespace OgnAPI.Beacon
{
    /// <summary>
    /// Třída reprezentuje letadlo jehož informace byly získané ze služby OGN.
    /// </summary>
    public class OgnAircraftBeacon : AircraftBeacon
    {
        //Příklad formátu APRS zprávy na https://github.com/svoop/ogn_client-ruby/wiki/SenderBeacon

        public OgnAircraftBeacon(string aprsSentence)
        {
            this.Parse(aprsSentence);
        }

        /// <summary>
        /// Zparsuje zprávu z APRS serveru.
        /// </summary>
        /// <param name="aprsSentence">zpráva z APRS serveru.</param>
        private void Parse(string aprsSentence)
        {
	        // zapamatuje si raw packet.
	        this.rawPacket = aprsSentence;

	        var ognMatch = new Regex(OgnAircraftBeaconPatterns.AIRCRAFT_BEACON_SENTENCE_PATTERN).Match(aprsSentence);

	        this.callSign = ognMatch.Groups["callsign"].Value;
	        this.dstCall = ognMatch.Groups["dstcall"].Value;
	        this.receiverName = ognMatch.Groups["receiver"].Value;
	        this.timestamp = AprsUtils.ToUtcTimestamp(ognMatch.Groups["time"].Value);

	        this.latitude = AprsUtils.DmsToDeg(double.Parse(ognMatch.Groups["latitude"].Value, CultureInfo.InvariantCulture) / 100);
	        if (ognMatch.Groups["latitude_sign"].Value == "S")
		        this.latitude *= -1;

	        this.longitude = AprsUtils.DmsToDeg(double.Parse(ognMatch.Groups["longitude"].Value, CultureInfo.InvariantCulture) / 100);
	        if (ognMatch.Groups["longitude_sign"].Value == "W")
		        this.longitude *= -1;

	        if (!string.IsNullOrEmpty(ognMatch.Groups["course_extension"].Value)) {
		        var trackString = ognMatch.Groups["course"].Value;
		        var groundSpeedString = ognMatch.Groups["ground_speed"].Value;

		        var appropriateValues = new string[] {"...", "   "};
		        if (trackString != "000" || groundSpeedString != "000") {
			        if (!appropriateValues.Contains(trackString) && !appropriateValues.Contains(groundSpeedString)) {
				        this.track = int.Parse(trackString);
				        this.groundSpeed = float.Parse(groundSpeedString, CultureInfo.InvariantCulture);
			        }
		        }
	        }

	        this.altitude = float.Parse(ognMatch.Groups["altitude"].Value, CultureInfo.InvariantCulture);

	        if (!string.IsNullOrEmpty(ognMatch.Groups["pos_extension"].Value)) {
		        var dlat = double.Parse(ognMatch.Groups["latitude_enhancement"].Value, CultureInfo.InvariantCulture) / 1000 / 60;
		        var dlon = double.Parse(ognMatch.Groups["longitude_enhancement"].Value, CultureInfo.InvariantCulture) / 1000 / 60;

		        this.latitude += this.latitude > 0 ? dlat : -dlat;
		        this.longitude += this.longitude > 0 ? dlon : -dlon;
	        }

	        var ognOptional = ognMatch.Groups["ogn_optional"].Value;
	        if (!string.IsNullOrEmpty(ognOptional)) {
		        this.ParseOptional(ognOptional);
	        }
        }

        /// <summary>
        /// Zparsuje nepovinné údaje.
        /// </summary>
        /// <param name="ognOptional">nepovinné údaje jako zpráva APRS serveru.</param>
        private void ParseOptional(string ognOptional)
        {
	        var commentMatch = new Regex(OgnAircraftBeaconPatterns.AIRCRAFT_BEACON_OPTIONAL_PATTERN).Match(ognOptional);

	        this.address = commentMatch.Groups["id"].Value.NullIfWhiteSpace();

	        var flags = commentMatch.Groups["details"].Value.NullIfWhiteSpace();
	        if (flags != null) {
		        this.stealth = AprsUtils.DecodeStealthValue(flags);
		        this.noTracking = AprsUtils.DecodeNoTrackingValue(flags);
		        this.aircraftType = AircraftTypeExtension.ForValue(AprsUtils.DecodeAircraftTypeValue(flags));
		        this.addressType = AddressTypeExtension.ForValue(AprsUtils.DecodeAddressTypeValue(flags));
	        }

	        this.climbRate = commentMatch.Groups["climb_rate"].Value.ParseToFloatOrNull();
	        this.turnRate = commentMatch.Groups["turn_rate"].Value.ParseToFloatOrNull();
	        this.flightLevel = commentMatch.Groups["flight_level"].Value.ParseToFloatOrNull();
	        this.signalStrength = commentMatch.Groups["signal_quality"].Value.ParseToFloatOrNull();
	        this.errorCount = commentMatch.Groups["errors"].Value.ParseToIntOrNull();
	        this.frequencyOffset = commentMatch.Groups["frequency_offset"].Value.ParseToFloatOrNull();
	        this.gpsStatus = commentMatch.Groups["gps_accuracy"].Value.NullIfWhiteSpace();
	        this.hardwareVersion = AprsUtils.DecodeHardwareVersion(commentMatch.Groups["flarm_hardware_version"].Value);
	        this.firmwareVersion = commentMatch.Groups["flarm_software_version"].Value.ParseToFloatOrNull();
	        this.originalAddress = commentMatch.Groups["flarm_id"].Value.NullIfWhiteSpace();
	        this.erp = commentMatch.Groups["signal_power"].Value.ParseToFloatOrNull();

	        var heardAircrafts = commentMatch.Groups["proximity"].Value;
	        this.heardAircraftIds = !string.IsNullOrEmpty(heardAircrafts) ? heardAircrafts.Split(' ').ToList() : null;
        }
    }
}
