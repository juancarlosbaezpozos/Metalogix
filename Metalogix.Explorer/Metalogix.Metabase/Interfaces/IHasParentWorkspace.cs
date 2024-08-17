using Metalogix.Metabase;

namespace Metalogix.Metabase.Interfaces
{
    public interface IHasParentWorkspace
    {
        Workspace ParentWorkspace { get; }
    }
}