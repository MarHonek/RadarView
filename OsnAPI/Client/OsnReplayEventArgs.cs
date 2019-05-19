using System;

namespace OsnAPI.Client
{
    /// <summary>
    /// Argumenty události pro zpětné přehrávání logů.
    /// </summary>
    public class OsnReplayEventArgs : EventArgs
    {
        /// <summary>
        /// Časový posun oproti aktuálnímu času (sekundy).
        /// </summary>
        public int TimeOffset { get; }

        public OsnReplayEventArgs(int timeOffset)
        {
            this.TimeOffset = timeOffset;
        }
    }
}
