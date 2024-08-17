namespace Metalogix.Actions
{
    public interface IActionConfig
    {
        ConfigurationResult Configure(ActionConfigContext context);
    }
}