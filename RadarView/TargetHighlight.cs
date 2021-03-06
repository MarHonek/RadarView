﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarView
{
    /// <summary>
    /// Výčet typů zvýraznění cílů.
    /// </summary>
    public enum TargetHighlight
    {
        /// <summary>
        /// Upozornění 1.
        /// </summary>
        Notice1,

        /// <summary>
        /// Upozornění 2.
        /// </summary>
        Notice2,

        /// <summary>
        /// Výstraha.
        /// </summary>
        Alert, 

        /// <summary>
        /// Ostatní.
        /// </summary>
        Other
    }

    /// <summary>
    /// Statická helper třída pro zvýraznění cílů.
    /// </summary>
    public static class TargetHighlightHelper
    {
        /// <summary>
        /// Převede typ zvýraznění v textové podobě na enum TargetHighLight.
        /// </summary>
        /// <param name="highlightString">Typ zvýraznění v textové podobě.</param>
        /// <returns>Typ zvýrazněni jako enum TargetHighlight.</returns>
        public static TargetHighlight ParseHighlightString(string highlightString)
        {
            TargetHighlight output;       
            if(Enum.TryParse(highlightString, true, out output))
            {
                return output;
            } else
            {
                return TargetHighlight.Other;
            }
        }
    }
}
