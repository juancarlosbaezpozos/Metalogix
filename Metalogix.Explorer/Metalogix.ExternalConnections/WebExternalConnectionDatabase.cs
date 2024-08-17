using System;
using System.ComponentModel;
using System.Data;

namespace Metalogix.ExternalConnections
{
    public class WebExternalConnectionDatabase : IExternalConnectionDatabase, IDisposable
    {
        private DataTable _stubTable = new DataTable();

        public WebExternalConnectionDatabase()
        {
        }

        public int AddConnection(Type connectionType, string connectionXml)
        {
            throw new NotImplementedException();
        }

        public void AddConnectionToNode(int connectionId, string nodeId)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            this._stubTable.Dispose();
        }

        public DataTable GetConnection(int connectionId)
        {
            return this._stubTable;
        }

        public DataTable GetConnections()
        {
            return this._stubTable;
        }

        public DataTable GetConnectionsToNode(string nodeId)
        {
            return this._stubTable;
        }

        public DataTable GetConnectionsToNodes()
        {
            return this._stubTable;
        }

        public DataTable GetNodes()
        {
            return this._stubTable;
        }

        public DataTable GetNodesToConnection(int connectionId)
        {
            return this._stubTable;
        }

        public void RemoveConnection(int id)
        {
            throw new NotImplementedException();
        }

        public void RemoveConnectionFromNode(int connectionId, string nodeId)
        {
            throw new NotImplementedException();
        }

        public void RemoveConnectionsFromNode(string nodeId)
        {
            throw new NotImplementedException();
        }

        public void UpdateConnection(int connectionId, string connectionXml)
        {
            throw new NotImplementedException();
        }
    }
}