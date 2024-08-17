using Metalogix.Metabase.Interfaces;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Metalogix.Metabase
{
    public sealed class MetabaseConnection : IDisposable
    {
        private readonly IMetabaseAdapter m_adapter;

        private WorkspaceList m_workspaces;

        [Browsable(false)]
        public IMetabaseAdapter Adapter
        {
            get { return this.m_adapter; }
        }

        public string ConnectionError { get; private set; }

        public bool IsConnected { get; private set; }

        public string MetabaseContext { get; private set; }

        public string MetabaseType { get; private set; }

        public WorkspaceList Workspaces
        {
            get
            {
                if (this.m_workspaces == null)
                {
                    this.m_workspaces = new WorkspaceList(this, this.Adapter.FetchWorkspaceList());
                }

                return this.m_workspaces;
            }
        }

        internal MetabaseConnection(IMetabaseAdapter adapter, string metabaseType, string metabaseContext,
            string connectionError)
        {
            this.m_adapter = adapter;
            this.MetabaseType = metabaseType;
            this.MetabaseContext = metabaseContext;
            this.ConnectionError = connectionError;
            this.Connect();
        }

        public void Connect()
        {
            this.IsConnected = false;
            if (this.m_adapter != null)
            {
                this.m_adapter.Open();
                this.IsConnected = true;
                this.ConnectionError = string.Empty;
            }
        }

        public Workspace CreateWorkspace(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            Workspace workspace = this.Workspaces.AddNew();
            workspace.Name = name;
            workspace.CommitChanges();
            return workspace;
        }

        public void Disconnect()
        {
            if (this.m_adapter != null)
            {
                this.m_adapter.Close();
            }

            this.IsConnected = false;
        }

        public void Dispose()
        {
            this.Disconnect();
            if (this.m_adapter != null)
            {
                this.m_adapter.Dispose();
            }
        }

        public Workspace GetWorkspace(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            return this.Workspaces[name];
        }
    }
}