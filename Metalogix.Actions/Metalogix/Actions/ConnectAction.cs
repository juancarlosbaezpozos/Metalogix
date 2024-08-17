using System;

namespace Metalogix.Actions
{
    [RunAsync(false)]
    [SourceCardinality(Cardinality.Zero)]
    [TargetCardinality(Cardinality.Zero)]
    public abstract class ConnectAction : Metalogix.Actions.Action, IConnectAction
    {
        protected ConnectAction()
        {
        }
    }
}