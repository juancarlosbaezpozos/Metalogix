using System;

namespace Metalogix.Licensing.SK
{
    public class QuotaExceededException : Exception
    {
        public QuotaExceededException() : base(
            "Associating the subscription to given license key will exceed the licensed seat count. You need to buy more seats to the license key to associate this subscription.")
        {
        }
    }
}