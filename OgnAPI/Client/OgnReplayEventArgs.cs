using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgnAPI.Client
{
    /// <summary>
    /// Argumenty události pro zpětné přehrávání logů.
    /// </summary>
    public class OgnReplayEventArgs : EventArgs
    {
        /// <summary>
        /// Časový posun oproti aktuálnímu času (sekundy).
        /// </summary>
        public int TimeOffset { get; }

        public OgnReplayEventArgs(int timeOffset)
        {
            this.TimeOffset = timeOffset;
        }
        
    }
}
