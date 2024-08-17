using Metalogix.DataStructures.Generic;
using Metalogix.Explorer;
using System;
using System.Collections.Generic;

namespace Metalogix.Actions
{
    public class ClipBoard
    {
        private static object s_clipoboard;

        static ClipBoard()
        {
            Settings.ActiveConnectionsChanged +=
                new NodeCollectionChangedHandler(ClipBoard.On_ActiveConnectionsChanged);
        }

        public ClipBoard()
        {
        }

        public static object GetDataObject()
        {
            return ClipBoard.s_clipoboard;
        }

        protected static void On_ActiveConnectionsChanged(NodeCollectionChangeType changeType, Node changedNode)
        {
            if (changeType != NodeCollectionChangeType.NodeRemoved || changedNode == null ||
                changedNode.Connection == null)
            {
                return;
            }

            Connection connection = changedNode.Connection;
            NodeCollection dataObject = ClipBoard.GetDataObject() as NodeCollection;
            if (dataObject != null)
            {
                NodeCollection nodeCollection = new NodeCollection();
                foreach (ExplorerNode explorerNode in dataObject)
                {
                    if (explorerNode != null && explorerNode.Connection.Equals(connection))
                    {
                        continue;
                    }

                    nodeCollection.Add(explorerNode);
                }

                if (nodeCollection.Count == 0)
                {
                    nodeCollection = null;
                }

                ClipBoard.SetDataObject(nodeCollection);
            }
        }

        public static void SetDataObject(object object_0)
        {
            ClipBoard.s_clipoboard = object_0;
        }
    }
}