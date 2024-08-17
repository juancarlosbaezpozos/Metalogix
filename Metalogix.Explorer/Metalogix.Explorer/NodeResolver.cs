using Metalogix.ObjectResolution;
using System;

namespace Metalogix.Explorer
{
    [IsDefault(true)]
    public class NodeResolver : NodeResolverBase
    {
        private static NodeResolver _instance;

        public static NodeResolver Instance
        {
            get
            {
                if (NodeResolver._instance == null)
                {
                    NodeResolver._instance = new NodeResolver();
                }

                return NodeResolver._instance;
            }
        }

        static NodeResolver()
        {
        }

        public NodeResolver()
        {
        }

        public override Node ResolveTypedObject(Location link)
        {
            Connection connection = ConnectionFactory.CreateConnection(link.ConnectionString);
            if (connection.Status == ConnectionStatus.NotChecked)
            {
                connection.CheckConnection();
            }

            connection.Node.FetchChildren();
            if (string.IsNullOrEmpty(link.Path))
            {
                return connection.Node;
            }

            return connection.Node.GetNodeByPath(link.Path);
        }
    }
}