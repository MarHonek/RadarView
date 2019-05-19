using RadarView.Model.Entities.Geographic;

namespace RadarView.Model.Entities.AviationData.Airports
{
	/// <summary>
	/// Třída reprezentuje vzletovou/přistávací dráhu.
	/// </summary>
	public class Runway
	{
		/// <summary>
		/// Id dráhy v databázi.
		/// </summary>
		public int Id { get; set; }

		/// <summary>
		/// Id letiště v databázi.
		/// </summary>
		public int AirportIdReference { get; set; }

		/// <summary>
		/// ICAO identifikátor letiště.
		/// </summary>
		public string AirportIcaoIdent { get; set; }

		/// <summary>
		/// Délka dráhy (stopy).
		/// </summary>
		public int Length { get; set; }

		/// <summary>
		/// Šířka dráhy (stopy).
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// Název povrchu.
		/// </summary>
		public string Surface { get; set; }

		/// <summary>
		/// Příznak určující zda je dráha osvětlena.
		/// </summary>
		public bool Lighted { get; set; }

		/// <summary>
		/// Příznak určující zda je dráha uzavřena.
		/// </summary>
		public bool Closed { get; set; }

		/// <summary>
		/// Název počátku dráhy.
		/// </summary>
		public string StartName { get; set; }

		/// <summary>
		/// Název konce dráhy.
		/// </summary>
		public string EndName { get; set; }

		/// <summary>
		/// Celý název dráhy.
		/// </summary>
		public string Name
		{
			get { return this.StartName + "/" + this.EndName; }
		}

		/// <summary>
		/// Zeměpisná poloha počátku dráhy.
		/// </summary>
		public Location StartLocation { get; set; }

		/// <summary>
		/// Zeměpisná poloha pocčátku dráhy.
		/// </summary>
		public Location EndLocation { get; set; }

		/// <summary>
		/// Nadmořská výška počátku dráhy (stopy).
		/// </summary>
		public int StartAltitude { get; set; }

		/// <summary>
		/// Nadmořska výška konce dráhy (stopy).
		/// </summary>
		public int EndAltitude { get; set; }

		/// <summary>
		/// Kurz počátku dráhy (stupně).
		/// </summary>
		public float StartHeading { get; set; }

		/// <summary>
		/// Kurz konce dráhy (stupně).
		/// </summary>
		public float EndHeading { get; set; }


		protected bool Equals(Runway other)
		{
			return this.Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return this.Equals((Runway) obj);
		}

		public override int GetHashCode()
		{
			return this.Id;
		}
	}
}
