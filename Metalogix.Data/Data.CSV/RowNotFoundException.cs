using System;

namespace Metalogix.Data.CSV
{
    internal class RowNotFoundException : CsvException
    {
        public RowNotFoundException() : base("The specified row does not exist in the Csv Document.")
        {
        }
    }
}