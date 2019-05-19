using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
	/// <summary>
	/// Vyjímka vyvolána pokud nebyl nalezen soubor s daty pro zpětné přehrávání.
	/// </summary>
    class FileForReplayDoNotExistsException : Exception
    {
        public FileForReplayDoNotExistsException() : base() { }

        public FileForReplayDoNotExistsException(string message) : base(message) { }
    }
}
