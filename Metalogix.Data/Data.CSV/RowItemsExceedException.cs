using System;

namespace Metalogix.Data.CSV
{
    internal class RowItemsExceedException : CsvException
    {
        public RowItemsExceedException() : base(
            "The number of items in the row exceed the header count of the Csv Document.")
        {
        }
    }
}