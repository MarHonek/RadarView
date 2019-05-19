using System;
using System.Globalization;

namespace OsnAPI.Utils
{
    /// <summary>
    /// Rozšiřující třída pro konverzi hodnot, které mohou být prázdné nebo null.
    /// </summary>
    public static class NullableTypesExtension
    {
	    ///<summary>
	    ///Konvertuje datový typ object na desetinné číslo typu float?.
	    ///</summary>
	    /// <param name="value">desetinné číslo jako hodnota datového typu object.</param>
	    ///<returns>Null jestliže je hodnotaje null, jinak hodnotu typu float.</returns>
	    ///<exception cref="InvalidCastException">jestliže hodnota není číslo.</exception>
	    internal static float? ParseToFloatOrNull(this object value)
	    {
		    if (value == null) {
			    return null;
		    }

		    return Convert.ToSingle(value, CultureInfo.InvariantCulture);
	    }


	    /// <summary>
	    /// Konvertuje datovy typ object na celé číslo typu nullable int?.
	    /// </summary>
	    /// <param name="value">celé číslo jako hodnota datového typu object.</param>
	    /// <returns>null jestliže je hodnota null, jinak hodnotu typu int?.</returns>
	    ///<exception cref="InvalidCastException">jestliže hodnota není číslo.</exception>
	    internal static int? ParseToIntOrNull(this object value)
	    {
		    if (value == null) {
			    return null;
		    }

		    Convert.ChangeType(value, TypeCode.Single, CultureInfo.InvariantCulture);
		    return Convert.ToInt32(value);
	    }

	    ///<summary> 
	    /// Konvertuje datovy typ object na celé číslo typu long?.
	    ///</summary>
	    /// <param name="value">celé číslo jako hodnota datového typu object.</param>
	    ///<returns>null jestliže je hodnota null, jinak hodnot typu float.</returns>
	    ///<exception cref="InvalidCastException">jestliže hodnota není číslo.</exception>
	    internal static long? ParseToLongOrNull(this object value)
	    {
		    if (value == null) {
			    return null;
		    }

		    return Convert.ToInt64(value);
	    }

	    ///<summary>
	    ///Konvertuje string, ktery obsahuje bílý znak, na hodnotu null.
	    ///</summary>
	    ///<param name="value">hodnota datového typu string.</param>
	    ///<returns>null jestliže string obsahuje pouze bíle znaky, jinak původní nezměněnou hodnotou.</returns>
	    internal static string NullIfWhiteSpace(this string value)
	    {
		    if (string.IsNullOrWhiteSpace(value)) {
			    return null;
		    }

		    return value;
	    }
    }
}
