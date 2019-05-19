using System;
using System.Collections.Generic;
using OsnAPI.Aircraft;

namespace OsnAPI.Client
{
    /// <summary>
    /// Poskytuje data pro událost AicraftDataReceivedAsync.
    /// </summary>
    public class OsnAircraftEventArgs : EventArgs
    {
        /// <summary>
        /// Globální unix časová známka (sekundy) určující poslední aktualizaci dat.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Seznam dat o letadlech
        /// </summary>
        public List<OsnAircraft> ListOfAircraft { get; set; }

        /// <summary>
        /// Inicializuje instanci třídy StateVectorArgs
        /// </summary>
        /// <param name="timestamp">globální unix časová známka</param>
        /// <param name="listOfAircraft">seznam letadel</param>
        public OsnAircraftEventArgs(long timestamp, List<OsnAircraft> listOfAircraft)
        {
            this.Timestamp = timestamp;
            this.ListOfAircraft = listOfAircraft;
        }
    }
}
