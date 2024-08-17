using Metalogix.Actions;

namespace Metalogix.Actions.Blocker
{
    public interface ILoggableActionBlocker : IActionBlocker
    {
        LogItem CreateLogItem();
    }
}