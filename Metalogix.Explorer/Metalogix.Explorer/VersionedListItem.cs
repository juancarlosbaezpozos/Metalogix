using Metalogix.DataStructures;
using System.ComponentModel;

namespace Metalogix.Explorer
{
    public interface VersionedListItem : ListItem, Node, IComparable, ICustomTypeDescriptor
    {
    }
}