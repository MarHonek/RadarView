using System.Globalization;
using Microsoft.Xna.Framework;

namespace RadarView.Model.Entities.Geographic
{
    /// <summary>
    /// Reprezentuje oblast ohraničenou zadanými zeměpisnými souřadnicemi.
    /// </summary>
    public class BoundingBox
    {
        /// <summary>
        /// Zeměpisná šířka jižní hranice.
        /// </summary>
        public float South { get; set; }

        /// <summary>
        /// Zeměpisná šířka severní hranice.
        /// </summary>
        public float North { get; set; }

        /// <summary>
        /// Zeměpisná délka západní hranice.
        /// </summary>
        public float West { get; set; }

        /// <summary>
        /// Zeměpisná délka východní hranice.s
        /// </summary>
        public float East { get; set; }

        /// <summary>
        /// Konstruktor vytvoří novou oblast.
        /// </summary>
        /// <param name="south">zeměpisná délka jižní hranice.</param>
        /// <param name="west">zeměpisná délka západní hranice.</param>
        /// <param name="north">zeměpisná délka severní hranice.</param>
        /// <param name="east">zeměpisná délka východní hranice.</param>
        public BoundingBox(float south, float west, float north, float east)
        {
            this.South = south;
            this.West = west;
            this.North = north;
            this.East = east;
        }

        /// <summary>
        /// Konstruktor vytvoří novou oblast na základě řetězce ve tvaru:
        /// North West South East
        /// pr.
        /// 49.0216 16.061 48.0566 17.65065
        /// </summary>
        /// <param name="boundingBox">hranice prostoru zadaného v textové podobě.</param>
        public BoundingBox(string boundingBox)
        {
            var coord = boundingBox.Split(' ');
            this.North = float.Parse(coord[0], CultureInfo.InvariantCulture);
            this.West = float.Parse(coord[1], CultureInfo.InvariantCulture);
            this.South = float.Parse(coord[2], CultureInfo.InvariantCulture);
            this.East = float.Parse(coord[3], CultureInfo.InvariantCulture);
        }
   
        /// <summary>
        /// Šířka oblasti ve stupních (desítkově).
        /// </summary>
        public float Width
        {
            get { return this.East - this.West; }
        }

        /// <summary>
        /// Výška oblasti ve stupních (desítkově).
        /// </summary>
        public float Height
        {
            get { return this.North - this.South; }
        }

		/// <summary>
		/// Rozšíří oblast. 
		/// </summary>
		/// <param name="size">Velikost v úhlech o kterou se oblast rozšíří v každém směru.</param>
		/// <returns>Rozšířená oblast.</returns>
        public BoundingBox ExtendBox(float size)
        {
			return new BoundingBox(this.South - size, this.West - size, this.North + size, this.East + size);
        }

        /// <summary>
        /// Zkontroluje zda má oblast správně zadané hranice tj. východní hranice je menší než západní. Serverní hranice je větší než jižní.
        /// </summary>
        public bool HasValidBounds
        {
            get { return this.South < this.North && this.West < this.East; }
        }     

        /// <summary>
        /// Zjistí zda se zadaný bod nachází unvitř oblasti.
        /// </summary>
        /// <param name="location">zeměpisná poloha bodu.</param>
        /// <returns>true pokud se nachází uvnitř oblasti, jinak false.</returns>
        public bool Contains(Location location)
        {
            return location.Latitude > this.South && location.Latitude < this.North &&
                location.Longitude > this.West && location.Longitude < this.East;
        }

		/// <summary>
		/// Vytvoří hranice oblasti na základě středu a průměru čtverce
		/// </summary>
		/// <param name="center">Poloha středu</param>
		/// <param name="size">Velikost stran v metrech.</param>
		/// <returns>Vytvořená oblast.</returns>
        public static BoundingBox CreateBoundingBox(Location center, int size)
        {
	        var topLeftCorner = Location.TranslateLocation(center, new Vector2(-size / 2, size / 2));
	        var rightBottomCorner = Location.TranslateLocation(center, new Vector2(size / 2, -size / 2));
	        var monitoredArea = new BoundingBox(rightBottomCorner.Latitude, topLeftCorner.Longitude, topLeftCorner.Latitude, rightBottomCorner.Longitude);

	        return monitoredArea;
        }


        public override string ToString()
        {
            return this.North.ToString(CultureInfo.InvariantCulture) + " " +
                   this.West.ToString(CultureInfo.InvariantCulture) + " " +
                   this.South.ToString(CultureInfo.InvariantCulture) + " " +
                   this.East.ToString(CultureInfo.InvariantCulture);
        }
    }
}
