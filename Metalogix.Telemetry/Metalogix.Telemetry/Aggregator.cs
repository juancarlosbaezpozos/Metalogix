using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Metalogix.Telemetry
{
    public class Aggregator
    {
        private static volatile bool _defaultEnabled;

        private static volatile Aggregator _default;

        private readonly static object StaticSyncroot;

        private readonly Thread _aggregatorThread;

        private readonly List<IAccumulator> _accumlators;

        private readonly object _instanceSyncRoot;

        private volatile bool _stopAccumulating;

        public static Aggregator Default
        {
            get
            {
                Aggregator aggregator;
                if (!Aggregator._defaultEnabled)
                {
                    return null;
                }

                if (Aggregator._default != null)
                {
                    return Aggregator._default;
                }

                lock (Aggregator.StaticSyncroot)
                {
                    if (Aggregator._defaultEnabled)
                    {
                        object obj = Aggregator._default;
                        if (obj == null)
                        {
                            obj = new Aggregator(true);
                            Aggregator._default = (Aggregator)obj;
                        }

                        aggregator = (Aggregator)obj;
                    }
                    else
                    {
                        aggregator = null;
                    }
                }

                return aggregator;
            }
        }

        public static bool DefaultEnabled
        {
            get { return Aggregator._defaultEnabled; }
            set
            {
                Aggregator._defaultEnabled = value;
                if (Aggregator._default == null)
                {
                    return;
                }

                lock (Aggregator.StaticSyncroot)
                {
                    if (Aggregator._default != null)
                    {
                        Aggregator._default.Stop();
                        Aggregator._default = null;
                    }
                }
            }
        }

        public bool IsStopped
        {
            get { return this._stopAccumulating; }
        }

        public Metalogix.Telemetry.Queue<IAccumulatorMessage> Queue { get; private set; }

        static Aggregator()
        {
            Aggregator.StaticSyncroot = new object();
        }

        public Aggregator(bool autoStart = false)
        {
            this._accumlators = new List<IAccumulator>();
            this._instanceSyncRoot = new object();
            this._stopAccumulating = true;

            this.Queue = new Metalogix.Telemetry.Queue<IAccumulatorMessage>();
            this._aggregatorThread = new Thread(() =>
            {
                while (!this._stopAccumulating)
                {
                    this.ProcessMessage();
                }
            });
            if (autoStart)
            {
                this.Start();
            }
        }

        public IDictionary<string, string> Peek(Func<IAccumulator, bool> filter = null)
        {
            return this.Read(filter, false);
        }

        private void ProcessMessage()
        {
            IAccumulatorMessage accumulatorMessage = this.Queue.Dequeue();
            if (accumulatorMessage == null || string.IsNullOrEmpty(accumulatorMessage.AccumulatorName))
            {
                return;
            }

            lock (this._instanceSyncRoot)
            {
                IAccumulator accumulator = this._accumlators.Find((IAccumulator accum) =>
                    accum.Name == accumulatorMessage.AccumulatorName);
                if (accumulator == null)
                {
                    Type accumlatorType = accumulatorMessage.AccumlatorType;
                    object[] accumulatorName = new object[] { accumulatorMessage.AccumulatorName };
                    accumulator = (IAccumulator)Activator.CreateInstance(accumlatorType, accumulatorName);
                    this._accumlators.Add(accumulator);
                }
                else if (accumulator.GetType() != accumulatorMessage.AccumlatorType)
                {
                    throw new AccumulatorConflictException(accumulator.Name);
                }

                accumulator.Accumulate(accumulatorMessage.Value());
            }
        }

        public IDictionary<string, string> Read(Func<IAccumulator, bool> filter = null)
        {
            return this.Read(filter, true);
        }

        private Dictionary<string, string> Read(Func<IAccumulator, bool> filter, bool reset)
        {
            Dictionary<string, string> strs;
            lock (this._instanceSyncRoot)
            {
                strs = (filter == null
                    ? (
                        from iaccumulator_0 in this._accumlators
                        where iaccumulator_0.Modified
                        select iaccumulator_0).ToDictionary<IAccumulator, string, string>(
                        (IAccumulator iaccumulator_0) => iaccumulator_0.Name, (IAccumulator iaccumulator_0) =>
                        {
                            if (!reset)
                            {
                                return iaccumulator_0.PeekAsString();
                            }

                            return iaccumulator_0.ReadAsString();
                        })
                    : (
                        from iaccumulator_0 in this._accumlators
                        where iaccumulator_0.Modified
                        select iaccumulator_0).Where<IAccumulator>(filter).ToDictionary<IAccumulator, string, string>(
                        (IAccumulator iaccumulator_0) => iaccumulator_0.Name, (IAccumulator iaccumulator_0) =>
                        {
                            if (!reset)
                            {
                                return iaccumulator_0.PeekAsString();
                            }

                            return iaccumulator_0.ReadAsString();
                        }));
            }

            return strs;
        }

        public void Start()
        {
            if (!this._stopAccumulating)
            {
                return;
            }

            lock (this._instanceSyncRoot)
            {
                if (this._stopAccumulating)
                {
                    this._stopAccumulating = false;
                    if (!this._aggregatorThread.IsAlive)
                    {
                        this._aggregatorThread.Start();
                    }

                    if (this.Queue.IsStopped)
                    {
                        this.Queue.Start();
                    }
                }
            }
        }

        public void Stop()
        {
            if (this._stopAccumulating)
            {
                return;
            }

            lock (this._instanceSyncRoot)
            {
                if (!this.Queue.IsStopped)
                {
                    this.Queue.Stop();
                }

                if (!this._stopAccumulating)
                {
                    this._stopAccumulating = true;
                }
            }
        }
    }
}