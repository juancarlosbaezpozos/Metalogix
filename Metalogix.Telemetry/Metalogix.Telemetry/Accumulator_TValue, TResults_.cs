using System;

namespace Metalogix.Telemetry
{
    public abstract class Accumulator<TValue, TResults> : IAccumulator
    {
        private readonly string _name;

        public abstract bool Modified { get; }

        public string Name
        {
            get { return this._name; }
        }

        protected Accumulator(string name)
        {
            this._name = name;
        }

        public abstract void Accumulate(TValue value);

        public void Accumulate(object value)
        {
            if (!(value is TValue))
            {
                throw new ArgumentException(string.Concat(value.GetType(), " is not assignable to ", typeof(TValue)));
            }

            this.Accumulate((TValue)value);
        }

        public abstract TResults Peek();

        public virtual string PeekAsString()
        {
            return this.Peek().ToString();
        }

        public abstract TResults Read();

        public virtual string ReadAsString()
        {
            return this.Read().ToString();
        }

        public abstract void Reset();
    }
}