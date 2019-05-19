using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using RadarView.Model.Entities.Exceptions;

namespace RadarView.Model.Service.UdpCommunicationClient
{
	/// <summary>
	/// <inheritdoc/>
	/// </summary>
	public class UdpCommunicationClient : IUdpCommunicationClient
	{
		/// <summary>
		/// UDP klient.
		/// </summary>
		private UdpClient udpClient;

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public int UdpPort { get; set; }

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public event EventHandler<UdpClientEventArgs> MessageReceived;

		/// <summary>
		/// <inheritdoc/>
		/// </summary>
		public void Initialize(int udpPort)
		{
			this.UdpPort = udpPort;
			this.udpClient = new UdpClient(this.UdpPort);
			this.Listen();
		}

		/// <summary>
		/// Začně naslouchat na zadaném portu.
		/// </summary>
		public void Listen()
		{
			try {
				
				this.udpClient.BeginReceive(new AsyncCallback(this.ReceiverCallback), null);
			} catch (Exception ex) {
				var errorMessage = "Chyba pri pokusu o UDP komunikaci. " + ex.StackTrace;
				Debug.WriteLine(errorMessage);
				throw new UdpClientException(errorMessage, ex.InnerException);
			}
		}

		/// <summary>
		/// Callback zpracuje příchozí data.
		/// </summary>
		private void ReceiverCallback(IAsyncResult res)
		{
			try {
				var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, this.UdpPort);
				var received = this.udpClient.EndReceive(res, ref remoteIpEndPoint);

				var message = Encoding.ASCII.GetString(received);
				this.MessageReceived?.Invoke(this, new UdpClientEventArgs(message));			

			} catch (Exception exception) {
				var errorMessage = "Došlo k chybě při zpracování UTP zprávy. " + exception.Message;
				Debug.WriteLine(errorMessage);
			}

			this.udpClient.BeginReceive(new AsyncCallback(this.ReceiverCallback), null);
		}
	}

	/// <summary>
	/// EventArgs pro předání zprávy získané z UDP komunikace.
	/// </summary>
	public class UdpClientEventArgs : EventArgs
	{
		/// <summary>
		/// Zpráva z UDP klienta.
		/// </summary>
		public string Message { get; set; }

		public UdpClientEventArgs(string message)
		{
			this.Message = message;
		}
	}
}
