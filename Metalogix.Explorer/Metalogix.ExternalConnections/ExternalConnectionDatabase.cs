using Metalogix.Explorer;
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.IO;
using System.Reflection;

namespace Metalogix.ExternalConnections
{
    public class ExternalConnectionDatabase : IExternalConnectionDatabase, IDisposable
    {
        private readonly SqlCeConnection m_connection;

        public ExternalConnectionDatabase(string databaseFile)
        {
            string str = string.Format("DataSource=\"{0}\"", databaseFile);
            bool flag = false;
            if (!File.Exists(databaseFile))
            {
                (new SqlCeEngine(str)).CreateDatabase();
                flag = true;
            }

            this.m_connection = SqlCeUtilities.GetConnection(str, true);
            if (flag)
            {
                using (SqlCeCommand sqlCeCommand = this.m_connection.CreateCommand())
                {
                    sqlCeCommand.CommandText =
                        "CREATE TABLE Connections(connectionId int PRIMARY KEY IDENTITY, connectionType nvarchar(512), connectionAssembly nvarchar(512), connectionXml ntext)";
                    sqlCeCommand.ExecuteNonQuery();
                    sqlCeCommand.CommandText =
                        "CREATE TABLE ConnectionsToNodes(connectionId int, nodeId nvarchar(2048), PRIMARY KEY(nodeId, connectionId))";
                    sqlCeCommand.ExecuteNonQuery();
                    sqlCeCommand.CommandText =
                        "CREATE INDEX ConnectionsToNodes_connectionId ON ConnectionsToNodes(connectionId)";
                    sqlCeCommand.ExecuteNonQuery();
                    sqlCeCommand.CommandText = "CREATE INDEX ConnectionsToNodes_nodeId ON ConnectionsToNodes(nodeId)";
                    sqlCeCommand.ExecuteNonQuery();
                }
            }
        }

        public int AddConnection(Type connectionType, string connectionXml)
        {
            int num;
            using (SqlCeCommand sqlCeCommand = this.m_connection.CreateCommand())
            {
                sqlCeCommand.CommandText =
                    "INSERT INTO Connections(connectionType, connectionAssembly, connectionXml) VALUES(@type, @assembly, @xml)";
                sqlCeCommand.Parameters.AddWithValue("@type", connectionType.FullName);
                sqlCeCommand.Parameters.AddWithValue("@assembly", connectionType.Assembly.GetName().Name);
                sqlCeCommand.Parameters.AddWithValue("@xml", connectionXml);
                sqlCeCommand.ExecuteNonQuery();
                sqlCeCommand.CommandText = "SELECT @@IDENTITY";
                num = Convert.ToInt32(sqlCeCommand.ExecuteScalar());
            }

            return num;
        }

