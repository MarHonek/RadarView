using System.Collections.Generic;
using RadarView.Model.Entities.Geographic;

namespace RadarView.Model.Entities.AviationData.Airports
{
    /// <summary>
    /// Třída reprezentuje letiště.
    /// </summary>
    public class Airport
    {
		/// <summary>
		/// ID letiště v databázi.
		/// </summary>
        public int Id { get; set; }

		/// <summary>
		/// ICAO Identifikátor letiště.
		/// </summary>
		public string IcaoIdent { get; set; }

		/// <summary>
		/// Typ letiště.
		/// </summary>
		public AirportType AirportType { get; set; }

		/// <summary>
		/// Název letiště.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Zeměpisná poloha letiště.
		/// </summary>
		public Location Location { get; set; }

		/// <summary>
		/// Nadmořská výška (stopy).
		/// </summary>
		public int Altitude { get; set; }

		/// <summary>
		/// Zkratka kontinentu.
		/// </summary>
		public string Continent { get; set; }

		/// <summary>
		/// Zkratka země ve které se letiště nachází.
		/// </summary>
		public string Country { get; set; }

		/// <summary>
		/// Region země.
		/// </summary>
		public string Region { get; set; }

		/// <summary>
		/// Město ve kterém se letiště nachází.
		/// </summary>
		public string City { get; set; }

		/// <summary>
		/// Gps identifikátor.
		/// </summary>
		public string GpsCode { get; set; }

		/// <summary>
		/// IATA identifikátor.
		/// </summary>
		public string IataCode { get; set; }


		/// <summary>
		/// Lokalni identifikátor.
		/// </summary>
		public  string LocalCode { get; set; }

		/// <summary>
		/// Seznam vzletových/přistávacích drah.
		/// </summary>
		public List<Runway> Runway { get; set; } = new List<Runway>();


		protected bool Equals(Airport other)
		{
			return string.Equals(this.IcaoIdent, other.IcaoIdent);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return this.Equals((Airport) obj);
		}

		public override int GetHashCode()
		{
			return (this.IcaoIdent != null ? this.IcaoIdent.GetHashCode() : 0);
		}
    }
}
