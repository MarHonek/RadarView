using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadarView.Model.Entities.AviationData
{
	/// <summary>
	/// Třídá která zaobaluje informace o zdrojích polohových dat o letadlech.
	/// </summary>
    public class AircraftDataSourceWrapper
	{
		/// <summary>
		/// Kolekce obsahující informace o dostupnosti datových zdrojů.
		/// Klíč: Datový zdroj.
		/// Hodnota: příznak, zda je zdroj dostupný.
		/// </summary>
		private ConcurrentDictionary<AircraftDataSourceEnum, bool> dataSourceAvailability;

		/// <summary>
		/// Kolekce obsahující časový rozdíl mezi lokálním a globálním časem pro všechny datové zdroje.
		/// Klíč: Datový zdroj.
		/// Hodnota: časový rozdíl.
		/// </summary>
		private ConcurrentDictionary<AircraftDataSourceEnum, int?> dataSourcestimeOffset;


		/// <summary>
		/// Událost vyvolána při změně dostupnosti zdrojů polohových dat.
		/// </summary>
		public event EventHandler AvailabilityChanged;


		public AircraftDataSourceWrapper()
		{
			this.dataSourceAvailability = new ConcurrentDictionary<AircraftDataSourceEnum, bool>();
			foreach (var dataSource in (AircraftDataSourceEnum[])Enum.GetValues(typeof(AircraftDataSourceEnum))) {
				this.dataSourceAvailability.TryAdd(dataSource, false);
			}

			this.dataSourcestimeOffset = new ConcurrentDictionary<AircraftDataSourceEnum, int?>();
			foreach (var dataSource in (AircraftDataSourceEnum[])Enum.GetValues(typeof(AircraftDataSourceEnum))) {
				this.dataSourcestimeOffset.TryAdd(dataSource, null);
			}
		}

		/// <summary>
		/// Vrátí textový řetězec obsahující aktivní zdroje polohových dat.
		/// </summary>
		/// <returns>Textový řetězec obsahující informace o dostupnosti zdrojů polohých dat letadel.</returns>
		public string GetConnectionStatusString()
		{
			var connectionStatus = "";

			foreach (var dataSource in (AircraftDataSourceEnum[])Enum.GetValues(typeof(AircraftDataSourceEnum))) {
				this.dataSourceAvailability.TryGetValue(dataSource, out var isSourceOk);
				if (isSourceOk) {
					connectionStatus += dataSource.ToString() + " ";
				}
			}

			return connectionStatus;
		}

		/// <summary>
		/// Nastaví dostupnost pro zdroj polohových dat letadel.
		/// </summary>
		/// <param name="dataSource">Datový zdroj.</param>
		/// <param name="isAvailable">Dostupnost zdroje.</param>
		public void SetAvailability(AircraftDataSourceEnum dataSource, bool isAvailable)
		{
			this.dataSourceAvailability[dataSource] = isAvailable;
			this.AvailabilityChanged?.Invoke(this, new EventArgs());
		}

		/// <summary>
		/// Vrátí dostupnost zdroje polohých dat letadel.
		/// </summary>
		/// <param name="datasource">Datový zdroj.</param>
		/// <returns>true pokud je zdroj dostupný, jinak false.</returns>
		public bool GetAvailability(AircraftDataSourceEnum datasource)
		{
			return this.dataSourceAvailability[datasource];
		}

		/// <summary>
		/// Aktualizuje časový rozdíl mezi lokálním a globálním časem pro zadaný datový zdroj.
		/// </summary>
		/// <param name="dataSource">datový zdroj.</param>
		/// <param name="offset">časový posun.</param>
		public void UpdateTimeOffset(AircraftDataSourceEnum dataSource, int offset)
		{
			this.dataSourcestimeOffset[dataSource] = offset;
		}

		/// <summary>
		/// Vrátí vypočítaný rozdíl mezi lokálním a globálním časem.
		/// </summary>
		/// <returns>Časový posun lokálního vůči globálnímu času.</returns>
		public int? GetTimeOffset(AircraftDataSourceEnum dataSourceEnum)
		{
			return this.dataSourcestimeOffset[dataSourceEnum];
		}
	}
}
