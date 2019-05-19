using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using OgnAPI.Utils;

namespace OgnAPI.Beacon.Database
{
    /// <summary>
    /// Třídá získá dodatečné informace o letadlech z OGN databáze.
    /// </summary>
    public class OgnDescriptorDB
    {
        /// <summary>
        /// Časový interval dalšího pokusu o obnovení ztraceného síťového spojení (sekundy).
        /// </summary>
        private const int RECONNECTION_TIMEOUT = 5 * 1000;

        /// <summary>
        /// Maximální počet pokusu o navázání ztraceného síťového spojení.
        /// </summary>
        private const int RECONNECTION_ATTEMPT_COUNT = 5;

        /// <summary>
        /// URL OGN databáze.
        /// </summary>
        private const string DEFAULT_DB_URL = "http://ddb.glidernet.org/download/";

        /// <summary>
        /// Kolekce obsahující dodatečné informace o letadlech dle jejich ID.
        /// Klíč: Tracker ID letadla.
        /// Hodnota: Dodatečné informace o letadle.
        /// </summary>
        private Dictionary<string, IAircraftDescriptor> descriptors;

        /// <summary>
        /// Lock pro synchronizaci přístupu vláken ke sdíleným atributům.
        /// </summary>
        private static readonly object Lock = new object();

        /// <summary>
        /// Logger runtime chyb.
        /// </summary>
        private ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        /// <summary>
        /// Provede inicializaci.
        /// </summary>
        public async Task Initialize()
        {
	        await this.InitializeDescriptors();
		}


		/// <summary>
		/// Najde dodatečné informace o letadle dle zadane adresy 'tracker id'.
		/// </summary>
		/// <param name="address">Tracker id.</param>
		/// <returns>Dodatečné informace o letadle.</returns>
		public IAircraftDescriptor FindDescriptor(string address)
        {
	        if (address == null) {
		        return null;
	        }

	        lock (Lock) {
		        this.descriptors.TryGetValue(address, out var descriptor);
		        return descriptor;
	        }
        }

        /// <summary>
        /// Inicializuje/načte OGN databázi.
        /// </summary>
        public async Task InitializeDescriptors()
        {
	        //Cyklus pro opětovné připojení (v případě ztráty spojení).
	        for (var i = 0; i < RECONNECTION_ATTEMPT_COUNT; i++) {
		        try {
			        var descriptorsTemp = new Dictionary<string, IAircraftDescriptor>();

			        var client = new WebClient();
			        using (var reader = new StreamReader(client.OpenRead(DEFAULT_DB_URL))) {
				        //přeskočí první řádek začínající znakem #
				        reader.ReadLine();

				        while (!reader.EndOfStream) {
					        var line = reader.ReadLine();
							this.SplitDescriptorLine(descriptorsTemp, line);
				        }
			        }

			        lock (Lock) {
				        this.descriptors = descriptorsTemp;
					}
			        break;

		        } catch (Exception) {
					this.log.Warn(string.Format("exception caught while trying to connect to OGN database. retrying in {0} ms", RECONNECTION_TIMEOUT));
			        await Task.Delay(RECONNECTION_TIMEOUT);
		        }
	        }
        }

        /// <summary>
        /// Aktualizuje lokální OGN databázi.
        /// pozn. Aktuálně nepožadujeme aby databáze byla znovu načítána.
        /// </summary>
        /// <param name="token">token pro přerušení vlánka v pozadí.</param>
        private async Task UpdateDescriptors(CancellationToken token)
        {
	        for (var i = 0; i < RECONNECTION_ATTEMPT_COUNT; i++) {
		        try {
			        var descriptorsTemp = new Dictionary<string, IAircraftDescriptor>();

			        var client = new WebClient();
			        using (var reader = new StreamReader(client.OpenRead(DEFAULT_DB_URL))) {
				        //Přeskočí první řádek začínající s #
				        reader.ReadLine();

				        while (!reader.EndOfStream) {
					        if (token.IsCancellationRequested) {
						        break;
					        }

					        //Zparsuje
					        var line = reader.ReadLine();
							this.SplitDescriptorLine(descriptorsTemp, line);
				        }
			        }

			        lock (Lock) {
				        this.descriptors = descriptorsTemp;
			        }

			        break;
		        } catch (Exception) {
					this.log.Warn(string.Format("exception caught while trying to connect to OGN database. retrying in {0} ms", RECONNECTION_TIMEOUT));
			        await Task.Delay(RECONNECTION_TIMEOUT);
		        }
	        }
        }

        /// <summary>
        /// Zparsuje řádek databáze v textové podobě.
        /// </summary>
        /// <param name="descriptorsTemp">dočasná kolekce obsahující dodatečné informace o letadlech.</param>
        /// <param name="line">textový řádek databáze.</param>
        private void SplitDescriptorLine(Dictionary<string, IAircraftDescriptor> descriptorsTemp, string line)
        {
	        var splitLine = line.Split(',');
	        splitLine = splitLine.Select(item => item.Replace("'", string.Empty)).ToArray();

	        var tracked = splitLine[5] == "Y" ? true : false;
	        var identified = splitLine[6] == "Y" ? true : false;
	        IAircraftDescriptor aircraft = new AircraftDescriptor(splitLine[2].NullIfWhiteSpace(),
		        splitLine[3].NullIfWhiteSpace(), splitLine[4].NullIfWhiteSpace(), tracked, identified);

	        descriptorsTemp.Add(splitLine[1], aircraft);
        }
    }
}
