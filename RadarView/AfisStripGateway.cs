using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RadarView
{
    /// <summary>
    /// Reprezentuje rozhraní pro komunikaci s aplikací 3. strany
    /// </summary>
    public sealed class AfisStripGateway
    {
        /// <summary>
        /// Port pro zasílání informací o vybraných letadlech, které se mají zvýraznit.
        /// </summary>
        private readonly int UtpPort = Properties.Settings.Default.AfisStripUtpPort;

        /// <summary>
        /// UDP klient.
        /// </summary>
        private UdpClient afisStripUdpClient;

        /// <summary>
        /// Událost vyvolána při obdržení informací (z UDP klienta) o letadlech, které se mají zvýraznit.
        /// </summary>
        public event EventHandler<AircraftHighlightEventArgs> HighlightedAircraftsReceived;

        /// <summary>
        /// Událost vyvolána při obdržení informací (z UDP klienta) o aktivních vzdušných prostorech. 
        /// </summary>
        public event EventHandler<ActiveAirspaceEventArgs> ActiveAirspacesReceived;


        //SingleTon pattern
        private static readonly Lazy<AfisStripGateway> lazy =
        new Lazy<AfisStripGateway>(() => new AfisStripGateway());

        public static AfisStripGateway Instance { get { return lazy.Value; } }

        private AfisStripGateway()
        {
            afisStripUdpClient = new UdpClient(UtpPort);
        }

        /// <summary>
        /// Začně naslouchat na UDP portu.
        /// </summary>
        public void Listen()
        {
            try
            {
                afisStripUdpClient.BeginReceive(new AsyncCallback(ReceiverCallback), null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Nepodařilo se spustit naslouchání na UTP v AfisStripGateway. " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Callback zpracuje příchozí data.
        /// </summary>
        private void ReceiverCallback(IAsyncResult res)
        {
            try
            {
                IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, UtpPort);
                byte[] received = afisStripUdpClient.EndReceive(res, ref RemoteIpEndPoint);

                string message = Encoding.ASCII.GetString(received);
                if (!string.IsNullOrEmpty(message))
                {
                    AfisStripJsonMessage deserializedMessage = JsonConvert.DeserializeObject<AfisStripJsonMessage>(message);
                    if (deserializedMessage.ActiveAirspaces != null)
                    {
                        ActiveAirspacesReceived?.Invoke(this, new ActiveAirspaceEventArgs(deserializedMessage.ActiveAirspaces));
                    }

                    if (deserializedMessage.HighlightedAircrafts != null)
                    {
                        HighlightedAircraftsReceived?.Invoke(this, new AircraftHighlightEventArgs(deserializedMessage.HighlightedAircrafts));
                    }
                }
            }
            catch (JsonReaderException deserializationException)
            {
                Debug.WriteLine("UTP zpráva zaslaná do AfisStripGateWay nemá správný formát. Chyba:" + deserializationException.Message);
            } catch (JsonSerializationException deserializationException)
            {
                Debug.WriteLine("UTP zpráva zaslaná do AfisStripGateWay nemá správný formát. Chyba:" + deserializationException.Message);
            }
            catch (Exception exception)
            {
                Debug.WriteLine("Došlo k chybě při zpracování UTP zprávy v AfisStripGateWay. " + exception.Message);
            }

            afisStripUdpClient.BeginReceive(new AsyncCallback(ReceiverCallback), null);
        }

    }

    /// <summary>
    /// Argumenty pro událost ActiveAirspacesReceived
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
        public Dictionary<string, TargetHighlight> Aircrafts { get; }

        public AircraftHighlightEventArgs(Dictionary<string, TargetHighlight> aircrafts)
        {
            this.Aircrafts = aircrafts;
        }
    }

    /// <summary>
    /// Třída reprezentuje typ příchozí zprávy ve formátu JSON.
    /// </summary>
    public class AfisStripJsonMessage
    {
        /// <summary>
        /// Seznam identifikátorů aktivních prostorů.
        /// </summary>
        public List<string> ActiveAirspaces { get; }

        /// <summary>
        /// Kolekce obsahující identifikátory letadel a jejich zvýraznění.
        /// Klíč: Identifikátor letadel.
        /// Hodnota: Typ zvýraznění.
        /// </summary>
        public Dictionary<string, TargetHighlight> HighlightedAircrafts { get; }

        public AfisStripJsonMessage(List<string> ActiveAirspaces, Dictionary<string, TargetHighlight> highlightedAircrafts)
        {
            this.ActiveAirspaces = ActiveAirspaces;
            this.HighlightedAircrafts = highlightedAircrafts;
        }
    }
}
