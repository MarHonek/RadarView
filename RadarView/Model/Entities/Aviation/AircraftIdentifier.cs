using System;
using System.Collections.Specialized;
using System.Linq;
using RadarView.Model.Entities.AviationData;
using RadarView.Model.Service.Utils;
using RadarView.Properties;

namespace RadarView.Model.Entities.Aviation
{
	/// <summary>
	/// Reprezentuje unikátní označení letadla.
	/// </summary>
	public class AircraftIdentifier
	{
		/// <summary>
		/// Unikátní vygenerované Id letadla.
		/// </summary>
		private string uniqueId;

		/// <summary>
		/// Zpětná reference na rawData.
		/// Slouží k získávání informací pro zlepšení detekce duplikátních letadel.
		/// </summary>
		private AircraftRawData rawDataRef;

		/// <summary>
		/// Velikost oblasti pro kterou jsou letadla z různých zdrojů a se stejným squawkem prohlášena za jedno (metry).
		/// </summary>
		private readonly float SquawkAreaThreshold = Settings.Default.PredictorSquawkAreaThreshold;

		/// <summary>
		/// Datový zdroj s největší prioritou od kterého jsme získali data.
		/// </summary>
		private AircraftDataSourceEnum maxPriorityDataSource;

		/// <summary>
		/// Registracní kod letadla.
		/// </summary>
		public string Registration { get; set; }

		/// <summary>
		/// Adresa zařízení přijímací vysílající informace o letadle.
		/// Může byt kód módu S, identifikator zařízení FLARM nebo OGN Tracker.
		/// </summary>
		public string Address { get; set; }

		/// <summary>
		/// Volací znak letadla.
		/// </summary>
		public string CallSign { get; set; }

		/// <summary>
		/// Kód odpovídače v módu A/C.
		/// </summary>
		public string Squawk { get; set; }

		/// <summary>
		/// IATA číslo letu.
		/// </summary>
		public string IataFlightNumber { get; set; }


		/// <summary>
		/// Vytvoří novou instanci třídy <see cref="AircraftIdentifier"/>
		/// </summary>
		/// <param name="registration">Registrační kód letadla.</param>
		/// <param name="address">Kód módu S.</param>
		/// <param name="callSign">Volačka.</param>
		/// <param name="IataFlightNumber">IATA číslo letu.</param>
		/// <param name="squawk">kód Squawk.</param>
		/// <param name="dataSource">Datový zdroj ze kterého pochází informace.</param>
        public AircraftIdentifier(string registration, string address, string callSign, string IataFlightNumber, string squawk, AircraftRawData rawDataRef, AircraftDataSourceEnum dataSource)
        {
            this.Registration = registration;
            this.Address = address;
            this.CallSign = callSign;
			this.IataFlightNumber = IataFlightNumber;
            this.Squawk = squawk;
			this.rawDataRef = rawDataRef;
			this.maxPriorityDataSource = dataSource;
		}

		/// <summary>
		/// Vygeneruje unikátní ID.
		/// </summary>
		public void GenerateUniqueId()
		{
			this.uniqueId = Guid.NewGuid().ToString();
		}

        /// <summary>
        /// Určí zda letadlo obsahuje zadaný identifikátor.
        ///
        /// Stringove porovnání probíhá na UPPER-CASE stringoch, takže je Case-Insensitive! 
        /// </summary>
        /// <param name="id">identifikátor letadla. Idenfikátor může být volací znak, kód v módu S nebo registračka.</param>
        /// <returns>true pokud letadlo obsahuje zadaný identifikátor, jinak false.</returns>
        public bool Contains(string id)
        {
			if (string.IsNullOrEmpty(id)) {
				return false;
			}

	        var upperId = id.ToUpperInvariant();

			if (!string.IsNullOrWhiteSpace(this.Registration) && this.Registration.ToUpperInvariant() == upperId) {
				return true;
			}

			if (!string.IsNullOrWhiteSpace(this.CallSign) && this.CallSign.ToUpperInvariant() == upperId) {
				return true;
			}

			if (!string.IsNullOrWhiteSpace(this.Address) && this.Address.ToUpperInvariant() == upperId) {
				return true;
			}

			if (!string.IsNullOrWhiteSpace(this.IataFlightNumber) && this.IataFlightNumber.ToUpperInvariant() == upperId) {
				return true;
			}

			return false;
        }

