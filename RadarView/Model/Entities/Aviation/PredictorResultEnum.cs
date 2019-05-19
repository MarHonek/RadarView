namespace RadarView.Model.Entities.Aviation
{
	/// <summary>
	/// Výčet výsledků prediktoru.
	/// </summary>
	public enum PredictorResultEnum
	{
		/// <summary>
		/// Příznak, zda má být letadlo zobrazeno.
		/// </summary>
		Show,
		
		/// <summary>
		/// Příznak, zda má být letadlo schováno, ale stále o něm mají být sbírány informace.
		/// </summary>
		Hide,
		
		/// <summary>
		/// Příznak, zda má být letadlo nezobraznovano a zároveň mají být odstraněny všechny informace o letadle.
		/// </summary>
		Remove
	}
}
