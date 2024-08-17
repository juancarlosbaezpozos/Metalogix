using System;
using System.Runtime.CompilerServices;

namespace Metalogix.Actions.Blocker
{
    public class ActionBlockerEventArgs : EventArgs
    {
        public IActionBlocker Blocker { get; private set; }

        public ActionBlockerChangeType ChangeType { get; private set; }

        public string Message { get; private set; }

        public ActionBlockerEventArgs(ActionBlockerChangeType changeType, IActionBlocker blocker, string message)
        {
            this.ChangeType = changeType;
            this.Blocker = blocker;
            this.Message = message;
        }
    }
}