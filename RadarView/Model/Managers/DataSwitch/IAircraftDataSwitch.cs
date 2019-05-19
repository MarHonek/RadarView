using RadarView.Model.Entities.Aviation.Interface;

namespace RadarView.Model.Managers.DataSwitch
{
	/// <summary>
	/// Reprezentuje přepínač leteckých datových zdrojů.
	/// Přípánač může měnit zdroj dat a zároveň zajistit ukládání dat do textového souboru
	/// nebo odesílaní dat na vzdálený server.
	/// TODO: Aktuálně se jedná pouze o přípravu třídy pro budoucí rozšíření.
	/// </summary>
	public interface IAircraftDataSwitch : IRenderingSample
	{
	}
}
