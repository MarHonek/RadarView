using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarView.utils
{
    /// <summary>
    /// Rozšiřuje třídu string
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// Vrací text, kde první písmeno je velké zbytek malé.
        /// </summary>
        /// <param name="rawString">zadaný text</param>
        /// <returns>Textový řetězec s velkým počátečním písmenem.</returns>
        public static string ToUpperFirstLetter(this string rawString)
        {
            return rawString[0].ToString().ToUpper() + rawString.Substring(1).ToLower();
        }
    }
}
