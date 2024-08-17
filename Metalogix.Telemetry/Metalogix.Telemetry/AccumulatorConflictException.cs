using System;

namespace Metalogix.Telemetry
{
    public class AccumulatorConflictException : Exception
    {
        private readonly string _accumulatorName;

        public string AccumulatorName
        {
            get { return this._accumulatorName; }
        }

        public override string Message
        {
            get
            {
                return string.Concat("An Accumulator already exists with the name \"", this._accumulatorName,
                    "\" but is of an incompatible Accumulator Type.");
            }
        }

        public AccumulatorConflictException(string accumulatorName)
        {
            this._accumulatorName = accumulatorName;
        }
    }
}