		/// <summary>
		/// Zkombinuje identifikátory. Pokud cílový identifikátor nemá hodnotu (je null) doplní ji ze zdrojového identifikátoru.
		/// </summary>
		/// <param name="source">nová množina identifikátorů</param>
		/// <param name="dataSource">datový zdroj.</param>
		public void Combine(AircraftIdentifier source, AircraftDataSourceEnum dataSource)
		{
			var priorityIsLower = DataSourcePriorityHelper.IsPriorityLowerThan(dataSource, this.maxPriorityDataSource);

			var id = typeof(AircraftIdentifier);
            var properties = id.GetProperties().Where(prop => prop.CanRead && prop.CanWrite);
            foreach (var prop in properties) {
                var value = prop.GetValue(source);
                if (value != null) {
					var currentValue = prop.GetValue(this);
					if (currentValue is string && !string.IsNullOrEmpty(currentValue as string) && priorityIsLower) {
						continue;
					}
                    prop.SetValue(this, value);
					this.maxPriorityDataSource = dataSource;
                }
            }
        }

		/// <summary>
		/// Určí zda jsou identifikátory porovnatelné.
		/// Identifikátor nesmí být null ani prázdný řetězec.
		/// </summary>
		/// <param name="currentId">aktuální identifikátor.</param>
		/// <param name="newId">nový identifikátor.</param>
		/// <returns>true pokud jsou porovnatelné, jinak false.</returns>
        private bool IsComparable(string currentId, string newId)
        {
			return !string.IsNullOrEmpty(currentId) && !string.IsNullOrEmpty(newId);
        }

		/// <summary>
		/// Určí jestli jsou letadla z různých datových zdrojů dostatečně blízko pro to, aby mohla být prohlášeno, že se jedná o jedno a to samé letadlo.
		/// Hranice maximální vzdálenosti mezi letadly je dán konstantou <see cref="SquawkAreaThreshold"/>.
		/// </summary>
		/// <param name="newStateVector">nový stavový vektor.</param>
		/// <returns>true pokud jsou letadla dostatečně blízko sebe, jinak false.</returns>
		private bool IsAircraftCloseEnough(AircraftStateVector newStateVector)
		{
			var stateVectorNearestToExists = this.rawDataRef.GetStateVectorOrPrevious(newStateVector.RealFix.DateTime);
			if(stateVectorNearestToExists == null) {
				stateVectorNearestToExists = this.rawDataRef.ListAllStateVectors().FirstOrDefault();
			}
			var distanceBetweenStateVectors = stateVectorNearestToExists.RealFix.Location.GetDistanceTo(newStateVector.RealFix.Location);
			return distanceBetweenStateVectors <= this.SquawkAreaThreshold;
		}


		/// <summary>
		/// Porovná identifikátory letadla. Pokud se rovnají vyhodnotí, že jde o jedno a to samé letadlo.
		/// </summary>
		/// <param name="obj">Identifikátor k porovnání.</param>
		/// <returns>true pokud se letadla rovnají, jinak false.</returns>
		public override bool Equals(object obj)
		{
			var id = obj as AircraftIdentifier;

			if (id == null) {
				return false;
			}

			//Unikátní (vygenerované) Id se používá pro porovnání 'render targetu' při vykreslení.
			//Pokud již bylo letadlo v minulé periodě vykresleno, nevytváří se nový 'render target', ale existující se pouze aktualizuje (překreslí).
			//Při slučování letadel je uniqueID nového letadla vždy null, takže je vysledek vždy false a pro porovnání se tedy nepoužívá.
			if (this.uniqueId == id.uniqueId) {
				return true;
			}

			if (this.IsComparable(this.Address, id.Address)) {
				return this.Address == id.Address;
			}

			if (this.IsComparable(this.IataFlightNumber, id.IataFlightNumber)) {
				return this.IataFlightNumber == id.IataFlightNumber;
			}

			if (this.IsComparable(this.CallSign, id.CallSign)) {
				return this.CallSign == id.CallSign;
			}

			//Z pozorování vychází, že jedním squawkem mohou být označeno více letadel.
			//K porovnání se tedy využivá i jejich poloha.
			if (this.IsComparable(this.Squawk, id.Squawk)) {
				return this.Squawk == id.Squawk && this.IsAircraftCloseEnough(id.rawDataRef.ListAllStateVectors().LastOrDefault());
			}

			if (this.IsComparable(this.Registration, id.Registration)) {
				return this.Registration == id.Registration;
			}

			return false;
		}

		/// <summary>
		/// Vrací hash identifikátoru letadla.
		/// Pozor: Jelikož možné určit, který z identifikátorů nebude prázdný, porovnání na základě hashe probíhá přes unikátní (vygenerované) Id letadla.
		/// </summary>
		/// <returns>Hash identifikátoru.</returns>
        public override int GetHashCode()
        {
			return this.uniqueId.GetHashCode();
        }

		public override string ToString()
		{
			return "Mode S: " + this.Address + ", IataFlightNumber: " + this.IataFlightNumber +  ", CallSign: " + this.CallSign + ", Registration: " + this.Registration + ", Squawk: " + this.Squawk; 
		}
	}
}
