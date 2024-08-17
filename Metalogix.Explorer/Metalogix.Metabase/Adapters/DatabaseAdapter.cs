using Metalogix.Metabase;
using Metalogix.Metabase.Data;
using Metalogix.Metabase.DataTypes;
using Metalogix.Metabase.Interfaces;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Text;

namespace Metalogix.Metabase.Adapters
{
    public abstract class DatabaseAdapter : IMetabaseAdapter, IDisposable
    {
        protected const string DATASOURCE = "DataSource=";

        protected const string MAXDATABASESIZE = "Max Database Size=4091";

        protected const string DEFAULT_WORKSPACE_NAME = "Workspace 1";

        protected const int BATCHSIZE_DELETE = 100;

        protected const int BATCHSIZE_UPDATE = 100;

        protected const int BATCHSIZE_INSERT = 100;

        protected readonly string DEFAULT_WORKSPACE_BASETYPE = typeof(Record).FullName;

        private readonly DbConnection m_connection;

        private readonly object m_lock = new object();

        private string m_adapterContext;

        private Metalogix.Metabase.Interfaces.AdapterCallWrapper m_adapterWrapper;

        private int m_iNextItemNum;

        protected DbCommand m_cmdBatchDeleteItemProperties;

        protected DbCommand m_cmdBatchDeleteItems;

        protected DbCommand m_cmdBatchDeleteProperty;

        protected DbCommand m_cmdBatchInsertItems;

        protected DbCommand m_cmdBatchUpdateItemProperties;

        protected DbCommand m_cmdBatchUpdateItems;

        protected DbCommandBuilder m_cmdBuilderItemProperties;

        protected DbCommandBuilder m_cmdBuilderWorkspaces;

        protected DbCommand m_cmdDeleteItems;

        protected DbCommand m_cmdIdentity;

        protected DbCommand m_cmdInsertItemProperties;

        protected DbCommand m_cmdInsertItems;

        protected DbCommand m_cmdSelectItemProperties;

        protected DbCommand m_cmdSelectItems;

        protected DbCommand m_cmdSelectMaxItemNum;

        protected DbCommand m_cmdSelectSingleItem;

        protected DbCommand m_cmdSelectSingleItemProperties;

        protected DbCommand m_cmdSelectTextMoniker;

        protected DbCommand m_cmdUpdateItemProperiesStorage_1;

        protected DbCommand m_cmdUpdateItemProperties;

        protected DbCommand m_cmdUpdateItems;

        protected DbCommand m_cmdUpdateTextMoniker;

        protected DbDataAdapter m_dataAdapterItemProperties;

        protected DbDataAdapter m_dataAdapterItems;

        protected DbDataAdapter m_dataAdapterSingleItem;

        protected DbDataAdapter m_dataAdapterSingleItemProperties;

        protected DbDataAdapter m_dataAdapterWorkspaces;

        protected DbCommand m_oleDbCmdSelectWorkspaces;

        public Metalogix.Metabase.Interfaces.AdapterCallWrapper AdapterCallWrapper
        {
            get { return this.m_adapterWrapper; }
        }

        public string AdapterContext
        {
            get { return this.m_adapterContext; }
        }

        public abstract string AdapterType { get; }

        protected DatabaseAdapter(string context) : this(context, ConfigurationVariables.DefaultMetabaseCallWrapper)
        {
        }

        protected DatabaseAdapter(string context, Metalogix.Metabase.Interfaces.AdapterCallWrapper wrapper)
        {
            this.m_iNextItemNum = 0;
            this.m_adapterContext = context;
            this.m_adapterWrapper = wrapper;
            DbConnection dbConnection = null;
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() => dbConnection = this.Initialize()));
            this.m_connection = dbConnection;
            if (this.m_connection == null)
            {
                throw new NullReferenceException("Database adapter cannot be null.");
            }

