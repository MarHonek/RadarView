using OgnAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OgnAPI.Beacon;

namespace OgnAPI
{
    /// <summary>
    /// Argumenty události vyvolané při předání dat klientovi.
    /// </summary>
    public class OgnEventArgs : EventArgs
    {
        /// <summary>
        /// Základní informace o letadle.
        /// </summary>
        public IAircraftBeacon Beacon { get; set; }

        /// <summary>
        /// Dodatečné informace o letadle.
        /// </summary>
        public IAircraftDescriptor Descriptor { get; set; }

        public OgnEventArgs(IAircraftBeacon beacon, IAircraftDescriptor descriptor)
        {
            this.Beacon = beacon;
            this.Descriptor = descriptor;
        }
    }
}
