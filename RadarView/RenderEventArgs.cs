using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarView
{
    /// <summary>
    /// Argumenty události předány při překreslení obrazovky zobrazující vzdušnou situaci.
    /// </summary>
    public class RenderEventArgs : EventArgs
    {
        /// <summary>
        /// Časová známka posledního překreslení.
        /// </summary>
        public long RenderTimestamp { get; set; }

        public RenderEventArgs(long RenderTimestamp)
        {
            this.RenderTimestamp = RenderTimestamp;
        }
    }
}
