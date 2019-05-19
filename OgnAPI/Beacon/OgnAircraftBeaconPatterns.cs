namespace OgnAPI.Beacon
{
    /// <summary>
    /// Pattern pro parsování zpráv ze APRS serveru.
    /// </summary>
    public static class OgnAircraftBeaconPatterns
    {
        // OGN APRS servers reply to the client or send periodic heart-bit where
        // first character is #
        // e.g:
        // # aprsc 2.0.14-g28c5a6a
        // # logresp PCBE13-1 unverified, server GLIDERN2
        public const string APPS_SRV_MSG_FIRST_CHARACTER = "#";

        //Pattern pro základní informace.
        public const string AIRCRAFT_BEACON_SENTENCE_PATTERN = @"(?<callsign>.+?)>(?<dstcall>.+),(?<relay>.+),(?<receiver>(.+)):/(?<time>\d{6}h|z)" + 
            @"(?<latitude>9000\.00|[0-8]\d{0,3}\s*\.\d{0,2}\s*)(?<latitude_sign>N|S)(?<symbol_table>.)" +
            @"(?<longitude>18000\.00|1[0-7]\d{0,3}\s*\.\d{0,2}\s*|0\d{0,4}\s*\.\d{0,2}\s*)(?<longitude_sign>E|W)(?<symbol>.)" +
            @"(?<course_extension>(?<course>(\d|\s|\.){3})/(?<ground_speed>(\d|\s|\.){3}))/A=(?<altitude>(-\d{5}|\d{6}))(?<pos_extension>\s" +
            @"!W((?<latitude_enhancement>\d)(?<longitude_enhancement>\d))!)(?:\s(?<ogn_optional>.*))?";

        //Pattern pro nepovinné údaje.
        public const string AIRCRAFT_BEACON_OPTIONAL_PATTERN =
            @"id(?<details>[\dA-F]{2})(?<id>[\dA-F]{6}?)\s" +
            @"((?<climb_rate>[+-]\d+?)fpm\s)?" +
            @"((?<turn_rate>[+-][\d.]+?)rot\s)?" +
            @"(FL(?<flight_level>[\d.]+)\s)?" +
            @"((?<signal_quality>[\d.]+?)dB\s)?" +
            @"((?<errors>\d+)e\s)?" +
            @"((?<frequency_offset>[+-][\d.]+?)kHz\s?)?" +
            @"(gps(?<gps_accuracy>\d+x\d+)\s?)?" +
            @"(s(?<flarm_software_version>[\d.]+)\s?)?" +
            @"(h(?<flarm_hardware_version>[\dA-F]{2})\s?)?" +
            @"(r(?<flarm_id>[\dA-F]+)\s?)?" +
            @"((?<signal_power>[+-][\d.]+)dBm\s?)?" +
            @"((?<proximity>(hear[\dA-F]{4}\s?)+))?";
    }
}
