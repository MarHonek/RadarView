using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadarView.Model.Entities.Aviation.Interface;
using RadarView.Model.Entities.AviationData;
using RadarView.Model.Entities.Geographic;

namespace RadarView.Model.Managers.LiveDataManager
{
	/// <summary>
	/// Třída shromažďuje surová polohová data ze serveru a pomocí prediktoru vypočítává polohu, která bude zobrazena.
	/// </summary>
	public interface IAircraftLiveDataManager : IRenderingSample
	{
		/// <summary>
		/// Smaže letadla, o kterých byly data získávána ze specifikovaného datového zdroje.
		/// </summary>
		/// <param name="dataSource">Datový zdroj pro který se mají smazat letadla.</param>
		void RemoveAircraftFromDataSource(AircraftDataSourceEnum dataSource);
	}
}
