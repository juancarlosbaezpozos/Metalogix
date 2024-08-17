using System;

namespace Metalogix.Telemetry
{
    public class InvalidTelemetryStateException : Exception
    {
        public InvalidTelemetryStateException(string message) : base(message)
        {
        }
    }
}