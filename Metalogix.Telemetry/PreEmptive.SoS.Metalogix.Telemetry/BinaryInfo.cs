using PreEmptive.SoS.Client.Messages;
using System;

namespace PreEmptive.SoS.Metalogix.Telemetry
{
    internal sealed class BinaryInfo
    {
        public static BinaryInformation Get()
        {
            return new BinaryInformation(new Guid("00000000-0000-0000-0000-000000000000"), "Metalogix.Telemetry",
                "8.3.0.3", "6/14/2017 2:36:09 PM");
        }
    }
}