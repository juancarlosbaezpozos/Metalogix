using Metalogix.Telemetry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Metalogix.Telemetry.Accumulators
{
    public class StringAccumulator : Accumulator<string, IEnumerable<string>>
    {
        private volatile bool _modified;

        private readonly object _synRoot = new object();

        private readonly List<string> _data = new List<string>();

        public override bool Modified
        {
            get { return this._modified; }
        }

        public StringAccumulator(string name) : base(name)
        {
        }

        public override void Accumulate(string value)
        {
            lock (this._synRoot)
            {
                this._data.Add(value);
                this._modified = true;
            }
        }

        public override IEnumerable<string> Peek()
        {
            IEnumerable<string> strs;
            if (this._data.Count == 0)
            {
                return null;
            }

            lock (this._synRoot)
            {
                if (this._data.Count != 0)
                {
                    return this._data.ToList<string>();
                }
                else
                {
                    strs = null;
                }
            }

            return strs;
        }

        public override string PeekAsString()
        {
            string str;
            if (this._data.Count == 0)
            {
                return string.Empty;
            }

            lock (this._synRoot)
            {
                str = (this._data.Count != 0
                    ? this._data.Aggregate<string>((string accumulator, string next) =>
                        string.Concat(accumulator, "|", next))
                    : string.Empty);
            }

            return str;
        }

        public override IEnumerable<string> Read()
        {
            IEnumerable<string> strs;
            lock (this._synRoot)
            {
                if (this._data.Count != 0)
                {
                    List<string> list = this._data.ToList<string>();
                    this._data.Clear();
                    this._modified = false;
                    return list;
                }
                else
                {
                    strs = null;
                }
            }

            return strs;
        }

        public override string ReadAsString()
        {
            string empty;
            if (this._data.Count == 0)
            {
                return string.Empty;
            }

            lock (this._synRoot)
            {
                if (this._data.Count != 0)
                {
                    string str = this._data.Aggregate<string>((string accumulator, string next) =>
                        string.Concat(accumulator, "|", next));
                    this._data.Clear();
                    this._modified = false;
                    empty = str;
                }
                else
                {
                    empty = string.Empty;
                }
            }

            return empty;
        }

        public override void Reset()
        {
            lock (this._synRoot)
            {
                this._data.Clear();
                this._modified = false;
            }
        }

        public class Message : AccumulatorMessage<string>
        {
            private readonly static Type AType;

            public override Type AccumlatorType
            {
                get { return StringAccumulator.Message.AType; }
            }

            static Message()
            {
                StringAccumulator.Message.AType = typeof(StringAccumulator);
            }

            public Message(string accumulatorName, string value) : base(accumulatorName, value)
            {
            }

            public static void Send(string accumulatorName, string keyName, string keyValue,
                bool sendEmptyMessage = false, Aggregator aggregator = null)
            {
                StringAccumulator.Message.Send(accumulatorName, string.Format("{0} : {1}", keyName, keyValue),
                    sendEmptyMessage, aggregator);
            }

            public static void Send(string accumulatorName, string value, bool sendEmptyMessage = false,
                Aggregator aggregator = null)
            {
                if (!sendEmptyMessage && value.Equals(string.Empty))
                {
                    return;
                }

                AccumulatorMessage<string>.Send(new StringAccumulator.Message(accumulatorName, value), aggregator);
            }

            public static void TransformAndSend(string accumulatorName, string value, Func<string, string> transform,
                bool sendEmptyMessage = false, Aggregator aggregator = null)
            {
                try
                {
                    if (Client.OptIn)
                    {
                        string str = transform(value);
                        if (sendEmptyMessage || !str.Equals(string.Empty))
                        {
                            AccumulatorMessage<string>.Send(new StringAccumulator.Message(accumulatorName, str),
                                aggregator);
                        }
                    }
                }
                catch
                {
                }
            }
        }
    }
}