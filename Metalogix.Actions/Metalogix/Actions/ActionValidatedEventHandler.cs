using System;

namespace Metalogix.Actions
{
    public delegate void ActionValidatedEventHandler(Metalogix.Actions.Action sender, IXMLAbleList source,
        IXMLAbleList target);
}