using Metalogix.DataStructures;
using System;
using System.ComponentModel;

namespace Metalogix.Explorer
{
    public interface ListItemVersion : ListItem, Node, Metalogix.DataStructures.IComparable, ICustomTypeDescriptor
    {
        string VersionComments { get; }

        string VersionString { get; }
    }
}