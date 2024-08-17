using Metalogix.Actions.Incremental.Database;

namespace Metalogix.Actions.Incremental
{
    public interface IIncrementalAction
    {
        MappingConnection IncrementalMappings { get; }
    }
}