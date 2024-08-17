namespace Metalogix.Actions
{
    internal interface IActionConfigProvider
    {
        IActionConfig GetActionConfig(Action action);
    }
}