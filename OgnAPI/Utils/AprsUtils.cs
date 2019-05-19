using System;
using System.Net;
using System.Text;

namespace OgnAPI.Utils
{
    /// <summary>
    /// Třída obsahující pomocné metody pro komunikace s APRS serverem.
    /// </summary>
    class AprsUtils
    {
        /// <summary>
        /// Generuje unikátní ID klienta.
        /// </summary>
        /// <returns>Unikátní ID.</returns>
        public static string GenerateClientId()
        {
            var suffix = Guid.NewGuid().ToString().Split('-')[3].ToUpper();
            var res = Dns.GetHostName().Substring(0, 3).ToUpper();
            var bld = new StringBuilder(res.Replace("-", ""));
            bld.Append("-");
            bld.Append(suffix);

            return bld.ToString();
        }

        /// <summary>
        /// Vytvoří přihlašovací větu k OGN serveru.
        /// </summary>
        /// <param name="userName">Přihlašovací jméno.</param>
        /// <param name="passCode">Heslo.</param>
        /// <param name="appName">Jméno aplikace.</param>
        /// <param name="version">Verze aplikace.</param>
        /// <param name="filter">Filtr.</param>
        /// <returns>Přihlašovací věta k OGN serveru.</returns>
        public static string FormatAprsLoginLine(string userName, string passCode, string appName,
            string version, string filter)
        {
            return filter == null ? string.Format("user {0} pass {1} vers {2} {3}", userName, passCode, appName, version)
                    : string.Format("user {0} pass {1} vers {2} {3} filter {4}", userName, passCode, appName, version, filter);
        }


        /// <summary>
        /// Vytvoří přihlašovací větu k OGN serveru.
        /// </summary>
        /// <param name="userName">Přihlašovací jméno.</param>
        /// <param name="passCode">Heslo.</param>
        /// <param name="appName">Jméno aplikace.</param>
        /// <param name="version">Verze aplikace.</param>
        /// <returns>Přihlašovací věta k OGN serveru.</returns>
        public static string FormatAprsLoginLine(string userName, string passCode, string appName,
                string version)
        {
            return FormatAprsLoginLine(userName, passCode, appName, version, null);
        }

        /// <summary>
        /// Konvertuje formát stupně-minuty-sekundy na desetiny stupňů.
        /// </summary>
        /// <param name="dms">hodnota ve formátu stupně-minuy-sekundy.</param>
        /// <returns>hodnota ve formátu desetiny stupně.</returns>
        public static double DmsToDeg(double dms)
        {
            var absDms = Math.Abs(dms);
            var d = Math.Floor(absDms);
            var m = (absDms - d) * 100 / 60;
            return d + m;
        }

        /// <summary>
        /// Vytvoří unix časovou známku (UTC) na základě hodin-minut-sekund. Čas se vypočítá z lokálního časového pásma.
        /// </summary>
        /// <param name="h">hodiny.</param>
        /// <param name="m">minuty.</param>
        /// <param name="s">sekundy.</param>
        /// <returns>Sekundy od počátku unix epochy.</returns>
        public static long ToUtcTimestamp(int h, int m, int s)
        {
            var ognTimeStamp = new TimeSpan(h, m, s);
            var now = DateTime.Now;

            var duration = new TimeSpan(ognTimeStamp.Ticks - now.TimeOfDay.Ticks);
            var reverse = duration.Ticks < 0 ? duration.Add(new TimeSpan(24, 0, 0)) : duration.Add(new TimeSpan(-24, 0, 0));

            var smallerDiff = Math.Abs(duration.Ticks) < Math.Abs(reverse.Ticks) ? duration : reverse;
            var result = now.Add(smallerDiff);

            var unixEpoch = result - new DateTime(1970, 1, 1, 0, 0, 0);
            return (long)unixEpoch.TotalSeconds;
        }

        /// <summary>
        /// Vytvoří unix časovou známku (UTC) na základě hodin-minut-sekund. Čas se vypočítá z lokálního časového pásma.
        /// </summary>
        /// <param name="time">čas ve formátu 6 číslic získané z OGN packetu (Např. 162334, 051202)</param>
        /// <returns>Počet sekund od počátku unix epochy.</returns>
        public static long ToUtcTimestamp(string time)
        {
            var h = Convert.ToInt32(time.Substring(0, 2));
            var m = Convert.ToInt32(time.Substring(2, 2));
            var s = Convert.ToInt32(time.Substring(4, 2));

            return ToUtcTimestamp(h, m, s);
        }

        /// <summary>
        /// Dekóduje typ letadla.
        /// Konvertuje 4 bity (3. a 6.) v hexadecimální reprezentaci na hodnotu indexu typu letadla.
        /// </summary>
        /// <param name="hexValue">Hodnota v hexadecimálním tvaru.</param>
        /// <returns>Index typu letadla.</returns>
        public static int DecodeAircraftTypeValue(string hexValue)
        {
            int decValue = Convert.ToInt16(hexValue, 16);
            var transDecValue = ((uint)decValue) & 124;
            return (int)(transDecValue >> 2);
        }

        /// <summary>
        /// Dekóduje typ adresy.
        /// Konvertuje poslední dva bity v hexadecimální reprezentaci na hodnotu indexu typu letadla.
        /// </summary>
        /// <param name="hexValue">hodnota v hexadecimálním tvaru.</param>
        /// <returns>Index typu letadla.</returns>
        public static int DecodeAddressTypeValue(string hexValue)
        {
            int decValue = Convert.ToInt16(hexValue, 16);
            var transDecValue = ((uint)decValue) & 3;
            return (int)(transDecValue);
        }

        /// <summary>
        /// Dekóduje hodnotu 'stealth mode'
        /// Konvertuje první bit v hexadecimálním tvaru na hodnotu typu bool.
        /// </summary>
        /// <param name="hexValue">Hodnota v hexadecimálním tvaru.</param>
        /// <returns>True pokud je 'stealth mode' aktivní, jinak false.</returns>
        public static bool DecodeStealthValue(string hexValue)
        {
            int decValue = Convert.ToInt16(hexValue, 16);
            var transDecValue = ((uint)decValue) & 128;
            return (transDecValue >> 7) == 1;
        }

        /// <summary>
        /// Dekóduje hodnotu 'no tracking mode'.
        /// Konvertuje druhý bit v hexadecimální reprezentaci na hodnotu typu bool.
        /// </summary>
        /// <param name="hexValue">Hodnota v hexadecimálním tvaru.</param>
        /// <returns>True pokud je 'no-tracking mode' aktivní, jinak false.</returns>
        public static bool DecodeNoTrackingValue(string hexValue)
        {
            int decValue = Convert.ToInt16(hexValue, 16);
            var transDecValue = ((uint)decValue) & 64;
            return (transDecValue >> 6) == 1;
        }

        /// <summary>
        /// Dekóduje verzi Hardware.
        /// Konvertuje číselnou hodnotu v hexadecimálním tvaru.
        /// </summary>
        /// <param name="hexValue">Hodnota v hexadecimálním tvaru.</param>
        /// <returns>Číslo verze hardware pokud je dostupná, jinak null.</returns>
        public static int? DecodeHardwareVersion(string hexValue)
        {
	        if (!string.IsNullOrEmpty(hexValue)) {
		        return Convert.ToInt16(hexValue, 16);
	        }

	        return null;
        }
    }
}
