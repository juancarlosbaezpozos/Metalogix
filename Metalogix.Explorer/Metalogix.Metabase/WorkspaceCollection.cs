using System;
using System.Collections.Generic;
using System.Reflection;

namespace Metalogix.Metabase
{
    public class WorkspaceCollection : List<Workspace>
    {
        public Type CollectionType
        {
            get { return typeof(Workspace); }
        }

        public new object this[int index]
        {
            get { return base[index]; }
        }

        public WorkspaceCollection()
        {
        }
    }
}