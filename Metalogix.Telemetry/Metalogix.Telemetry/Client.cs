using PreEmptive.SoS.Client.Messages;
using PreEmptive.SoS.Metalogix.Telemetry;
using PreEmptive.SoS.Runtime;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Metalogix.Telemetry
{
    public static class Client
    {
        private static bool _alwaysOptIn;

        private static volatile Client.State _currentState;

        private readonly static object StaticSyncRoot;

        public static string Edition { get; private set; }

        public static string InstanceId { get; private set; }

        public static bool OptIn
        {
            get
            {
                if (!Client._alwaysOptIn)
                {
                    return TelemetryConfigurationVariables.TelemetryOptIn;
                }

                return true;
            }
            set { TelemetryConfigurationVariables.TelemetryOptIn = value; }
        }

        static Client()
        {
            Client.StaticSyncRoot = new object();
        }

        public static void Initialize(string edition, string instanceId, bool autoSetup = false,
            bool autoTearDown = false, bool alwaysOptIn = false)
        {
            lock (Client.StaticSyncRoot)
            {
                if (Client._currentState != Client.State.Uninitialized)
                {
                    throw new InvalidTelemetryStateException("Telemetry is already initialized.");
                }

                Client._alwaysOptIn = alwaysOptIn;
                Client.Edition = edition;
                Client.InstanceId = instanceId;
                if (autoTearDown)
                {
                    if (!AppDomain.CurrentDomain.IsDefaultAppDomain())
                    {
                        AppDomain.CurrentDomain.DomainUnload += new EventHandler(Client.TearDown);
                    }
                    else
                    {
                        AppDomain.CurrentDomain.ProcessExit += new EventHandler(Client.TearDown);
                    }
                }

                Client._currentState = Client.State.Initialized;
            }

            if (autoSetup)
            {
                Client.Setup();
            }
        }

        public static void ProfileSystem()
        {
            lock (Client.StaticSyncRoot)
            {
                if (Client._currentState != Client.State.SetUpAndStarted)
                {
                    throw new InvalidTelemetryStateException("Telemetry in not in a valid state.");
                }

                Client.ProfileSystemImpl();
            }
        }

        private static void ProfileSystemImpl()
        {
        }

        public static void Setup()
        {
            lock (Client.StaticSyncRoot)
            {
                switch (Client._currentState)
                {
                    case (Client.State)Client.State.Uninitialized:
                    {
                        throw new InvalidTelemetryStateException("Cannot call Setup if Telemetry is not Initialized.");
                    }
                    case (Client.State)Client.State.Initialized:
                    {
                        Client.SetupImpl(new Dictionary<string, string>()
                        {
                            { "Edition", Client.Edition }
                        });
                        Client._currentState = Client.State.SetUpAndStarted;
                        Aggregator.DefaultEnabled = true;
                        break;
                    }
                    case (Client.State)Client.State.SetUpAndStarted:
                    {
                        return;
                    }
                    case (Client.State)Client.State.TornDown:
                    {
                        throw new InvalidTelemetryStateException("Cannot call Setup after TearDown.");
                    }
                }
            }
        }

        private static void SetupImpl(IDictionary<string, string> customDataDictionary)
        {
        }

        private static void TearDown(object sender, EventArgs e)
        {
            Client.TearDown();
        }

        public static void TearDown()
        {
            lock (Client.StaticSyncRoot)
            {
                switch (Client._currentState)
                {
                    case (Client.State)Client.State.Uninitialized:
                    {
                        throw new InvalidTelemetryStateException(
                            "Cannot call TearDown if Telemetry is not Initialized.");
                    }
                    case (Client.State)Client.State.Initialized:
                    {
                        throw new InvalidTelemetryStateException("Cannot call TearDown if Setup has not been called.");
                    }
                    case (Client.State)Client.State.SetUpAndStarted:
                    {
                        Aggregator @default = Aggregator.Default;
                        if (@default == null)
                        {
                            Client.TearDownImpl(null);
                        }
                        else
                        {
                            @default.Stop();
                            Client.UpdateImpl(@default.Read(null));
                        }

                        Client._currentState = Client.State.TornDown;
                        break;
                    }
                }
            }
        }

        private static void TearDownImpl(IDictionary<string, string> customDataDictionary)
        {
        }

        public static void Update(Aggregator aggregator = null, Func<IAccumulator, bool> filter = null,
            bool resetAfterUpdate = true)
        {
            lock (Client.StaticSyncRoot)
            {
                Aggregator aggregator1 = aggregator ?? Aggregator.Default;
                aggregator = aggregator1;
                if (aggregator1 != null)
                {
                    if (Client._currentState != Client.State.SetUpAndStarted)
                    {
                        throw new InvalidTelemetryStateException("Telemetry in not in a valid state.");
                    }

                    IDictionary<string, string> strs =
                        (resetAfterUpdate ? aggregator.Read(filter) : aggregator.Peek(filter));
                    if (strs == null)
                    {
                        strs = new Dictionary<string, string>()
                        {
                            { "License Update", "License Update" }
                        };
                    }
                    else if (strs.Count == 0)
                    {
                        strs.Add("License Update", "License Update");
                    }

                    Client.UpdateImpl(strs);
                }
            }
        }

        public static void Update(IDictionary<string, string> customDataDictionary)
        {
            lock (Client.StaticSyncRoot)
            {
                if (Client._currentState != Client.State.SetUpAndStarted)
                {
                    throw new InvalidTelemetryStateException("Telemetry in not in a valid state.");
                }

                Client.UpdateImpl(customDataDictionary);
            }
        }

        private static void UpdateImpl(IDictionary<string, string> customDataDictionary)
        {
            FeatureMessage featureMessage = new FeatureMessage("TelemetryUpdate")
            {
                Event = new EventInformation()
                {
                    Code = "Feature.Tick"
                },
                Binary = BinaryInfo.Get()
            };
            featureMessage.AddExtendedKeys(customDataDictionary);
            Access.Send(featureMessage);
        }

        private enum State
        {
            Uninitialized,
            Initialized,
            SetUpAndStarted,
            TornDown
        }
    }
}