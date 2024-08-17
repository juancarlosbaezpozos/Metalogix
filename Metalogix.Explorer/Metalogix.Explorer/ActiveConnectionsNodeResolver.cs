using Metalogix.Explorer.Properties;
using Metalogix.ObjectResolution;
using System;

namespace Metalogix.Explorer
{
    public class ActiveConnectionsNodeResolver : NodeResolverBase
    {
        private static ActiveConnectionsNodeResolver _instance;

        public static ActiveConnectionsNodeResolver Instance
        {
            get
            {
                if (ActiveConnectionsNodeResolver._instance == null)
                {
                    ActiveConnectionsNodeResolver._instance = new ActiveConnectionsNodeResolver();
                }

                return ActiveConnectionsNodeResolver._instance;
            }
        }

        static ActiveConnectionsNodeResolver()
        {
        }

        public ActiveConnectionsNodeResolver()
        {
        }

        public override Node ResolveTypedObject(Location link)
        {
            base.AssertTypeMatch(link);
            Connection byConnString = Settings.ActiveConnections.GetByConnString(link.ConnectionString);
            if (byConnString == null)
            {
                throw new ArgumentException(link.ConnectionString, Resources.ConnectionStringNotPresent);
            }

            if (byConnString.Status == ConnectionStatus.NotChecked)
            {
                byConnString.CheckConnection();
            }

            byConnString.Node.FetchChildren();
            if (string.IsNullOrEmpty(link.Path))
            {
                return byConnString.Node;
            }

            return byConnString.Node.GetNodeByPath(link.Path);
        }
    }
}