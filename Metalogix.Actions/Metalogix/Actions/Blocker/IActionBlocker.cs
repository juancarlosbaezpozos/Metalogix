using System;

namespace Metalogix.Actions.Blocker
{
    public interface IActionBlocker
    {
        string BlockedReason { get; }

        void Block();

        void BlockUntil(Func<bool> condition);

        bool ShouldBlock();
    }
}