        public void AddConnectionToNode(int connectionId, string nodeId)
        {
            using (SqlCeCommand sqlCeCommand = this.m_connection.CreateCommand())
            {
                sqlCeCommand.CommandText =
                    "INSERT INTO ConnectionsToNodes(connectionId, nodeId) VALUES(@connId, @nodeId)";
                sqlCeCommand.Parameters.AddWithValue("@connId", connectionId);
                sqlCeCommand.Parameters.AddWithValue("@nodeId", nodeId);
                sqlCeCommand.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            this.m_connection.Dispose();
        }

        public DataTable GetConnection(int connectionId)
        {
            DataTable dataTable;
            using (SqlCeCommand sqlCeCommand = this.m_connection.CreateCommand())
            {
                sqlCeCommand.CommandText = "SELECT * FROM Connections WHERE connectionId=@id";
                sqlCeCommand.Parameters.AddWithValue("@id", connectionId);
                using (SqlCeDataAdapter sqlCeDataAdapter = new SqlCeDataAdapter(sqlCeCommand))
                {
                    DataTable dataTable1 = new DataTable();
                    sqlCeDataAdapter.Fill(dataTable1);
                    dataTable = dataTable1;
                }
            }

            return dataTable;
        }

        public DataTable GetConnections()
        {
            DataTable dataTable;
            using (SqlCeCommand sqlCeCommand = this.m_connection.CreateCommand())
            {
                sqlCeCommand.CommandText = "SELECT * FROM Connections";
                using (SqlCeDataAdapter sqlCeDataAdapter = new SqlCeDataAdapter(sqlCeCommand))
                {
                    DataTable dataTable1 = new DataTable();
                    sqlCeDataAdapter.Fill(dataTable1);
                    dataTable = dataTable1;
                }
            }

            return dataTable;
        }

        public DataTable GetConnectionsToNode(string nodeId)
        {
            DataTable dataTable;
            using (SqlCeCommand sqlCeCommand = this.m_connection.CreateCommand())
            {
                sqlCeCommand.CommandText = "SELECT * FROM ConnectionsToNodes WHERE nodeId=@nodeId";
                sqlCeCommand.Parameters.AddWithValue("@nodeId", nodeId);
                using (SqlCeDataAdapter sqlCeDataAdapter = new SqlCeDataAdapter(sqlCeCommand))
                {
                    DataTable dataTable1 = new DataTable();
                    sqlCeDataAdapter.Fill(dataTable1);
                    dataTable = dataTable1;
                }
            }

            return dataTable;
        }

        public DataTable GetConnectionsToNodes()
        {
            DataTable dataTable;
            using (SqlCeCommand sqlCeCommand = this.m_connection.CreateCommand())
            {
                sqlCeCommand.CommandText = "SELECT * FROM ConnectionsToNodes";
                using (SqlCeDataAdapter sqlCeDataAdapter = new SqlCeDataAdapter(sqlCeCommand))
                {
                    DataTable dataTable1 = new DataTable();
                    sqlCeDataAdapter.Fill(dataTable1);
                    dataTable = dataTable1;
                }
            }

            return dataTable;
        }

        public DataTable GetNodes()
        {
            DataTable dataTable;
            using (SqlCeCommand sqlCeCommand = this.m_connection.CreateCommand())
            {
                sqlCeCommand.CommandText = "SELECT nodeId FROM ConnectionsToNodes";
                using (SqlCeDataAdapter sqlCeDataAdapter = new SqlCeDataAdapter(sqlCeCommand))
                {
                    DataTable dataTable1 = new DataTable();
                    sqlCeDataAdapter.Fill(dataTable1);
                    dataTable = dataTable1;
                }
            }

            return dataTable;
        }

        public DataTable GetNodesToConnection(int connectionId)
        {
            DataTable dataTable;
            using (SqlCeCommand sqlCeCommand = this.m_connection.CreateCommand())
            {
                sqlCeCommand.CommandText = "SELECT nodeId FROM ConnectionsToNodes WHERE connectionId=@id";
                sqlCeCommand.Parameters.AddWithValue("@id", connectionId);
                using (SqlCeDataAdapter sqlCeDataAdapter = new SqlCeDataAdapter(sqlCeCommand))
                {
                    DataTable dataTable1 = new DataTable();
                    sqlCeDataAdapter.Fill(dataTable1);
                    dataTable = dataTable1;
                }
            }

            return dataTable;
        }

        public void RemoveConnection(int id)
        {
            using (SqlCeCommand sqlCeCommand = this.m_connection.CreateCommand())
            {
                sqlCeCommand.CommandText = "DELETE FROM Connections WHERE connectionId=@id";
                sqlCeCommand.Parameters.AddWithValue("@id", id);
                sqlCeCommand.ExecuteNonQuery();
            }
        }

        public void RemoveConnectionFromNode(int connectionId, string nodeId)
        {
            using (SqlCeCommand sqlCeCommand = this.m_connection.CreateCommand())
            {
                sqlCeCommand.CommandText =
                    "DELETE FROM ConnectionsToNodes WHERE connectionId=@connId AND nodeId=@nodeId";
                sqlCeCommand.Parameters.AddWithValue("@connId", connectionId);
                sqlCeCommand.Parameters.AddWithValue("@nodeId", nodeId);
                sqlCeCommand.ExecuteNonQuery();
            }
        }

        public void RemoveConnectionsFromNode(string nodeId)
        {
            using (SqlCeCommand sqlCeCommand = this.m_connection.CreateCommand())
            {
                sqlCeCommand.CommandText = "DELETE FROM ConnectionsToNodes WHERE nodeId=@nodeId";
                sqlCeCommand.Parameters.AddWithValue("@nodeId", nodeId);
                sqlCeCommand.ExecuteNonQuery();
            }
        }

        public void UpdateConnection(int connectionId, string connectionXml)
        {
            using (SqlCeCommand sqlCeCommand = this.m_connection.CreateCommand())
            {
                sqlCeCommand.CommandText = "UPDATE Connections SET connectionXml=@xml WHERE connectionId=@id";
                sqlCeCommand.Parameters.AddWithValue("@id", connectionId);
                sqlCeCommand.Parameters.AddWithValue("@xml", connectionXml);
                sqlCeCommand.ExecuteNonQuery();
            }
        }
    }
}