using Metalogix.Telemetry;
using System;
using System.Threading;

namespace Metalogix.Telemetry.Accumulators
{
    public class IntAccumulator : Accumulator<int, int>
    {
        private volatile bool _modified;

        private int _currentTotal;

        public override bool Modified
        {
            get { return this._modified; }
        }

        public IntAccumulator(string name) : base(name)
        {
        }

        public override void Accumulate(int value)
        {
            Interlocked.Add(ref this._currentTotal, value);
            this._modified = true;
        }

        public override int Peek()
        {
            return Interlocked.CompareExchange(ref this._currentTotal, 0, 0);
        }

        public override int Read()
        {
            int num = Interlocked.CompareExchange(ref this._currentTotal, 0, this._currentTotal);
            this._modified = false;
            return num;
        }

        public override void Reset()
        {
            Interlocked.Exchange(ref this._currentTotal, 0);
            this._modified = false;
        }

        public class Message : AccumulatorMessage<int>
        {
            private readonly static Type AType;

            public override Type AccumlatorType
            {
                get { return IntAccumulator.Message.AType; }
            }

            static Message()
            {
                IntAccumulator.Message.AType = typeof(IntAccumulator);
            }

            public Message(string accumulatorName, int value) : base(accumulatorName, value)
            {
            }

            public static void Send(string accumulatorName, int value, Aggregator aggregator = null)
            {
                AccumulatorMessage<int>.Send(new IntAccumulator.Message(accumulatorName, value), aggregator);
            }
        }
    }
}