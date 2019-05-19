using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarView.Model.Service.ApiRestClient
{
	/// <summary>
	/// Trida starajici se rest komunikaci se serverem.
	/// </summary>
	public interface IApiRestClient
	{
		/// <summary>
		/// Provede GET request.
		/// </summary>
		/// <param name="url">Url serveru.</param>
		/// <param name="resource">Adresa zdroju.</param>
		/// <param name="itemId">Hlavicka requestu.</param>
		/// <param name="body">Telo requestu</param>
		/// <returns>Odpoved na dotaz.</returns>
		Task<TItem> MakeGerRequestAsync<TItem>(string url, string resource, string itemId, object body);
	}
}
