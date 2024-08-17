using System;

namespace Metalogix.Licensing.SK
{
    public class AlreadyAssociatedException : Exception
    {
        public AlreadyAssociatedException() : base(
            "The subscription or subscription ID you have entered is already associated.")
        {
        }
    }
}