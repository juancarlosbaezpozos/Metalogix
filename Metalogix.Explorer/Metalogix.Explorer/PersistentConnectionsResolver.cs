using System;

namespace Metalogix.Explorer
{
    public class PersistentConnectionsResolver : NodeResolverBase
    {
        private static PersistentConnectionsResolver _instance;

        public static PersistentConnectionsResolver Instance
        {
            get
            {
                if (PersistentConnectionsResolver._instance == null)
                {
                    PersistentConnectionsResolver._instance = new PersistentConnectionsResolver();
                }

                return PersistentConnectionsResolver._instance;
            }
        }

        static PersistentConnectionsResolver()
        {
        }

        public PersistentConnectionsResolver()
        {
        }

        public override Node ResolveTypedObject(Location link)
        {
            Node node = null;
            bool flag = false;
            Connection byConnString = Settings.ActiveConnections.GetByConnString(link.ConnectionString);
            if (byConnString == null)
            {
                byConnString = ConnectionFactory.CreateConnection(link.ConnectionString);
                flag = true;
            }

            Exception exception = null;
            try
            {
                if (byConnString.Status == ConnectionStatus.NotChecked)
                {
                    byConnString.CheckConnection();
                }
            }
            catch (Exception exception1)
            {
                exception = exception1;
            }

            Connection connection = Settings.ActiveConnections.GetByConnString(byConnString.ConnectionString);
            if (connection != null)
            {
                try
                {
                    if (connection.Status == ConnectionStatus.NotChecked)
                    {
                        connection.CheckConnection();
                    }

                    flag = false;
                    byConnString = connection;
                }
                catch
                {
                }
            }

            if (!flag || exception == null)
            {
                byConnString.Node.FetchChildren();
                node = (link.Path == "" ? byConnString.Node : byConnString.Node.GetNodeByPath(link.Path));
            }

            if (flag)
            {
                if (exception != null)
                {
                    throw exception;
                }

                Settings.ActiveConnections.Add(byConnString);
            }

            return node;
        }
    }
}