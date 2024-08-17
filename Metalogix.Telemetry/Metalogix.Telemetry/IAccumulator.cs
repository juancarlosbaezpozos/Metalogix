using System;

namespace Metalogix.Telemetry
{
    public interface IAccumulator
    {
        bool Modified { get; }

        string Name { get; }

        void Accumulate(object value);

        string PeekAsString();

        string ReadAsString();

        void Reset();
    }
}