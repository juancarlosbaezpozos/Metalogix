using System;

namespace Metalogix.Licensing.SK
{
    public class SeatsMismatchException : Exception
    {
        public SeatsMismatchException() : base(
            "The subscription ID you have entered has different seats count that the registered one on the license server.")
        {
        }
    }
}