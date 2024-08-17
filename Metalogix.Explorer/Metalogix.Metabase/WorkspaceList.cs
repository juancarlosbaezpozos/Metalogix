using Metalogix.Metabase.DataTypes;
using Metalogix.Metabase.Interfaces;
using Metalogix.Metabase.Widgets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;
using System.Threading;

namespace Metalogix.Metabase
{
    public class WorkspaceList : IEnumerable
    {
        private DataTable m_dataTable;

        private ArrayList m_workspaceCache;

        private bool m_bEventsSuspended;

        private MetabaseConnection m_connection;

        private object m_lock = new object();

        public MetabaseConnection Connection
        {
            get { return this.m_connection; }
        }

        public int Count
        {
            get { return this.m_workspaceCache.Count; }
        }

        public Workspace DefaultWorkspace
        {
            get
            {
                if (this.Count == 0)
                {
                    Workspace str = this.AddNew();
                    str.Name = Guid.Empty.ToString();
                    str.CommitChanges();
                }

                return this[0];
            }
        }

        public Workspace this[int index]
        {
            get
            {
                if (this.m_workspaceCache[index] == null)
                {
                    this.m_workspaceCache[index] = new Workspace(this.Connection, this.m_dataTable.Rows[index]);
                }

                return (Workspace)this.m_workspaceCache[index];
            }
        }

        public Workspace this[string sName]
        {
            get
            {
                for (int i = 0; i < this.Count; i++)
                {
                    if (this[i].Name == sName)
                    {
                        return this[i];
                    }
                }

                return null;
            }
        }

        public object WorkspaceLock
        {
            get { return this.m_lock; }
        }

        public WorkspaceList(MetabaseConnection connection, DataTable dataTable)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }

            if (dataTable == null)
            {
                throw new ArgumentNullException("dataTable");
            }

            this.m_connection = connection;
            this.m_dataTable = dataTable;
            this.m_dataTable.DefaultView.ListChanged += new ListChangedEventHandler(this.On_defaultView_ListChanged);
            this.m_workspaceCache = new ArrayList();
            for (int i = 0; i < this.m_dataTable.Rows.Count; i++)
            {
                this.m_workspaceCache.Add(null);
            }
        }

        public Workspace AddNew()
        {
            return this.AddNew(typeof(Record).FullName);
        }

        public Workspace AddNew(string strBaseType)
        {
            if (strBaseType == null)
            {
                throw new ArgumentNullException("strBaseType");
            }

            lock (this.WorkspaceLock)
            {
                if (string.Equals(typeof(Record).FullName, strBaseType) ||
                    strBaseType.Equals("Metalogix.UI.Metabase.Record") ||
                    strBaseType.Equals("Metalogix.UI.Metabase.ListItem"))
                {
                    DateTime now = DateTime.Now;
                    DataRow str = this.m_dataTable.NewRow();
                    str["WorkspaceID"] = Guid.NewGuid().ToString();
                    str["BaseType"] = strBaseType;
                    str["DateCreated"] = now;
                    str["DateModified"] = now;
                    this.m_bEventsSuspended = true;
                    this.m_dataTable.Rows.Add(str);
                    this.m_workspaceCache.Add(null);
                    this.m_bEventsSuspended = false;
                }
                else
                {
                    if (!strBaseType.Equals("Metalogix.UI.Metabase.Document"))
                    {
                        throw new ArgumentException(string.Concat(strBaseType, " is not a sub type of Item."));
                    }

                    DateTime dateTime = DateTime.Now;
                    DataRow dataRow = this.m_dataTable.NewRow();
                    dataRow["WorkspaceID"] = Guid.NewGuid().ToString();
                    dataRow["BaseType"] = strBaseType;
                    dataRow["DateCreated"] = dateTime;
                    dataRow["DateModified"] = dateTime;
                    this.m_bEventsSuspended = true;
                    this.m_dataTable.Rows.Add(dataRow);
                    this.m_workspaceCache.Add(null);
                    this.m_bEventsSuspended = false;
                    PropertySummaryList propertySummaryList =
                        new PropertySummaryList(this[this.m_dataTable.Rows.Count - 1].Records, null)
                        {
                            new PropertySummary(new RecordPropertyDescriptor("SourceURL", typeof(Url)))
                        };
                    this[this.m_dataTable.Rows.Count - 1].PropertySummaries = propertySummaryList.ToXml();
                }
            }

            this.On_defaultView_ListChanged(this,
                new ListChangedEventArgs(ListChangedType.ItemAdded, this.m_dataTable.Rows.Count - 1, -1));
            return this[this.m_dataTable.Rows.Count - 1];
        }

        public void CommitChanges()
        {
            lock (this.WorkspaceLock)
            {
                this.Connection.Adapter.CommitWorkspaces(this.m_dataTable);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return new WorkspaceList.WorkspaceListEnumerator(this);
        }

        private void On_defaultView_ListChanged(object sender, ListChangedEventArgs e)
        {
            if (this.ListChanged != null && !this.m_bEventsSuspended)
            {
                this.ListChanged(sender, e);
            }
        }

        public void Remove(object workspaceObject)
        {
            this.Remove(workspaceObject as Workspace);
        }

        public void Remove(Workspace workspace)
        {
            this.RemoveMultiple(new Workspace[] { workspace });
            this.CommitChanges();
        }

        public void RemoveAt(int index)
        {
            this.RemoveAt(index, true);
        }

        public void RemoveAt(int index, bool commitChanges)
        {
            lock (this.WorkspaceLock)
            {
                this[index].Records.Clear();
                this[index].Records.CommitChanges();
                this.m_dataTable.Rows[index].Delete();
                this.m_workspaceCache.RemoveAt(index);
                if (commitChanges)
                {
                    this.CommitChanges();
                }
            }
        }

        public void RemoveMultiple(params Workspace[] workspaces)
        {
            if (workspaces == null || (int)workspaces.Length == 0)
            {
                return;
            }

            List<string> strs = new List<string>();
            Workspace[] workspaceArray = workspaces;
            for (int i = 0; i < (int)workspaceArray.Length; i++)
            {
                Workspace workspace = workspaceArray[i];
                if (workspace != null && workspace.Exists())
                {
                    strs.Add(workspace.Name);
                }
            }

            lock (this.WorkspaceLock)
            {
                Stack<int> nums = new Stack<int>();
                for (int j = 0; j < this.Count; j++)
                {
                    if (strs.Contains(this[j].Name))
                    {
                        nums.Push(j);
                    }
                }

                while (nums.Count != 0)
                {
                    this.RemoveAt(nums.Pop(), false);
                }
            }
        }

        public void RemoveRange(IEnumerable<Workspace> workspaces)
        {
            this.RemoveMultiple((new List<Workspace>(workspaces)).ToArray());
        }

        public event ListChangedEventHandler ListChanged;

        private class WorkspaceListEnumerator : IEnumerator
        {
            private int m_iPos;

            private WorkspaceList m_workspaceList;

            public object Current
            {
                get
                {
                    if (this.m_iPos < 0 || this.m_iPos >= this.m_workspaceList.Count)
                    {
                        return null;
                    }

                    return this.m_workspaceList[this.m_iPos];
                }
            }

            public WorkspaceListEnumerator(WorkspaceList list)
            {
                this.m_workspaceList = list;
            }

            public bool MoveNext()
            {
                this.m_iPos++;
                return this.m_iPos < this.m_workspaceList.Count;
            }

            public void Reset()
            {
                this.m_iPos = 0;
            }
        }
    }
}