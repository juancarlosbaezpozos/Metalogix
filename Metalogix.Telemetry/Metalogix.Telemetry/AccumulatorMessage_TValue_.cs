using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Telemetry
{
    public abstract class AccumulatorMessage<TValue> : IAccumulatorMessage
    {
        private TValue _value;

        public abstract Type AccumlatorType { get; }

        public string AccumulatorName
        {
            get { return JustDecompileGenerated_get_AccumulatorName(); }
            set { JustDecompileGenerated_set_AccumulatorName(value); }
        }

        private string JustDecompileGenerated_AccumulatorName_k__BackingField;

        public string JustDecompileGenerated_get_AccumulatorName()
        {
            return this.JustDecompileGenerated_AccumulatorName_k__BackingField;
        }

        private void JustDecompileGenerated_set_AccumulatorName(string value)
        {
            this.JustDecompileGenerated_AccumulatorName_k__BackingField = value;
        }

        public TValue Value
        {
            get { return this._value; }
            set
            {
                if (!typeof(TValue).IsValueType)
                {
                    throw new Exception("Message value is a reference type and cannot be modified");
                }

                this._value = value;
            }
        }

        protected AccumulatorMessage(string accumulatorName, TValue value)
        {
            this.AccumulatorName = accumulatorName;
            this._value = value;
        }

        public virtual object Clone()
        {
            return this.MemberwiseClone();
        }

        object Metalogix.Telemetry.IAccumulatorMessage.Value()
        {
            return this._value;
        }

        public void Send(Aggregator aggregator = null)
        {
            AccumulatorMessage<TValue>.Send(this, aggregator);
        }

        public static void Send(IAccumulatorMessage message, Aggregator aggregator = null)
        {
            try
            {
                if (Client.OptIn)
                {
                    Aggregator aggregator1 = aggregator ?? Aggregator.Default;
                    aggregator = aggregator1;
                    if (aggregator1 != null)
                    {
                        IAccumulatorMessage accumulatorMessage = message.Clone() as IAccumulatorMessage;
                        aggregator.Queue.Enqueue(accumulatorMessage);
                        return;
                    }
                }
            }
            catch
            {
            }
        }
    }
}