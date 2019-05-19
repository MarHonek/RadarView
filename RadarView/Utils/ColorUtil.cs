using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarView.Utils
{
	/// <summary>
	/// Pomocná třída pro práci s barvami.
	/// </summary>
    public static class ColorUtil
    {
        /// <summary>
        /// Konvertuje barvu v hexadecimálním tvaru #rrggbb nebo #rrggbbaa do objektu třídy Color 
        /// </summary>
        /// <param name="hexFormatColor">barva ve hexadecimálním tvaru</param>
        /// <returns>Konvertovaná barva</returns>
        public static Color HexToColor(string hexFormatColor)
        {
            var red = 0;
            var green = 0;
            var blue = 0;
            var alpha = 255;

            red = Convert.ToInt32(hexFormatColor.Substring(1, 2), 16);
            green = Convert.ToInt32(hexFormatColor.Substring(3, 2), 16);
            blue = Convert.ToInt32(hexFormatColor.Substring(5, 2), 16);

            if(hexFormatColor.Length == 9)
            {
                alpha = Convert.ToInt32(hexFormatColor.Substring(7, 2), 16);
            }

            return new Color(red, green, blue, alpha);

        }
    }
}
