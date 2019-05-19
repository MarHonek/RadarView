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
	/// Vyjímka vyvolána při chybě při stahování obrázků ze serveru.
	/// </summary>
	public class ImageDownloadClientException : Exception
	{
		public ImageDownloadClientException()
		{
		}

		public ImageDownloadClientException(string message) : base(message)
		{
		}

		public ImageDownloadClientException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ImageDownloadClientException([NotNull] SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
