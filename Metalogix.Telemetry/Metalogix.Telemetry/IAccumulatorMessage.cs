using System;

namespace Metalogix.Telemetry
{
    public interface IAccumulatorMessage
    {
        Type AccumlatorType { get; }

        string AccumulatorName { get; }

        object Clone();

        void Send(Aggregator aggregator = null);

        object Value();
    }
}