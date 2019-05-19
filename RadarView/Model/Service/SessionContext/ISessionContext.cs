using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RadarView.Model.Entities.AviationData;
using RadarView.Model.Entities.AviationData.Airports;
using RadarView.Model.Entities.Geographic;
using RadarView.Model.Entities.MapLayer;

namespace RadarView.Model.Service.SessionContext
{
	/// <summary>
	/// Trida udrzujici informace o stavu sveta (aplikace).
	/// </summary>
	public interface ISessionContext
	{
		/// <summary>
		/// Aktualne nastavene letiste.
		/// </summary>
		Airport CurrentAirport { get; set; }

		/// <summary>
		/// Aktuálně nastavený výchozí střed (poloha).
		/// </summary>
		Location DefaultCenter { get; }

		/// <summary>
		/// Kolekce map, ktere lze zobrazit v aplikaci.
		/// </summary>
		MapLayerCollection MapLayerCollection { get; set; }

		/// <summary>
		/// Sledovana oblast.
		/// </summary>
		BoundingBox MonitoredArea { get; set; }

		/// <summary>
		/// Objekt udržuje informace o zdrojích polohových dat.
		/// </summary>
		AircraftDataSourceWrapper AircraftDataSourceWrapper { get; set; }

		/// <summary>
		/// Kód země pro výběr leteckých dat.
		/// Zatím pouze ČR
		/// </summary>
		string CountryCode { get; set; }

		/// <summary>
		/// Oblast letiště. Obsahuje nadmořskou výšku a stanovenou oblast.s
		/// </summary>
		Tuple<int, BoundingBox> AirportArea { get; set; }

		/// <summary>
		/// Kolekce obsahuje datové zdroje a číslo souboru, který má být zpětně přehrán.
		/// Klíč: datový zdroj.
		/// Hodnota: číslo souboru, který má být přehrán. Pokud je hodnota -1, soubor nemá být přehrán.
		/// </summary>
		Dictionary<AircraftDataSourceEnum, string> DataSourceReplayFiles { get; set; }

		/// <summary>
		/// Uloží data do konfigu.
		/// </summary>
		void SaveToConfig();
	}
}
