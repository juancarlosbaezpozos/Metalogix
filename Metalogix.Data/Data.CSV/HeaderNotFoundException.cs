using System;

namespace Metalogix.Data.CSV
{
    internal class HeaderNotFoundException : CsvException
    {
        public HeaderNotFoundException() : base("The specified header does not exist in the Csv Document.")
        {
        }
    }
}