using System.Collections.Generic;
using Newtonsoft.Json;
using UnitsNet;

namespace OgnAPI.Beacon
{
    /// <summary>
    /// Třída reprezentuje letadlo.
    /// </summary>
    public class AircraftBeacon : IAircraftBeacon
    {
        #region attributes

        /// <summary>
        /// <see cref="IAircraftBeacon.CallSign"/>
        /// </summary>
        protected string callSign;

        /// <summary>
        /// <see cref="IAircraftBeacon.Timestamp"/>
        /// </summary>
        protected long timestamp;

		/// <summary>
		/// <see cref="IAircraftBeacon.DstCall"/>
		/// </summary>
        protected string dstCall;

        /// <summary>
        /// <see cref="IAircraftBeacon.Latitude"/>
        /// </summary>
        protected double latitude;

        /// <summary>
        /// <see cref="IAircraftBeacon.Longitude"/>
        /// </summary>
        protected double longitude;

        /// <summary>
        /// <see cref="IAircraftBeacon.Altitude"/>
        /// </summary>
        protected float altitude;

        /// <summary>
        /// <see cref="IAircraftBeacon.Track"/>
        /// </summary>
        protected int? track;

        /// <summary>
        /// <see cref="IAircraftBeacon.GroundSpeed"/>
        /// </summary>
        protected float? groundSpeed;

        /// <summary>
        /// <see cref="IAircraftBeacon.RawPacket"/>
        /// </summary>
        protected string rawPacket;

        /// <summary>
        /// <see cref="IAircraftBeacon.ReceiverName"/>
        /// </summary>
        protected string receiverName;

        /// <summary>
        /// <see cref="IAircraftBeacon.Address"/>
        /// </summary>
        protected string address;

        /// <summary>
        /// <see cref="IAircraftBeacon.OriginalAddress"/>
        /// </summary>
        protected string originalAddress;

        /// <summary>
        /// <see cref="IAircraftBeacon.AddressType"/>
        /// </summary>
        protected AddressType addressType = AddressType.Unrecognized;

        /// <summary>
        /// <see cref="IAircraftBeacon.AircraftType"/>
        /// </summary>
        protected AircraftType aircraftType = AircraftType.Unknown;

        /// <summary>
        /// <see cref="IAircraftBeacon.Stealth"/>
        /// </summary>
        protected bool stealth;

        /// <summary>
        /// <see cref="IAircraftBeacon.NoTracking"/>
        /// </summary>
        protected bool noTracking;

        /// <summary>
        /// <see cref="IAircraftBeacon.ClimbRate"/>
        /// </summary>
        protected float? climbRate;

        /// <summary>
        /// <see cref="IAircraftBeacon.TurnRate"/>
        /// </summary>
        protected float? turnRate;

        /// <summary>
        /// <see cref="IAircraftBeacon.SignalStrength"/>
        /// </summary>
        protected float? signalStrength;

        /// <summary>
        /// <see cref="IAircraftBeacon.ERP"/>
        /// </summary>
        protected float? erp;

        /// <summary>
        /// <see cref="IAircraftBeacon.FrequencyOffset"/>
        /// </summary>
        protected float? frequencyOffset;

        /// <summary>
        /// <see cref="IAircraftBeacon.GpsStatus"/>
        /// </summary>
        protected string gpsStatus;

        /// <summary>
        /// <see cref="IAircraftBeacon.ErrorCount"/>
        /// </summary>
        protected int? errorCount;

        /// <summary>
        /// <see cref="IAircraftBeacon.HardwareVersion"/>
        /// </summary>
        protected int? hardwareVersion;

        /// <summary>
        /// <see cref="IAircraftBeacon.FirmwareVersion"/>
        /// </summary>
        protected float? firmwareVersion;

        /// <summary>
        /// <see cref="IAircraftBeacon.HeardAircraftIds"/>
        /// </summary>
        protected List<string> heardAircraftIds = new List<string>();

        /// <summary>
        /// <see cref="IAircraftBeacon.FlightLevel"/>
        /// </summary>
        protected float? flightLevel;      
        #endregion

        #region properties

        /// <summary>
        /// <see cref="callSign"/>
        /// </summary>
        public string CallSign
        {
            get { return this.callSign; }
        }

        /// <summary>
        /// <see cref="timestamp"/>
        /// </summary>
        public long Timestamp
        {
            get { return this.timestamp; }
            set { this.timestamp = value; }
        }

        /// <summary>
        /// <see cref="latitude"/>
        /// </summary>
        public double Latitude
        {
            get { return this.latitude; }
        }

        /// <summary>
        /// <see cref="longitude"/>
        /// </summary>
        public double Longitude
        {
            get { return this.longitude; }
        }

        /// <summary>
        /// <see cref="altitude"/>
        /// </summary>
        public float Altitude
        {
	        get
	        {
		        return this.altitude;
	        }
        }

        /// <summary>
        /// <see cref="track"/>
        /// </summary>
        public int? Track
        {
            get { return this.track; }
        }

        /// <summary>
        /// <see cref="groundSpeed"/>
        /// </summary>
        public float? GroundSpeed
        {
	        get
	        {
		        return this.groundSpeed;
	        }
        }

        /// <summary>
        /// <see cref="rawPacket"/>
        /// </summary>
        public string RawPacket
        {
            get { return this.rawPacket; }
        }

        /// <summary>
        /// <see cref="receiverName"/>
        /// </summary>
        public string ReceiverName
        {
            get { return this.receiverName; }
        }

        /// <summary>
        /// <see cref="address"/>
        /// </summary>
        public string Address
        {
            get { return this.address; }
        }

