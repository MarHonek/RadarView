using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using RadarView.Annotations;

namespace RadarView.Model.Entities.Exceptions
{
	/// <summary>
	/// Vyjímka vyvolána pokud došlo k chybě při UDP komunikaci skrze UDP clienta.
	/// </summary>
	public class UdpClientException : Exception
	{
		public UdpClientException()
		{
		}

		public UdpClientException(string message) : base(message)
		{
		}

		public UdpClientException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected UdpClientException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
