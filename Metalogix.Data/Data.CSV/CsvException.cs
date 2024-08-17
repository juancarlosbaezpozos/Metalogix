using System;

namespace Metalogix.Data.CSV
{
    internal class CsvException : Exception
    {
        public CsvException(string message) : base(message)
        {
        }
    }
}