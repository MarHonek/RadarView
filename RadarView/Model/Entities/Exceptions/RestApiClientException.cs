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
	/// Vyjimka pro oznaceni chyby pri komunikaci pres Rest klienta.
	/// </summary>
	public class RestApiClientException : Exception
	{
		public RestApiClientException()
		{
		}

		public RestApiClientException(string message) : base(message)
		{
		}

		public RestApiClientException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected RestApiClientException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
