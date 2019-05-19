using System;
using System.Collections.Generic;
using RadarView.Model.Entities.AviationData;

namespace RadarView.Model.Service.SessionContext
{
	/// <summary>
	/// Třída obsahující informace o datových zdrojích.
	/// </summary>
	public class DataSourceSessionContext
	{
		/// <summary>
		/// Kolekce obsahující časový rozdíl mezi lokálním a globálním časem pro všechny datové zdroje.
		/// Klíč: Datový zdroj.
		/// Hodnota: časový rozdíl.
		/// </summary>
		private Dictionary<AircraftDataSourceEnum, int> timeOffsetOfDataSources;

		/// <summary>
		/// Vytvoří novou instanci třídy DadaSourceSessionContext
		/// </summary>
		public DataSourceSessionContext() 
		{
			this.timeOffsetOfDataSources = new Dictionary<AircraftDataSourceEnum, int>();
			foreach (var dataSource in (AircraftDataSourceEnum[])Enum.GetValues(typeof(AircraftDataSourceEnum))) 
			{
				this.timeOffsetOfDataSources.Add(dataSource, 0);
			}
		}

		/// <summary>
		/// Aktualizuje časový rozdíl mezi lokálním a globálním časem pro zadaný datový zdroj.
		/// </summary>
		/// <param name="dataSource">datový zdroj.</param>
		/// <param name="offset">časový posun.</param>
		public void UpdateTimeOffset(AircraftDataSourceEnum dataSource, int offset) 
		{
			this.timeOffsetOfDataSources[dataSource] = offset;
		}


		/// <summary>
		/// Vrátí vypočítaný rozdíl mezi lokálním a globálním časem.
		/// </summary>
		/// <returns>Časový posun lokálního vůči globálnímu času.</returns>
		public int GetTimeOffset() 
		{
			return this.timeOffsetOfDataSources[AircraftDataSourceEnum.OSN];
		}
	}
}
