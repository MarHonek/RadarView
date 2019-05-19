using System.Collections.Generic;
using System.Linq;
using RadarView.Model.Entities.AviationData;

namespace RadarView.Model.Entities.Aviation
{
    /// <summary>
    /// Kolekce pro uložení surových dat o letadlech (z datových zdrojů).
    /// </summary>
    public class AircraftRawDataCollection : List<AircraftRawData>
    {
	    /// <summary>
	    /// Indexer
	    /// </summary>
	    /// <param name="id">Indentifikátor letadla.</param>
	    /// <returns>Surová data pro letadlo se zadaným identifikátorem.</returns>
	    public AircraftRawData this[AircraftIdentifier id]
	    {
		    get { return this.FirstOrDefault(x => x.Id.Equals(id)); }
		    set
		    {
			    var aircraft = this.Find(x => x.Id.Equals(id));
			    if (aircraft == null) {
				    //Letadlo v kolekci nebylo nalezeno.
				    var newAircraft = value;
					newAircraft.Id.GenerateUniqueId();
					this.Add(newAircraft);
				} else {
				    //Letadlo se již v kolekci nachází.	
				    aircraft.AddData(value);
			    }
		    }
	    }

	    /// <summary>
	    /// Z důvodu, že datové zdroje mohou jednou za čas poskytnout chybná data (obsahuje chybné identifikátory), která způsobí vytvoření duplikátů.
	    /// Duplikáty jsou sjednoceny a smazány.
	    /// </summary>
	    public void MergeDuplicates()
	    {
		    for (var i = 0; i < this.Count; i++) {
			    for (var j = this.Count - 1; j >= 0; j--) {

				    var aircraft = this[i];
				    var duplicate = this[j];

				    if (aircraft.Id.Equals(duplicate.Id) && i != j) {
					    aircraft.MergeDuplicates(duplicate);
					    this.RemoveAt(j);
				    }
			    }
		    }
	    }

	    /// <summary>
	    /// Odstraní všechny informace o letadlech získané ze zadaného datového zdroje.
	    /// </summary>
	    /// <param name="dataSource">Zdroj informací o letadle.</param>
	    public void RemoveAircraftFromSource(AircraftDataSourceEnum dataSource)
	    {
		    for (var i = this.Count - 1; i >= 0; i--) {
			    this[i].RemoveDataSource(dataSource);
			    if (this[i].ListParticipatingDataSources().Count <= 0) {
				    this.Remove(this[i]);
			    }
		    }
	    }
    }
}
