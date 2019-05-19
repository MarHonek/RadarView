using System.Globalization;

namespace OgnAPI.Utils
{
    /// <summary>
    /// Pomocná třída pro parsování stringu na ostatní typy.
    /// </summary>
    public static class StringExtensions
    {
	    ///<summary>
	    ///Konvertuje string reprezentující číslo typu float na číslo typu 'nullable' float.
	    ///</summary>
	    ///<returns>Null jestliže se hodnota rovná prázdnému řetězci, jinak číslo typu float.</returns>
	    internal static float? ParseToFloatOrNull(this string value)
	    {
		    if (value == string.Empty) {
			    return null;
		    }

		    return float.Parse(value, CultureInfo.InvariantCulture);
	    }

	    ///<summary>
	    ///Konvertuje string reprezetující číslo typu int na číslo typu 'nullable' int
	    ///</summary>
	    ///<returns>Null jestliže s hodnota rovná prázdnému řetězci, jinak číslo typu int.</returns>
	    internal static int? ParseToIntOrNull(this string value)
	    {
		    if (value == string.Empty) {
			    return null;
		    }

		    return int.Parse(value);
	    }

	    ///<summary>
	    ///Konvertuje string obsahující bílé znaky na hodnotu null.
	    ///</summary>
	    ///<returns>Null jestliže string obsahuje prázdné znaky, jinak nezměněnou hodnotu.</returns>
	    internal static string NullIfWhiteSpace(this string value)
	    {
		    if (string.IsNullOrWhiteSpace(value)) {
			    return null;
		    }

		    return value;
	    }
    }
}
