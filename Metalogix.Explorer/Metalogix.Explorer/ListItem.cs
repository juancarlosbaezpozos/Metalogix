using Metalogix.DataStructures;
using System.ComponentModel;

namespace Metalogix.Explorer
{
    public interface ListItem : Node, IComparable, ICustomTypeDescriptor
    {
        ListItemVersionCollection VersionHistory { get; }
    }
}