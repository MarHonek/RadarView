using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadarView.Model.Entities.Aviation;
using RadarView.Model.Entities.AviationData;
using RadarView.Properties;

namespace RadarView.Model.Service.Utils
{
	/// <summary>
	/// Pomocná třída, která se stará o prací s prioritou datových zdrojů.
	/// </summary>
	public static class DataSourcePriorityHelper
	{	
		/// <summary>
		/// Priorita datové zdroje v textové podobě. Pořadí v souboru určuje prioritu. První datový zdroj je nejprioritnější.
		/// </summary>
		private static readonly StringCollection DataSourcePriority = Settings.Default.DataSourcePriority;

		/// <summary>
		/// Seznam deserializovaných priorit. Na prvním místě se nachází neprioritnější datový zdroj.
		/// </summary>
		private static List<AircraftDataSourceEnum> dataSourceEnumPriorities;


		static DataSourcePriorityHelper()
		{
			var dataSourceEnumHelper = new AircraftDataSourceEnumHelper();
			dataSourceEnumPriorities = new List<AircraftDataSourceEnum>();
			foreach (var dataSourceStringValue in DataSourcePriority) {
				dataSourceEnumPriorities.Add(dataSourceEnumHelper.ConvertToEnum(dataSourceStringValue));
			}

		}

		/// <summary>
		/// Vrátí seznam prioritizovaných datových zdrojů. První je nejprioritnější datový zdroj.
		/// </summary>
		/// <returns>Seznam datový zdrojů seřazené podle priority o největší po nejmněší.</returns>
		public static List<AircraftDataSourceEnum> GetListOfPrioritizedDataSources()
		{
			return new List<AircraftDataSourceEnum>(dataSourceEnumPriorities);
		}

		/// <summary>
		/// Vrátí datový zdroj podle čísla priority. 0 = nejprioritnější datový zdroj.
		/// Pokud je priorita mimo rozsah vrátí poslední nejprioritnější datový zdroj.
		/// </summary>
		/// <param name="priority">číslo priority datového zdroje.</param>
		/// <returns>Datový zdroj se zadanou prioritou.</returns>
		public static AircraftDataSourceEnum GetDataSourceByPriority(int priority)
		{
			return dataSourceEnumPriorities[priority];
		}

		/// <summary>
		/// Vrátí číslo priority podle zadaného datového zdroje.
		/// Priorita jde sestupně tj. 0 = nejvyšší priorita.
		/// </summary>
		/// <param name="dataSource">Datový zdroj pro který se zjišťuje priorita.</param>
		/// <returns>číslo priority datového zdroje.</returns>
		public static int GetPriorityByDataSource(AircraftDataSourceEnum dataSource)
		{
			return dataSourceEnumPriorities.IndexOf(dataSource);
		}

		/// <summary>
		/// Určí zda má první datový zdroj menší prioritu než druhý.
		/// </summary>
		/// <param name="first">První datový zdroj.</param>
		/// <param name="second">druhý datový zdroj.</param>
		/// <returns>True pokud má první datový zdroj menší prioritu než druhý, jinak false.</returns>
		public static bool IsPriorityLowerThan(AircraftDataSourceEnum first, AircraftDataSourceEnum second)
		{
			return GetPriorityByDataSource(first) > GetPriorityByDataSource(second);
		}
	}
}
