using System;

namespace Metalogix.Actions
{
    public delegate void ActionStartedEventHandler(Metalogix.Actions.Action sender, string sSourceString,
        string sTargetString);
}