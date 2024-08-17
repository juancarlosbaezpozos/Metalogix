using System;

namespace Metalogix.Actions
{
    [RunAsync(false)]
    [SourceCardinality(Cardinality.Zero)]
    [TargetCardinality(Cardinality.Zero)]
    public abstract class ConnectAction<T> : Metalogix.Actions.Action<T>, IConnectAction
    {
        protected ConnectAction()
        {
        }
    }
}