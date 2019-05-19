using System;

namespace RadarView.Model.Service.UdpCommunicationClient
{
	/// <summary>
	/// Klient pro komunikaci s aplikacemi 3. stran pomocí UDP protokolu.
	/// </summary>
	public interface IUdpCommunicationClient
	{
		/// <summary>
		/// Událost vyvolána pokud klient obdrží UDP zprávu.
		/// </summary>
		event EventHandler<UdpClientEventArgs> MessageReceived;

		/// <summary>
		/// Inicializuje klienta.
		/// </summary>
		/// <param name="udpPort">UDP port</param>
		void Initialize(int udpPort);

		/// <summary>
		/// Síťový port použitý pro komunikaci.
		/// </summary>
		int UdpPort { get; set; }

		/// <summary>
		/// Začne naslouchat na zvoleném portu.
		/// </summary>
		void Listen();
	}
}
