using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using RadarView.Model.Entities.Json;
using RadarView.Model.Service.UdpCommunicationClient;

namespace RadarView.Model.DataService.Network
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class AfisStripDataService : IAfisStripDataService
	{
		/// <summary>
		/// Událost vyvolána při obdržení informací (z UDP klienta) o letadlech, které se mají zvýraznit.
		/// </summary>
		public event EventHandler<AircraftHighlightEventArgs> HighlightedAircraftReceived;

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public event EventHandler<ActiveAirspaceEventArgs> ActiveAirspaceReceived;


		private readonly IUdpCommunicationClient _udpCommunicationClient;


		public AfisStripDataService(IUdpCommunicationClient udpCommunicationClient)
		{
			this._udpCommunicationClient = udpCommunicationClient;
			this._udpCommunicationClient.MessageReceived += this._udpCommunicationClient_MessageReceived;
		}

		private void _udpCommunicationClient_MessageReceived(object sender, UdpClientEventArgs e)
		{
			try {
				var deserializedMessage = JsonConvert.DeserializeObject<AfisStripJsonMessage>(e.Message);
				if (deserializedMessage.ActiveAirspace != null) {
					this.ActiveAirspaceReceived?.Invoke(this,
						new ActiveAirspaceEventArgs(deserializedMessage.ActiveAirspace));
				}

				if (deserializedMessage.HighlightedAircraft != null) {
					this.HighlightedAircraftReceived?.Invoke(this,
						new AircraftHighlightEventArgs(deserializedMessage.HighlightedAircraft));
				}
			} catch (Exception) {
				Debug.WriteLine("Nepodařilo se deserializovat data z UPD klienta");
			}
		}

		/// <summary>
		/// Argumenty pro událost ActiveAirspaceReceived
		/// </summary>
		public class ActiveAirspaceEventArgs : EventArgs
		{
			/// <summary>
			/// Seznam obsahující názvy vzdušných prostorů.
			/// </summary>
			public List<string> AirspaceNames { get; }

			public ActiveAirspaceEventArgs(List<string> airspaceNames)
			{
				this.AirspaceNames = airspaceNames;
			}
		}

		/// <summary>
		/// Arguementy pro událost HighlightedTargetsReceived
		/// </summary>
		public class AircraftHighlightEventArgs : EventArgs
		{
			/// <summary>
			/// Kolekce obsahující identifikaci letadla a typ zvýraznění.
			/// Klíč: identifikace letadla.
			/// Hodnota: Typ zvýraznění.
			/// </summary>
			public Dictionary<string, TargetHighlight> ListOfAircraft { get; }

			public AircraftHighlightEventArgs(Dictionary<string, TargetHighlight> listOfAircraft)
			{
				this.ListOfAircraft = listOfAircraft;
			}
		}
	}
}
