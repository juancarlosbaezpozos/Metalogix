using System;
using System.Data;

namespace Metalogix.ExternalConnections
{
    public interface IExternalConnectionDatabase : IDisposable
    {
        int AddConnection(Type connectionType, string connectionXml);

        void AddConnectionToNode(int connectionId, string nodeId);

        DataTable GetConnection(int connectionId);

        DataTable GetConnections();

        DataTable GetConnectionsToNode(string nodeId);

        DataTable GetConnectionsToNodes();

        DataTable GetNodes();

        DataTable GetNodesToConnection(int connectionId);

        void RemoveConnection(int id);

        void RemoveConnectionFromNode(int connectionId, string nodeId);

        void RemoveConnectionsFromNode(string nodeId);

        void UpdateConnection(int connectionId, string connectionXml);
    }
}