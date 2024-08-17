using Metalogix.Telemetry;
using System;
using System.Threading;

namespace Metalogix.Telemetry.Accumulators
{
    public class LongAccumulator : Accumulator<long, long>
    {
        private volatile bool _modified;

        private long _currentTotal;

        public override bool Modified
        {
            get { return this._modified; }
        }

        public LongAccumulator(string name) : base(name)
        {
        }

        public override void Accumulate(long value)
        {
            Interlocked.Add(ref this._currentTotal, value);
            this._modified = true;
        }

        public override long Peek()
        {
            return Interlocked.Read(ref this._currentTotal);
        }

        public override long Read()
        {
            long num = Interlocked.CompareExchange(ref this._currentTotal, 0L, this._currentTotal);
            this._modified = false;
            return num;
        }

        public override void Reset()
        {
            Interlocked.Exchange(ref this._currentTotal, 0L);
            this._modified = false;
        }

        public class Message : AccumulatorMessage<long>
        {
            private readonly static Type AType;

            public override Type AccumlatorType
            {
                get { return LongAccumulator.Message.AType; }
            }

            static Message()
            {
                LongAccumulator.Message.AType = typeof(LongAccumulator);
            }

            public Message(string accumulatorName, long value) : base(accumulatorName, value)
            {
            }

            public static void Send(string accumulatorName, long value, Aggregator aggregator = null)
            {
                AccumulatorMessage<long>.Send(new LongAccumulator.Message(accumulatorName, value), aggregator);
            }
        }
    }
}