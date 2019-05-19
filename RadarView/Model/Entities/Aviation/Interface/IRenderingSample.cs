using System;
using System.Collections.Generic;

namespace RadarView.Model.Entities.Aviation.Interface
{
    /// <summary>
    /// Rohraní pro třídy, které reprezentují zdroj predikovaných dat tj. vzorků polohových dat (letadel), které budou vykresleny, případně uloženy do souboru (zalogovány).
    /// </summary>
    public interface IRenderingSample
    {
        /// <summary>
        /// Vlastnost určující zda mohou být data zalogována (uložena do souboru).
        /// </summary>
        bool IsLoggable { get; }

        /// <summary>
        /// Získá seznam vzorků letadel (polohových dat), které budou vykresleny a případně uloženy do souboru (zalogovány).
        /// </summary>
        /// <param name="currentFixDateTime">Aktuální poloha letadel.</param>
        /// <param name="trailDotsDateTime">Historické polohy letadel.</param>
        /// <returns>Seznam vzorku letadel pro vykreslení.</returns>
        Dictionary<AircraftIdentifier, Aircraft> GetSample(DateTime currentFixDateTime, DateTime[] trailDotsDateTime);
    }
}
