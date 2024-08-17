using System;

namespace Metalogix.Data.CSV
{
    internal class DuplicateHeaderException : CsvException
    {
        public DuplicateHeaderException(string header) : base(
            string.Concat("The specified header already exists in the Csv Document: ", header))
        {
        }
    }
}