        /// <summary>
        /// <see cref="originalAddress"/>
        /// </summary>
        public string OriginalAddress
        {
            get { return this.originalAddress; }
        }

        /// <summary>
        /// <see cref="addressType"/>
        /// </summary>
        public AddressType AddressType
        {
            get { return this.addressType; }
        }

        /// <summary>
        /// <see cref="aircraftType"/>
        /// </summary>
        public AircraftType AircraftType
        {
            get { return this.aircraftType; }
        }

        /// <summary>
        /// <see cref="stealth"/>
        /// </summary>
        public bool Stealth
        {
            get { return this.stealth; }
        }

        /// <summary>
        /// <see cref="noTracking"/>
        /// </summary>
        public bool NoTracking
        {
            get { return this.noTracking; }
        }

        /// <summary>
        /// <see cref="climbRate"/>
        /// </summary>
        public float? ClimbRate
        {
	        get
	        {
		        return this.climbRate;
	        }
        }

        /// <summary>
        /// <see cref="turnRate"/>
        /// </summary>
        public float? TurnRate
        {
            get { return this.turnRate; }
        }

        /// <summary>
        /// <see cref="signalStrength"/>
        /// </summary>
        public float? SignalStrength
        {
            get { return this.signalStrength; }
        }

        /// <summary>
        /// <see cref="frequencyOffset"/>
        /// </summary>
        public float? FrequencyOffset
        {
            get { return this.frequencyOffset; }
        }

        /// <summary>
        /// <see cref="gpsStatus"/>
        /// </summary>
        public string GpsStatus
        {
            get { return this.gpsStatus; }
        }

        /// <summary>
        /// <see cref="errorCount"/>
        /// </summary>
        public int? ErrorCount
        {
            get { return this.errorCount; }
        }

        /// <summary>
        /// <see cref="firmwareVersion"/>
        /// </summary>
        public float? FirmwareVersion
        {
            get { return this.firmwareVersion; }
        }

        /// <summary>
        /// <see cref="heardAircraftIds"/>
        /// </summary>
        public string[] HeardAircraftIds
        {
            get
            {
                string[] array = null;
                if (this.heardAircraftIds != null) {
	                array = new string[this.heardAircraftIds.Count];
	                this.heardAircraftIds.CopyTo(array);
                }

                return array;
            }
        }

        /// <summary>
        /// <see cref="erp"/>
        /// </summary>
        public float? ERP
        {
            get { return this.erp; }
        }

        /// <summary>
        /// <see cref="flightLevel"/>
        /// </summary>
        public float? FlightLevel
        {
            get { return this.flightLevel; }
        }

        /// <summary>
        /// <see cref="hardwareVersion"/>
        /// </summary>
        public int? HardwareVersion
        {
            get { return this.hardwareVersion; }
        }
		#endregion

		[JsonConstructor]
        public AircraftBeacon(string callSign, long timestamp, double latitude, double longitude, float altitude, int? track, float? groundSpeed, string rawPacket, string receiverName,
            string address, string originalAddress, AddressType addressType, AircraftType aircraftType, bool stealth, bool noTracking, float? climbRate, float? turnRate, float? signalStrength,
            float? erp, float? frequencyOffset, string gpsStatus, int? errorCount, int? hardwareVersion, float? firmwareVersion, List<string> heardAircraftIds, float? flightLevel)
        {
            this.callSign = callSign;
            this.timestamp = timestamp;
            this.latitude = latitude;
            this.longitude = longitude;
            this.altitude = altitude;
            this.track = track;
            this.groundSpeed = groundSpeed;
            this.rawPacket = rawPacket;
            this.receiverName = receiverName;
            this.address = address;
            this.originalAddress = originalAddress;
            this.addressType = addressType;
            this.aircraftType = aircraftType;
            this.stealth = stealth;
            this.noTracking = noTracking;
            this.climbRate = climbRate;
            this.turnRate = turnRate;
            this.signalStrength = signalStrength;
            this.erp = erp;
            this.frequencyOffset = frequencyOffset;
            this.gpsStatus = gpsStatus;
            this.errorCount = errorCount;
            this.hardwareVersion = hardwareVersion;
            this.firmwareVersion = firmwareVersion;
            this.heardAircraftIds = heardAircraftIds;
            this.flightLevel = flightLevel;
        }

        public AircraftBeacon() { }
          
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;

            var beacon = (IAircraftBeacon)obj;
            return this.CallSign == beacon.CallSign;
        }

        public override int GetHashCode()
        {
            return this.CallSign.GetHashCode();
        }

        public override string ToString()
        {
            return "CallSign:" + this.CallSign +
            "; Address:" + this.Address +
            "; TimeStamp:" + this.Timestamp +
            "; ReceiverName:" + this.ReceiverName +
            "; Longitude:" + this.Longitude +
            "; Latitude:" + this.Latitude +
            "; Alt:" + this.Altitude + 
            "; Track:" + this.Track +
            "; AddressType:" + this.AddressType.ToString() +
            "; AircraftType:" + this.AircraftType.ToString() +
            "; FrequencyOffset:" + this.FrequencyOffset +
            "; Firmware:" + this.FirmwareVersion +
            "; Hardware:" + this.HardwareVersion +           
            "; ErrorCount:" + this.ErrorCount +
            "; Stealth:" + this.Stealth +
            "; NoTracking:" + this.NoTracking +
            "; TurnRate:" + this.TurnRate +
            "; ClimbRate:" + this.ClimbRate +
            "; FlighLevel:" + this.FlightLevel +
            "; ERP:" + this.ERP +
            "; GpsStatus:" + this.GpsStatus +
            "; RawPacket:" + this.RawPacket;
        }
    }
}
