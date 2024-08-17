using Metalogix.Telemetry;
using System;
using System.Threading;

namespace Metalogix.Telemetry.Accumulators
{
    public class FloatAccumulator : Accumulator<float, float>
    {
        private volatile bool _modified;

        private float _currentTotal;

        public override bool Modified
        {
            get { return this._modified; }
        }

        public FloatAccumulator(string name) : base(name)
        {
        }

        public override void Accumulate(float value)
        {
            float single;
            do
            {
                single = this._currentTotal;
            } while (!single.Equals(Interlocked.CompareExchange(ref this._currentTotal, single + value, single)));

            this._modified = true;
        }

        public override float Peek()
        {
            return Interlocked.CompareExchange(ref this._currentTotal, 0f, 0f);
        }

        public override float Read()
        {
            float single = Interlocked.CompareExchange(ref this._currentTotal, 0f, this._currentTotal);
            this._modified = false;
            return single;
        }

        public override void Reset()
        {
            Interlocked.Exchange(ref this._currentTotal, 0f);
            this._modified = false;
        }

        public class Message : AccumulatorMessage<float>
        {
            private readonly static Type AType;

            public override Type AccumlatorType
            {
                get { return FloatAccumulator.Message.AType; }
            }

            static Message()
            {
                FloatAccumulator.Message.AType = typeof(FloatAccumulator);
            }

            public Message(string accumulatorName, float value) : base(accumulatorName, value)
            {
            }

            public static void Send(string accumulatorName, float value, Aggregator aggregator = null)
            {
                AccumulatorMessage<float>.Send(new FloatAccumulator.Message(accumulatorName, value), aggregator);
            }
        }
    }
}