            if (this.m_connection.State == ConnectionState.Broken || this.m_connection.State == ConnectionState.Closed)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() => this.m_connection.Open()));
            }
        }

        private void BatchInsertItemProperties(ArrayList changedRows)
        {
            if (changedRows.Count == 0)
            {
                return;
            }

            this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
            {
                DataRow item = (DataRow)changedRows[0];
                string str = DatabaseUtils.BuildWhereClause(changedRows);
                StringBuilder stringBuilder = new StringBuilder(128);
                StringBuilder stringBuilder1 = new StringBuilder(128);
                foreach (DataColumn column in item.Table.Columns)
                {
                    object obj = item[column, DataRowVersion.Current];
                    if (column.Ordinal == 0 || obj == DBNull.Value)
                    {
                        continue;
                    }

                    stringBuilder1.Append(column.ColumnName);
                    stringBuilder1.Append(",");
                    stringBuilder.Append(DatabaseUtils.GetSQLValue(obj));
                    stringBuilder.Append(" as ");
                    stringBuilder.Append(column.ColumnName);
                    stringBuilder.Append(",");
                }

                string str1 = stringBuilder.ToString(0, stringBuilder.Length - 1);
                string str2 = stringBuilder1.ToString(0, stringBuilder1.Length - 1);
                this.m_cmdBatchInsertItems.CommandText = string.Concat(new string[]
                {
                    "INSERT INTO ItemProperties (ItemID, ", str2, ") SELECT ItemID, ", str1,
                    " FROM Items Where ItemID IN (", str, ")"
                });
                lock (this.m_lock)
                {
                    this.m_cmdBatchInsertItems.ExecuteScalar();
                }
            }));
        }

        private void BatchSaveItemProperties(DataRowState changeState, ArrayList changedRows)
        {
            DataRowState dataRowState = changeState;
            if (dataRowState == DataRowState.Added)
            {
                this.BatchInsertItemProperties(changedRows);
                return;
            }

            if (dataRowState != DataRowState.Modified)
            {
                return;
            }

            this.BatchUpdateItemProperties(changedRows);
        }

        private void BatchUpdateItemProperties(ArrayList array)
        {
            if (array.Count == 0)
            {
                return;
            }

            this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
            {
                DataRow item = (DataRow)array[0];
                string str = (string)item[1];
                string str1 = DatabaseUtils.BuildWhereClause(array);
                bool flag = false;
                StringBuilder stringBuilder = new StringBuilder(128);
                foreach (DataColumn column in item.Table.Columns)
                {
                    object obj = item[column, DataRowVersion.Original];
                    object item1 = item[column, DataRowVersion.Current];
                    if (obj == item1)
                    {
                        continue;
                    }

                    flag = true;
                    stringBuilder.Append(column.ColumnName);
                    stringBuilder.Append(" = ");
                    stringBuilder.Append(DatabaseUtils.GetSQLValue(item1));
                    stringBuilder.Append(",");
                }

                if (!flag)
                {
                    return;
                }

                this.m_cmdBatchUpdateItemProperties.CommandText = string.Concat(new string[]
                {
                    "UPDATE ItemProperties Set ", stringBuilder.ToString(0, stringBuilder.Length - 1),
                    " WHERE PropertyName = '", str, "' AND ItemID IN (", str1, ")"
                });
                lock (this.m_lock)
                {
                    this.m_cmdBatchUpdateItemProperties.ExecuteScalar();
                }
            }));
        }

        private void BatchUpdateItems(ArrayList array, Hashtable values)
        {
            if (array.Count == 0)
            {
                return;
            }

            this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
            {
                string str = DatabaseUtils.BuildWhereClause(array);
                StringBuilder stringBuilder = new StringBuilder(128);
                DateTime utcNow = DateTime.UtcNow;
                foreach (object key in values.Keys)
                {
                    if (!string.Equals((string)key, "ModificationDate", StringComparison.OrdinalIgnoreCase))
                    {
                        stringBuilder.Append(key);
                        stringBuilder.Append(" = ");
                        stringBuilder.Append(DatabaseUtils.GetSQLValue(values[key]));
                        stringBuilder.Append(",");
                    }
                    else
                    {
                        object item = values[key];
                        if (item == null)
                        {
                            continue;
                        }

                        if (!(item is DateTime))
                        {
                            DateTime.TryParse(values[key].ToString(), out utcNow);
                        }
                        else
                        {
                            utcNow = (DateTime)item;
                        }
                    }
                }

                this.m_cmdBatchUpdateItems.CommandText = string.Format(
                    "UPDATE Items Set {0} ModificationDate=@ModificationDate WHERE ItemID IN ({1})",
                    (stringBuilder.Length > 0
                        ? string.Concat(stringBuilder.ToString(0, stringBuilder.Length - 1), ",")
                        : string.Empty), str);
                this.m_cmdBatchUpdateItems.Parameters["@ModificationDate"].Value = utcNow;
                lock (this.m_lock)
                {
                    this.m_cmdBatchUpdateItems.ExecuteScalar();
                }
            }));
        }

        public DbTransaction BeginTransaction()
        {
            return this.m_connection.BeginTransaction();
        }

        public void Close()
        {
            if (this.m_connection.State == ConnectionState.Open)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() => this.m_connection.Close()));
            }
        }

        public void CommitItem(DataRow row)
        {
            lock (this.m_lock)
            {
                this.AdapterCallWrapper(
                    new AdapterCallWrapperAction(() => this.m_dataAdapterItems.Update(new DataRow[] { row })));
            }
        }

        public void CommitItemData(DataSet dataSet)
        {
            lock (this.m_lock)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
                {
                    this.m_dataAdapterItems.Update(dataSet, "Items");
                    this.m_dataAdapterItemProperties.Update(dataSet, "ItemProperties");
                }));
            }
        }

        public void CommitItemProperties(ArrayList arrayRows)
        {
            if (arrayRows == null || arrayRows.Count == 0)
            {
                return;
            }

            DataRow[] dataRowArray = new DataRow[arrayRows.Count];
            arrayRows.CopyTo(dataRowArray);
            lock (this.m_lock)
            {
                try
                {
                    this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
                        this.m_dataAdapterItemProperties.Update(dataRowArray)));
                }
                catch
                {
                }
            }
        }

        public void CommitItemProperties(DataTable dataTable)
        {
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
            {
                DataTable changes = dataTable.GetChanges();
                if (changes == null || changes.Rows.Count == 0)
                {
                    return;
                }

                ArrayList arrayLists = new ArrayList(32);
                DataRowState rowState = changes.Rows[0].RowState;
                foreach (DataRow row in changes.Rows)
                {
                    DataRowState dataRowState = row.RowState;
                    if (dataRowState != rowState || arrayLists.Count >= 100)
                    {
                        this.IssueBatchItemPropertiesCmd(rowState, arrayLists);
                        arrayLists.Clear();
                    }

                    if (dataRowState == DataRowState.Added || dataRowState == DataRowState.Modified ||
                        dataRowState == DataRowState.Deleted)
                    {
                        arrayLists.Add(row);
                    }

                    rowState = row.RowState;
                }

                this.IssueBatchItemPropertiesCmd(rowState, arrayLists);
                arrayLists.Clear();
                dataTable.AcceptChanges();
            }));
        }

        public void CommitItems(DataTable dataTableItems, DataTable dataTableItemProperties)
        {
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
            {
                DataTable changes = dataTableItems.GetChanges();
                if (changes == null || changes.Rows.Count == 0)
                {
                    return;
                }

                ArrayList arrayLists = new ArrayList(32);
                DataRowState rowState = changes.Rows[0].RowState;
                foreach (DataRow row in changes.Rows)
                {
                    DataRowState dataRowState = row.RowState;
                    int count = arrayLists.Count;
                    if (dataRowState != rowState || dataRowState == DataRowState.Added && count >= 100 ||
                        dataRowState == DataRowState.Modified && count >= 100 ||
                        dataRowState == DataRowState.Deleted && count >= 100)
                    {
                        this.IssueBatchItemCmd(rowState, arrayLists, dataTableItemProperties);
                        arrayLists.Clear();
                    }

                    if (dataRowState == DataRowState.Added || dataRowState == DataRowState.Modified ||
                        dataRowState == DataRowState.Deleted)
                    {
                        arrayLists.Add(row);
                    }

                    rowState = row.RowState;
                }

                this.IssueBatchItemCmd(rowState, arrayLists, dataTableItemProperties);
                arrayLists.Clear();
                dataTableItems.AcceptChanges();
            }));
        }

        public void CommitWorkspace(DataRow row)
        {
            lock (this.m_lock)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
                    this.m_dataAdapterWorkspaces.Update(new DataRow[] { row })));
            }
        }

        public void CommitWorkspaces(DataTable dataTable)
        {
            lock (this.m_lock)
            {
                this.AdapterCallWrapper(
                    new AdapterCallWrapperAction(() => this.m_dataAdapterWorkspaces.Update(dataTable)));
            }
        }

        protected void CreateSchema(DbConnection connection)
        {
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
            {
                using (DbCommand dbCommand = connection.CreateCommand())
                {
                    dbCommand.Connection = connection;
                    dbCommand.CommandText =
                        "CREATE TABLE Workspaces (\r\n                WorkspaceID nvarchar(36) PRIMARY KEY,\r\n                Name nvarchar(255),\r\n                CustomPropertyDefinitions ntext,\r\n                BaseType ntext,\r\n                ViewDefinitions ntext,\r\n                PropertySummaries ntext,\r\n                Settings ntext,\r\n                DateCreated datetime,\r\n                DateModified datetime\r\n                )";
                    dbCommand.Parameters.Clear();
                    dbCommand.ExecuteNonQuery();
                    dbCommand.CommandText =
                        "CREATE TABLE Items (\r\n                ItemID nvarchar(36) PRIMARY KEY,\r\n                ItemNum integer,\r\n                WorkspaceID nvarchar(36),\r\n                CreationDate datetime,\r\n                ModificationDate datetime\r\n                )";
                    dbCommand.Parameters.Clear();
                    dbCommand.ExecuteNonQuery();
                    dbCommand.CommandText =
                        "CREATE TABLE ItemProperties (\r\n                PropertyName nvarchar(50) NOT NULL,\r\n                ItemID nvarchar(36) NOT NULL,\r\n                ShortTextValue nvarchar(255),\r\n                LongTextValue ntext,\r\n                TextBlobValue ntext,\r\n                NumberValue numeric (18,2),\r\n                DateValue datetime,\r\n                PRIMARY KEY(PropertyName, ItemID)\r\n                )";
                    dbCommand.Parameters.Clear();
                    dbCommand.ExecuteNonQuery();
                    dbCommand.CommandText = "CREATE UNIQUE INDEX IX_WORKSPACEID ON Workspaces (WorkspaceID)";
                    dbCommand.Parameters.Clear();
                    dbCommand.ExecuteNonQuery();
                    dbCommand.CommandText = "CREATE UNIQUE INDEX IX_ITEMID ON Items (ItemID)";
                    dbCommand.Parameters.Clear();
                    dbCommand.ExecuteNonQuery();
                    dbCommand.CommandText = "CREATE INDEX IX_WORKSPACEID ON Items (WorkspaceID)";
                    dbCommand.Parameters.Clear();
                    dbCommand.ExecuteNonQuery();
                    dbCommand.CommandText = "CREATE INDEX IX_ITEMID ON ItemProperties (ItemID)";
                    dbCommand.Parameters.Clear();
                    dbCommand.ExecuteNonQuery();
                }
            }));
        }

        public void Dispose()
        {
            this.m_connection.Dispose();
        }

        public void ExpandTextStorage(string sPropertyName, string sWorkspaceID)
        {
            lock (this.m_lock)
            {
                this.m_cmdUpdateItemProperiesStorage_1.Parameters[0].Value = sPropertyName;
                this.m_cmdUpdateItemProperiesStorage_1.Parameters[1].Value = sWorkspaceID;
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
                    this.m_cmdUpdateItemProperiesStorage_1.ExecuteNonQuery()));
            }
        }

        public DataSet FetchItems(string guidWorkspaceID)
        {
            DataSet dataSet = new DataSet("ItemsData");
            IndexedDataTable itemDataTable = Record.GetItemDataTable();
            dataSet.Tables.Add(itemDataTable);
            DateTime now = DateTime.Now;
            this.m_dataAdapterItems.SelectCommand.Parameters[0].Value = guidWorkspaceID;
            lock (this.m_lock)
            {
                this.AdapterCallWrapper(
                    new AdapterCallWrapperAction(() => this.m_dataAdapterItems.Fill(dataSet, "Items")));
            }

            DateTime dateTime = DateTime.Now;
            itemDataTable.AddIndex("ItemID", itemDataTable.Columns["ItemID"], true);
            IndexedDataTable itemPropertiesDataTable = Record.GetItemPropertiesDataTable();
            dataSet.Tables.Add(itemPropertiesDataTable);
            DateTime now1 = DateTime.Now;
            this.m_dataAdapterItemProperties.SelectCommand.Parameters[0].Value = guidWorkspaceID;
            lock (this.m_lock)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
                    this.m_dataAdapterItemProperties.Fill(dataSet, "ItemProperties")));
            }

            DateTime dateTime1 = DateTime.Now;
            itemPropertiesDataTable.AddIndex("ItemID", itemPropertiesDataTable.Columns["ItemID"], false);
            DateTime now2 = DateTime.Now;
            lock (this.m_lock)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
                    this.m_iNextItemNum = (this.m_cmdSelectMaxItemNum.ExecuteScalar() is int
                        ? (int)this.m_cmdSelectMaxItemNum.ExecuteScalar()
                        : 0)));
            }

            this.m_iNextItemNum++;
            return dataSet;
        }

        public DataSet FetchSingleItem(string workspaceId, string sourceUrl)
        {
            DataSet dataSet = new DataSet("ItemsData");
            IndexedDataTable itemDataTable = Record.GetItemDataTable();
            dataSet.Tables.Add(itemDataTable);
            lock (this.m_lock)
            {
                this.m_dataAdapterSingleItem.SelectCommand.Parameters[0].Value = workspaceId;
                this.m_dataAdapterSingleItem.SelectCommand.Parameters[1].Value = sourceUrl;
                this.m_dataAdapterSingleItem.Fill(dataSet, "Items");
            }

            itemDataTable.AddIndex("ItemID", itemDataTable.Columns["ItemID"], true);
            string str = (itemDataTable.Rows.Count == 1 ? (string)itemDataTable.Rows[0]["ItemID"] : string.Empty);
            IndexedDataTable itemPropertiesDataTable = Record.GetItemPropertiesDataTable();
            dataSet.Tables.Add(itemPropertiesDataTable);
            lock (this.m_lock)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
                {
                    this.m_dataAdapterSingleItemProperties.SelectCommand.Parameters[0].Value = workspaceId;
                    this.m_dataAdapterSingleItemProperties.SelectCommand.Parameters[1].Value = str;
                    this.m_dataAdapterSingleItemProperties.Fill(dataSet, "ItemProperties");
                }));
            }

            itemPropertiesDataTable.AddIndex("ItemID", itemPropertiesDataTable.Columns["ItemID"], false);
            lock (this.m_lock)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
                    this.m_iNextItemNum = (this.m_cmdSelectMaxItemNum.ExecuteScalar() is int
                        ? (int)this.m_cmdSelectMaxItemNum.ExecuteScalar()
                        : 0)));
            }

            this.m_iNextItemNum++;
            return dataSet;
        }

        public DataTable FetchWorkspaceList()
        {
            DataTable dataTable = new DataTable("Workspaces");
            lock (this.m_lock)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
                {
                    this.m_dataAdapterWorkspaces.FillSchema(dataTable, SchemaType.Source);
                    this.m_dataAdapterWorkspaces.Fill(dataTable);
                }));
            }

            DatabaseUtils.ValidateTableSchema(dataTable, Workspace.Schema);
            return dataTable;
        }

        public T GetConnectionAs<T>()
            where T : DbConnection
        {
            return (T)this.m_connection;
        }

        public int IncrementItemNum()
        {
            DatabaseAdapter databaseAdapter = this;
            int mINextItemNum = databaseAdapter.m_iNextItemNum;
            int num = mINextItemNum;
            databaseAdapter.m_iNextItemNum = mINextItemNum + 1;
            return num;
        }

        protected abstract DbConnection Initialize();

        private void IssueBatchItemCmd(DataRowState rowState, ArrayList arrayDataRows,
            DataTable dataTableItemPropertyDeletions)
        {
            this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
            {
                if (rowState != DataRowState.Added)
                {
                    if (rowState == DataRowState.Deleted)
                    {
                        string str = DatabaseUtils.BuildWhereClause(arrayDataRows);
                        Hashtable hashtables = new Hashtable(arrayDataRows.Count);
                        foreach (DataRow arrayDataRow in arrayDataRows)
                        {
                            hashtables[arrayDataRow["ItemID", DataRowVersion.Original]] = 1;
                        }

                        this.m_cmdBatchDeleteItems.CommandText =
                            string.Concat("DELETE FROM Items WHERE ItemID IN (", str, ")");
                        this.m_cmdBatchDeleteItemProperties.CommandText =
                            string.Concat("DELETE FROM ItemProperties WHERE ItemID IN (", str, ")");
                        lock (this.m_lock)
                        {
                            this.m_cmdBatchDeleteItemProperties.ExecuteNonQuery();
                            this.m_cmdBatchDeleteItems.ExecuteNonQuery();
                        }

                        ArrayList arrayLists = new ArrayList(64);
                        foreach (object key in hashtables.Keys)
                        {
                            arrayLists.AddRange(
                                ((IndexedDataTable)dataTableItemPropertyDeletions).FindAllRows("ItemID", key));
                        }

                        DataRow[] dataRowArray = new DataRow[arrayLists.Count];
                        arrayLists.CopyTo(dataRowArray);
                        DataRow[] dataRowArray1 = dataRowArray;
                        for (int i = 0; i < (int)dataRowArray1.Length; i++)
                        {
                            dataRowArray1[i].AcceptChanges();
                        }

                        hashtables.Clear();
                        return;
                    }

                    if (rowState == DataRowState.Modified)
                    {
                        Hashtable hashtables1 = new Hashtable(4);
                        ArrayList arrayLists1 = new ArrayList(32);
                        foreach (DataRow dataRow in arrayDataRows)
                        {
                            foreach (DataColumn column in dataRow.Table.Columns)
                            {
                                object item = dataRow[column, DataRowVersion.Original];
                                object obj = dataRow[column, DataRowVersion.Current];
                                if (item.Equals(obj))
                                {
                                    continue;
                                }

                                if (!hashtables1.ContainsKey(column.ColumnName))
                                {
                                    hashtables1[column.ColumnName] = obj;
                                    arrayLists1.Add(dataRow);
                                }
                                else if (hashtables1[column.ColumnName].Equals(obj))
                                {
                                    arrayLists1.Add(dataRow);
                                }
                                else
                                {
                                    this.BatchUpdateItems(arrayLists1, hashtables1);
                                    hashtables1.Clear();
                                    arrayLists1.Clear();
                                }
                            }
                        }

                        if (arrayLists1.Count > 0)
                        {
                            this.BatchUpdateItems(arrayLists1, hashtables1);
                            arrayLists1.Clear();
                        }

                        hashtables1.Clear();
                    }
                }
                else
                {
                    foreach (DataRow arrayDataRow1 in arrayDataRows)
                    {
                        lock (this.m_lock)
                        {
                            this.m_dataAdapterItems.Update(new DataRow[] { arrayDataRow1 });
                        }
                    }
                }
            }));
        }

        private void IssueBatchItemPropertiesCmd(DataRowState rowState, ArrayList arrayDataRows)
        {
            if (arrayDataRows == null || arrayDataRows.Count == 0)
            {
                return;
            }

            this.AdapterCallWrapper(new AdapterCallWrapperAction(() =>
            {
                if (rowState == DataRowState.Deleted)
                {
                    ArrayList arrayLists = new ArrayList(32);
                    string str = null;
                    foreach (DataRow arrayDataRow in arrayDataRows)
                    {
                        string str1 = arrayDataRow["PropertyName", DataRowVersion.Original].ToString();
                        if (str == null)
                        {
                            str = str1;
                        }
                        else if (!str.Equals(str1))
                        {
                            string str2 = DatabaseUtils.BuildWhereClause(arrayLists);
                            this.m_cmdBatchDeleteItemProperties.CommandText = string.Concat(new string[]
                            {
                                "DELETE FROM ItemProperties WHERE PropertyName = ", str, " AND ItemID IN (", str2, ")"
                            });
                            lock (this.m_lock)
                            {
                                this.m_cmdBatchDeleteItemProperties.ExecuteScalar();
                            }

                            arrayLists.Clear();
                            str = str1;
                        }

                        arrayLists.Add(arrayDataRow);
                    }

                    string str3 = DatabaseUtils.BuildWhereClause(arrayLists);
                    this.m_cmdBatchDeleteItemProperties.CommandText = string.Concat(new string[]
                    {
                        "DELETE FROM ItemProperties WHERE PropertyName = ", DatabaseUtils.GetSQLValue(str),
                        " AND ItemID IN (", str3, ")"
                    });
                    lock (this.m_lock)
                    {
                        this.m_cmdBatchDeleteItemProperties.ExecuteScalar();
                    }
                }
                else if (rowState == DataRowState.Modified || rowState == DataRowState.Added)
                {
                    ArrayList arrayLists1 = new ArrayList();
                    DataRow item = (DataRow)arrayDataRows[0];
                    foreach (DataRow dataRow in arrayDataRows)
                    {
                        if ((!dataRow[1].Equals(item[1]) || !dataRow[2].Equals(item[2]) ||
                             !dataRow[3].Equals(item[3]) || !dataRow[6].Equals(item[6])
                                ? true
                                : !dataRow[5].Equals(item[5])))
                        {
                            this.BatchSaveItemProperties(rowState, arrayLists1);
                            arrayLists1.Clear();
                        }

                        arrayLists1.Add(dataRow);
                        item = dataRow;
                    }

                    if (arrayLists1.Count > 0)
                    {
                        this.BatchSaveItemProperties(rowState, arrayLists1);
                    }
                }
            }));
        }

        public string LoadTextMoniker(TextMoniker txtMon)
        {
            string str;
            lock (this.m_lock)
            {
                if (txtMon == null || txtMon.ParentItem == null)
                {
                    str = null;
                }
                else
                {
                    object obj = null;
                    this.m_cmdSelectTextMoniker.Parameters[0].Value = txtMon.ParentItem.ID;
                    this.m_cmdSelectTextMoniker.Parameters[1].Value = txtMon.Name;
                    this.AdapterCallWrapper(
                        new AdapterCallWrapperAction(() => obj = this.m_cmdSelectTextMoniker.ExecuteScalar()));
                    if (obj == null)
                    {
                        str = null;
                    }
                    else
                    {
                        str = obj.ToString();
                    }
                }
            }

            return str;
        }

        public void Open()
        {
            if (this.m_connection.State != ConnectionState.Open && this.m_connection.Database != null &&
                this.m_connection.DataSource != null)
            {
                this.AdapterCallWrapper(new AdapterCallWrapperAction(() => this.m_connection.Open()));
            }
        }

        public void SaveTextMoniker(TextMoniker txtMon, string sFullText)
        {
            lock (this.m_lock)
            {
                if (txtMon != null && txtMon.ParentItem != null)
                {
                    this.m_cmdUpdateTextMoniker.Parameters["@TextBlobValue"].Value = sFullText;
                    this.m_cmdUpdateTextMoniker.Parameters["@ItemID"].Value = txtMon.ParentItem.ID;
                    this.m_cmdUpdateTextMoniker.Parameters["@PropertyName"].Value = txtMon.Name;
                    this.AdapterCallWrapper(
                        new AdapterCallWrapperAction(() => this.m_cmdUpdateTextMoniker.ExecuteScalar()));
                }
            }
        }

        protected void UpdateNumericDataType(DbConnection connection)
        {
            using (DbCommand dbCommand = connection.CreateCommand())
            {
                dbCommand.CommandText =
                    "SELECT NUMERIC_SCALE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='ItemProperties' AND COLUMN_NAME='NumberValue'";
                dbCommand.Connection = connection;
                if (Convert.ToInt16(dbCommand.ExecuteScalar()) == 0)
                {
                    dbCommand.CommandText = "ALTER TABLE ItemProperties ALTER COLUMN NumberValue numeric(18,2)";
                    dbCommand.ExecuteNonQuery();
                }
            }
        }
    }
}